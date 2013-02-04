using System;

namespace NSAPConnector.CustomExceptions
{
    public class NSAPConnectorException : ApplicationException
    {
        public NSAPConnectorException(string message) : base(message)
        {
            
        }

        public NSAPConnectorException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
