using System.Collections.Generic;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    /// <summary>
    /// Implementation of the IDestinationConfiguration
    /// used to add configuration data to the SAP adapter
    /// </summary>
    internal class DestinationConfiguration : IDestinationConfiguration
    {
        private readonly RfcConfigParameters _configParameters;
 
        public DestinationConfiguration(Dictionary<string,string> configParameters)
        {
            _configParameters = new RfcConfigParameters();

            foreach(KeyValuePair<string, string> pair in configParameters)
            {
                _configParameters.Add(pair.Key, pair.Value);
            }
        }

        public RfcConfigParameters GetParameters(string destinationName)
        {
            return _configParameters;
        }

        public bool ChangeEventsSupported()
        {
            return true;
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;
    }
}
