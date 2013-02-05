namespace NSAPConnector
{
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
