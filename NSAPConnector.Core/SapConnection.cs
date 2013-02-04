using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    public class SapConnection : IDisposable
    {
        public RfcDestination Destination { get; set; }

        public SapConnection(string destinationConfigName)
        {
            
        }

        public SapConnection(Dictionary<string,string> sapConfigParameters)
        {
            
        }

        public void Open()
        {
           Destination = RfcDestinationManager.GetDestination("DEV");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
