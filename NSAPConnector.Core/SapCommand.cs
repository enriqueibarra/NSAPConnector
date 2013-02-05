using System;
using System.Collections.Generic;
using System.Data;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;
using System.Text.RegularExpressions;

namespace NSAPConnector
{
    public class SapCommand
    {
        private RfcRepository Repository
        {
            get
            {
                if(Connection == null)
                {
                    throw new NSAPConnectorException("Connection is not set.");
                }

                return Connection.Destination.Repository;
            }
        }

        public string CommandText { get; set; }
        public SapConnection Connection { get; set; }
        public SapTransaction Transaction { get; set; }
        public List<SapParameter> Parameters { get; set; }
 
        #region Ctor
        
        public SapCommand()
        {
            Parameters = new List<SapParameter>();
        }

        public SapCommand(string cmdText) : this()
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            CommandText = cmdText;
        }

        public SapCommand(string cmdText, SapConnection connection)
            : this(cmdText)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            Connection = connection;
        }

        public SapCommand(string cmdText, SapConnection connection, SapTransaction transaction)
            : this(cmdText, connection)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            Transaction = transaction;
        } 

        #endregion

        public DataSet ExecuteDataSet()
        {
            var resultDataSet = new DataSet();

            var functionRef = ExecuteRfc();

            //populate dataset with the data from all the result tables of the invoked RFC
            foreach(var tableName in GetResultTableNames(functionRef.Metadata.ToString()))
            {
                var rfcTable = functionRef.GetTable(tableName);

                resultDataSet.Tables.Add(ConvertRfcTableToDataTable(rfcTable, tableName));

                PopulateDataTable(rfcTable, resultDataSet.Tables[tableName]);
            }

            return resultDataSet;
        }

        public Dictionary<string, IRfcTable> ExecuteRfcTables()
        {
            var resultDictionary = new Dictionary<string, IRfcTable>();

            var functionRef = ExecuteRfc();

            //populate dictionary with the result tables of the invoked RFC
            foreach (var tableName in GetResultTableNames(functionRef.Metadata.ToString()))
            {
                var rfcTable = functionRef.GetTable(tableName);

                resultDictionary[tableName] = rfcTable;
            }

            return resultDictionary;
        }

        public IRfcFunction ExecuteRfc()
        {
            IRfcFunction functionRef;

            try
            {
                //map commandText to the corresponding server BAPI
                functionRef = Repository.CreateFunction(CommandText);
            }
            catch (Exception ex)
            {
                throw new NSAPConnectorException(
                    string.Format("An exception occurred when trying to find corresponding function for the '{0}' on server.Check inner exception for more details.", CommandText), ex);
            }

            //set RFC parameters
            foreach (var param in Parameters)
            {
                functionRef.SetValue(param.Name, param.Value);
            }

            //if there is a transaction then add current RFC to it
            if (Transaction != null)
            {
                Transaction.AddFunction(functionRef);
            }

            try
            {
                //Invoke RFC
                functionRef.Invoke(Connection.Destination);
            }
            catch (Exception ex)
            {
                throw new NSAPConnectorException(
                    string.Format("An exception occurred when trying to execute corresponding function for the '{0}' on server.Check inner exception for more details.", CommandText), ex);
            }

            return functionRef;
        }

        private void PopulateDataTable(IRfcTable rfcTable, DataTable dataTable)
        {
            for (var i = 0; i < rfcTable.RowCount; i++)
            {
                rfcTable.CurrentIndex = i;

                var row = dataTable.NewRow();
                
                foreach(DataColumn column in dataTable.Columns)
                {
                    try
                    {
                        SetDataColumnValueFromRfcColumn(rfcTable.CurrentRow[column.ColumnName], row, column);
                    }
                    catch(Exception ex)
                    {
                        throw new NSAPConnectorException(
                            string.Format("An error occurred when trying to add value from column '{0}' from table '{1}'. For more details check inner exception.", column.ColumnName, dataTable.TableName), ex);
                    }
                }

                dataTable.Rows.Add(row);
            }
        }

        private DataTable ConvertRfcTableToDataTable(IRfcTable tableToConvert, string tableName)
        {
            var columnsMetadata = tableToConvert.Metadata.LineType;
            var dataTable = new DataTable(tableName);

            for (var i = 0; i < columnsMetadata.FieldCount; i++)
            {
                dataTable.Columns
                    .Add(columnsMetadata[i].Name, GetCorrespondingDotNetType(columnsMetadata[i].DataType.ToString(), columnsMetadata[i].NucLength));
            }

            return dataTable;
        }

        private Type GetCorrespondingDotNetType(string sapType, int typeLength)
        {
            switch(sapType)
            {
                case "BYTE" :
                    return typeof (byte[]);

                case "INT":
                    return typeof(int);

                case "INT1":
                    return typeof(byte);

                case "INT2":
                    return typeof(short);

                case "FLOAT":
                    return typeof(double);

                case "NUM":

                    if(typeLength <= 9 )
                    {
                        return typeof (int);
                    }

                    if(typeLength <= 19)
                    {
                        return typeof (long);
                    }

                    return typeof(string);

                default:
                    return typeof (string);
            }
        }

        private void SetDataColumnValueFromRfcColumn(IRfcField rfcColumn, DataRow dataRow, DataColumn dataColumn)
        {

            if( dataColumn.DataType == typeof(byte[]))
            {
                dataRow[dataColumn.ColumnName] = rfcColumn.GetByteArray();

                return;
            }

            if (dataColumn.DataType == typeof(int))
            {
                dataRow[dataColumn.ColumnName] = rfcColumn.GetInt();

                return;
            }

            if (dataColumn.DataType == typeof(byte))
            {
                dataRow[dataColumn.ColumnName] = rfcColumn.GetByte();

                return;
            }

            if (dataColumn.DataType == typeof(short))
            {
                dataRow[dataColumn.ColumnName] = rfcColumn.GetShort();

                return;
            }

            if (dataColumn.DataType == typeof(double))
            {
                dataRow[dataColumn.ColumnName] = rfcColumn.GetDouble();

                return;
            }

            dataRow[dataColumn.ColumnName] = rfcColumn.GetString();
        }

        private IEnumerable<string> GetResultTableNames(string metadataDescription)
        {
            var regex = new Regex(@"TABLES (\w+):");

            foreach(Match match in regex.Matches(metadataDescription))
            {
                yield return match.Groups[1].Value;
            }
        }
    }
}
