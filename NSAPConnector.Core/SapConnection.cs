using System;
using System.Collections.Generic;
using System.Diagnostics;
using NSAPConnector.CustomExceptions;
using SAP.Middleware.Connector;
using NSAPConnector.Utils;

namespace NSAPConnector
{
    public class SapConnection : IDisposable
    {
        private readonly IDestinationConfiguration _destinationConfiguration;
        private readonly string _destinationConfigName;

        internal SapTransaction CurrentTransaction { get; set; }

        public RfcDestination Destination { get; set; }

        #region Ctor
        
        public SapConnection(string destinationConfigName)
            : this(ConfigParser.GetDestinationParameters(destinationConfigName))
        {
        }

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

        public SapTransaction BeginTransaction()
        {
            CurrentTransaction = new SapTransaction(this);

            return CurrentTransaction;
        }

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
