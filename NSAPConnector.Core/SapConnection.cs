using System;
using System.Collections.Generic;
using System.Diagnostics;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;
using NSAPConnector.Utils;

namespace NSAPConnector
{
    /// <summary>
    /// This class is used for establishing
    /// a connection to the SAP based on the 
    /// provided configuration parameters
    /// </summary>
    public class SapConnection : IDisposable
    {
        private readonly IDestinationConfiguration _destinationConfiguration;
        private readonly string _destinationConfigName;

        /// <summary>
        /// This property is used only inside NSAPConnector library
        /// for being able to work transactionally
        /// </summary>
        internal SapTransaction CurrentTransaction { get; set; }

        /// <summary>
        /// This property is populated on connection opening
        /// with the RfcDestination which corresponds to the 
        /// provided configuration parameters
        /// </summary>
        public RfcDestination Destination { get; set; }

        #region Ctor
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationConfigName">Name of the destination from the application configuration file.</param>
        public SapConnection(string destinationConfigName)
            : this(ConfigParser.GetDestinationParameters(destinationConfigName))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sapConfigParameters">A dictionary with parameters (key = paramater name, value = parameter value)</param>
        public SapConnection(Dictionary<string, string> sapConfigParameters)
        {
            _destinationConfigName = sapConfigParameters.ContainsKey("NAME")
                                         ? sapConfigParameters["NAME"]
                                         : "DummyDestinationConfigName";

            _destinationConfiguration = new DestinationConfiguration(sapConfigParameters);

            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(_destinationConfiguration);
            }
            catch (Exception ex)
            {
                throw new NSAPConnectorException("An exception occurred when trying to register configuration parameters. Check inner exception for details.", ex);
            }
        } 

        #endregion

        /// <summary>
        /// Open connection.
        /// A new RfcDestination is created.
        /// </summary>
        public void Open()
        {
            try
            {
                Destination = RfcDestinationManager.GetDestination(_destinationConfigName);
            }
            catch(Exception ex)
            {
                throw new NSAPConnectorException("An exception occurred when trying to open a SapConnection. Check inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Open a new transaction which is bind
        /// to the current connection
        /// </summary>
        /// <returns></returns>
        public SapTransaction BeginTransaction()
        {
            CurrentTransaction = new SapTransaction(this);

            return CurrentTransaction;
        }

        /// <summary>
        /// Commit/Rollback existing transaction.
        /// Unregister current destination configuration.
        /// </summary>
        public void Dispose()
        {
            if (_destinationConfiguration != null)
            {
                if (CurrentTransaction != null)
                {
                    try
                    {
                        CurrentTransaction.Commit();
                        
                        CurrentTransaction = null;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(
                            "An error occurred when trying to commit current open transaction. Exception: {0}", ex);
                    }
                }

                try
                {
                    RfcDestinationManager.UnregisterDestinationConfiguration(_destinationConfiguration);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("An error occurred when trying to unregister destination configuration. Exception: {0}", ex);
                }
            }
        }
    }
}
