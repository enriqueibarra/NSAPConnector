using System;
using System.Collections.Generic;
using System.Data;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;
using System.Text.RegularExpressions;
using System.Linq;

namespace NSAPConnector
{
    /// <summary>
    /// This class provides all means for
    /// configuring and executing a RFC call
    /// </summary>
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

        /// <summary>
        /// SAP command to execute.
        /// Name of the function.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// SapConnection to be used for
        /// executing SAP command
        /// </summary>
        public SapConnection Connection { get; set; }

        /// <summary>
        /// If RFC should be transactional this property should be set
        /// to an instance of the SapTransaction
        /// </summary>
        public SapTransaction Transaction { get; set; }

        /// <summary>
        /// RFC parameters
        /// </summary>
        public SapParameterCollection Parameters { get; set; }
 
        #region Ctors
        
        public SapCommand()
        {
            Parameters = new SapParameterCollection();
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

        /// <summary>
        /// Executes configured sap command with the 
        /// given parameters and all results of type table will
        /// be put in a DataSet.
        /// </summary>
        /// <returns>DataSet which will contain a DataTable for each table result returned by RFC call. 
        /// DataTables will contain columns with the same name like rfcTable's columns and with the corresponding .net type.</returns>
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

        /// <summary>
        /// Executes configured sap command with the 
        /// given parameters and all results of type table will
        /// be stored in a dictionary.
        /// </summary>
        /// <returns>Dictionary with RfcTables (key = table name, value = RfcTable instance)</returns>
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

        /// <summary>
        /// Executes configured sap command with the
        /// given parameters and the RfcFunction reference 
        /// is returned.
        /// </summary>
        /// <returns>RfcFunction reference after invokation</returns>
        public IRfcFunction ExecuteRfc()
        {

            if (string.IsNullOrEmpty(CommandText))
            {
                throw new NSAPConnectorException("Command text cannot be null or empty.");
            }

            if (Connection == null)
            {
                throw new NSAPConnectorException("Connection is not set.");
            }

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

        /// <summary>
        /// Executes configured sap command with the 
        /// given parameters and returns a SapDataReader
        /// based on the table result. 
        /// </summary>
        /// <param name="tableName">If this parameter is provided then SapDataReader 
        /// is based on the table result with the name equal with tableName, ohterwise
        /// the first table result will be used.</param>
        /// <returns>SapDataReader instance</returns>
        public SapDataReader ExecuteReader(string tableName = null)
        {
            var rfcFunctionRef = ExecuteRfc();

            //if no result table name was provided then try to find first result table name returned by the current BAPI
            if(string.IsNullOrEmpty(tableName))
            {
                tableName = GetResultTableNames(rfcFunctionRef.Metadata.ToString()).FirstOrDefault();
            }
            
            var resultTable = string.IsNullOrEmpty(tableName)
                                  ? null
                                  : rfcFunctionRef.GetTable(tableName);

            return new SapDataReader(resultTable);
        }

        /// <summary>
        /// Populates a DataTable with the values
        /// from the given RfcTable
        /// </summary>
        /// <param name="rfcTable">Source table</param>
        /// <param name="dataTable">Destination table</param>
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

        /// <summary>
        /// Creates a DataTable which has the same structure whith
        /// the given RfcTable.
        /// </summary>
        /// <param name="tableToConvert">RfcTable to clone.</param>
        /// <param name="tableName">Name of the DataTable to be created.</param>
        /// <returns>DataTable instance.</returns>
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

        /// <summary>
        /// Maps SAP types to .Net types.
        /// </summary>
        /// <param name="sapType">SAP type to map</param>
        /// <param name="typeLength">Length of the SAP type (ex: CHAR(16) => typeLength = 16)</param>
        /// <returns>.Net type</returns>
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

        /// <summary>
        /// Extract value from RfcColumn by using the correct method for the 
        /// column type and put extracted value into the DataColumn.
        /// </summary>
        /// <param name="rfcColumn">Value source.</param>
        /// <param name="dataRow">Value destination.</param>
        /// <param name="dataColumn">Column to be set from destination.</param>
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

        /// <summary>
        /// Extract all table names from the rfc results metadata.
        /// </summary>
        /// <param name="metadataDescription">Result metadata description.</param>
        /// <returns>List of the found result tables.</returns>
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
