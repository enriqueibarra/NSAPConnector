using System;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    /// <summary>
    /// This class provides basic means for
    /// being able to make transactional rfc calls to SAP 
    /// </summary>
    public class SapTransaction : RfcTransaction
    {
        private readonly SapConnection _connection;

        public SapTransaction(SapConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Commits current transaction and unbinds it from
        /// the corresponding connection.
        /// </summary>
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

        /// <summary>
        /// Rollback current transaction and unbinds it from
        /// the corresponding connection.
        /// </summary>
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
