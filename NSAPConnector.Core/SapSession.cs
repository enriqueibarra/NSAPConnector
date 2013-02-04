using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;

namespace NSAPConnector
{
    public class SapSession: IDisposable
    {
        public ISessionProvider SessionProvider { get; set; }

        public SapSession(SapConnection connection)
        {
            
        }

        public SapSession(SapConnection connection, ISessionProvider sessionProvider)
        {
            
        }

        public void StartSession()
        {
            
        }

        public void EndSession()
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
