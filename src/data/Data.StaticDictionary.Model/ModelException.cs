namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model
{
    public sealed class ModelException : Exception
    {
        public ModelException()
            : base()
        {
        }

        public ModelException(string message)
            : base(message)
        {
        }

        public ModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}