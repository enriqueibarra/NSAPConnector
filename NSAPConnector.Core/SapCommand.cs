using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSAPConnector
{
    public class SapCommand
    {
        public SapConnection Connection { get; set; }

        public SapCommand()
        {
            
        }

        public SapCommand(string cmdText)
        {
            
        }

        public SapCommand(string cmdText, SapConnection connection)
        {
            
        }

        public SapCommand(string cmdText, SapConnection connection, SapTransaction transaction)
        {
            
        }
    }
}
