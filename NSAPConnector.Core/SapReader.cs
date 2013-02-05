using System;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    public class SapDataReader
    {
        private readonly IRfcTable _rfcTable;

        private int _internalIndex;

        public int RowCount { get; private set; }

        public IRfcStructure Item { get; private set; }

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

        public void ResetIndex()
        {
            _internalIndex = -1;
        }

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
