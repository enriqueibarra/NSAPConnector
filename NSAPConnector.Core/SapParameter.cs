namespace NSAPConnector
{
    /// <summary>
    /// This class is used for 
    /// storing RFC parameter information.
    /// </summary>
    public class SapParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public SapParameter()
        {
            
        }

        public SapParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
