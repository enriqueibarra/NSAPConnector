using System;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    public class SapTransaction : RfcTransaction
    {
        private readonly SapConnection _connection;

        public SapTransaction(SapConnection connection)
        {
            _connection = connection;
        }

        public void Commit()
        {
            try
            {
                base.Commit(_connection.Destination);
            }
            catch(Exception ex)
            {
                throw new NSAPConnectorException("An error occurred when trying to commit current transaction. Check inner exception for details.", ex);
            }
            finally
            {
                //remove transaction reference for avoiding repeated commit on connection dispose
                _connection.CurrentTransaction = null;
            }
        }

        public void Rollback()
        {
            try
            {
                //RfcTransaction.Commit is taking care of the rollback operation too
                base.Commit(_connection.Destination);
            }
            catch (Exception ex)
            {
                throw new NSAPConnectorException("An error occurred when trying to Rollback current transaction. Check inner exception for details.", ex);
            }
            finally
            {
                //remove transaction reference for avoiding repeated commit on connection dispose
                _connection.CurrentTransaction = null;
            }
        }
    }
}
