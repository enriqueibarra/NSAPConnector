using System;
using SAP.Middleware.Connector;
using System.Diagnostics;

namespace NSAPConnector
{
    public class SapSession: IDisposable
    {
        private readonly SapConnection _connection;

        public ISessionProvider SessionProvider { get; set; }

        public SapSession(SapConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            _connection = connection;
        }

        public SapSession(SapConnection connection, ISessionProvider sessionProvider) : this(connection)
        {
            SessionProvider = sessionProvider;
        }

        public void StartSession()
        {
            if (SessionProvider != null)
            {
                RfcSessionManager.RegisterSessionProvider(SessionProvider);
            }

            RfcSessionManager.BeginContext(_connection.Destination);
        }

        public void EndSession()
        {
            if (SessionProvider != null)
            {
                RfcSessionManager.UnregisterSessionProvider(SessionProvider);
                SessionProvider = null;
            }

            RfcSessionManager.EndContext(_connection.Destination);
        }

        public void Dispose()
        {
            if (SessionProvider == null)
            {
                return;
            }

            try
            {
                RfcSessionManager.UnregisterSessionProvider(SessionProvider);
            }
            catch(Exception ex)
            {
                Trace.TraceError("An error occurred when trying to unregister sessionProvider of type '{0}'.Exception: {1}", SessionProvider.GetType().Name, ex.ToString());
            }
        }
    }
}
