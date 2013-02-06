using System;
using SAP.Middleware.Connector;
using System.Diagnostics;

namespace NSAPConnector
{
    /// <summary>
    /// This class is used for makeing statefull calls.
    /// It is based on the RfcSessionManager.BeginContext/EndContext
    /// functionalities from the SAP connector.
    /// </summary>
    public class SapSession: IDisposable
    {
        private readonly SapConnection _connection;


        /// <summary>
        /// Custom session provider which if set will be used 
        /// for managing current session.
        /// </summary>
        public ISessionProvider SessionProvider { get; set; }

        #region Ctors
        
        public SapSession(SapConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            _connection = connection;
        }

        public SapSession(SapConnection connection, ISessionProvider sessionProvider)
            : this(connection)
        {
            SessionProvider = sessionProvider;
        }

        #endregion
        /// <summary>
        /// Start a new Session/Context.
        /// </summary>
        public void StartSession()
        {
            if (SessionProvider != null)
            {
                RfcSessionManager.RegisterSessionProvider(SessionProvider);
            }

            RfcSessionManager.BeginContext(_connection.Destination);
        }

        /// <summary>
        /// End the current Session/Context.
        /// </summary>
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

            GC.SuppressFinalize(this);
        }
    }
}
