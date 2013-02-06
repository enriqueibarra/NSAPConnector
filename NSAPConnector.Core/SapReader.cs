using System;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    /// <summary>
    /// This class simulates a DataReader behaviour
    /// without using the same principles in the implementation.
    /// Basically it reads row by row data from an RfcTable.
    /// </summary>
    public class SapDataReader
    {
        private readonly IRfcTable _rfcTable;

        private int _internalIndex;

        /// <summary>
        /// Total number of rows from the underlying RfcTable.
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Current row from the table.
        /// </summary>
        public IRfcStructure Item { get; private set; }

        /// <summary>
        /// Returns true if underlying RfcTable contains at
        /// least one row, otherwise it returns false.
        /// </summary>
        public bool HasRows
        {
            get { return RowCount > 0; }
        }

        internal SapDataReader(IRfcTable rfcTable)
        {
            if (rfcTable == null)
            {
                throw new ArgumentNullException("rfcTable");
            }

            _rfcTable = rfcTable;

            RowCount = rfcTable.RowCount;

            ResetIndex();
        }

        /// <summary>
        /// Set current index to the begining of the table.
        /// </summary>
        public void ResetIndex()
        {
            _internalIndex = -1;
        }

        /// <summary>
        /// Forwards to the next row.
        /// </summary>
        /// <returns>True if a new row was read. 
        /// False if no row read and the end of the table has been reached.</returns>
        public bool Read()
        {
            if (!HasRows)
            {
                //Table has no rows
                return false;
            }

            _internalIndex++;

            if (_internalIndex == RowCount)
            {
                //reached the end of table
                return false;
            }

            //get next row
            _rfcTable.CurrentIndex = _internalIndex;
            Item = _rfcTable.CurrentRow;

            return true;
        }
    }
}
