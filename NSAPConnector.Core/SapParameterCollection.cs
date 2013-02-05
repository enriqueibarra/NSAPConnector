using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSAPConnector
{
    public class SapParameterCollection : List<SapParameter>
    {
        public void Add(string name, string value)
        {
            base.Add(new SapParameter(name, value));
        }
    }
}
