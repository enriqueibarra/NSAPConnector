using System.Collections.Generic;

namespace NSAPConnector
{
    /// <summary>
    /// This class is used for storing
    /// all parameters for a SapCommand.
    /// </summary>
    public class SapParameterCollection : List<SapParameter>
    {
        public void Add(string name, string value)
        {
            base.Add(new SapParameter(name, value));
        }
    }
}
