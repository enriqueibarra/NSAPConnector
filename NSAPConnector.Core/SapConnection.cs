using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;
using NSAPConnector.Utils;

namespace NSAPConnector
{
    public class SapConnection : IDisposable
    {
        private readonly IDestinationConfiguration _destinationConfiguration;

        public RfcDestination Destination { get; set; }

        public SapConnection(string destinationConfigName) 
            : this(ConfigParser.GetDestinationParameters(destinationConfigName))
        {
            
        }

        public SapConnection(Dictionary<string,string> sapConfigParameters)
        {
            _destinationConfiguration = new DestinationConfiguration(sapConfigParameters);
        }

        public void Open()
        {
           Destination = RfcDestinationManager.GetDestination("DEV");
        }

        public void Dispose()
        {
            if (_destinationConfiguration != null)
            {
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
