using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NSAPConnector.CustomExceptions;

namespace NSAPConnector.Utils
{
    internal class ConfigParser
    {
        private static readonly string _configFile;


        static ConfigParser()
        {
            _configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        internal static Dictionary<string, string> GetDestinationParameters(string destinationName)
        {
            var destParams = new Dictionary<string, string>();
            IEnumerable<XElement> resultElements;
            XElement xDoc;

            try
            {
                //open cofiguration file
                xDoc = XElement.Load(_configFile);
            }
            catch (Exception ex)
            {
                throw new NSAPConnectorException(string.Format("An error occurred when trying to open/read application configuration file '{0}'",_configFile), ex);
            }

            //if no name was provided then take data from first destination configuration section
            if (string.IsNullOrEmpty(destinationName))
            {
                resultElements =
                    xDoc.XPathSelectElements("/SAP.Middleware.Connector/ClientSettings/DestinationConfiguration/destinations/add");

                if (!resultElements.Any())
                {
                    throw new NSAPConnectorException("There is no destination configuration in the config file.");
                }
            }
            else
            {
                resultElements =
                   xDoc.XPathSelectElements(string.Format("/SAP.Middleware.Connector/ClientSettings/DestinationConfiguration/destinations/add[@NAME='{0}']", destinationName));

                if (!resultElements.Any())
                {
                    throw new NSAPConnectorException(string.Format("There is no destination configuration with the name '{0}' in the config file.",destinationName));
                }
            }

            foreach (var attr in resultElements.First().Attributes())
            {
                destParams[attr.Name.LocalName] = attr.Value;
            }

            return destParams;
        }
    }
}
