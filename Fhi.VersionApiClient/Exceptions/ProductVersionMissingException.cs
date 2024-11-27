namespace Fhi.VersionApiClient.Exceptions
{
    [Serializable]
    public class ProductVersionMissingException : Exception
    {
        public ProductVersionMissingException() { }
        public ProductVersionMissingException(string message) : base(message) { }
        public ProductVersionMissingException(string message, Exception inner) : base(message, inner) { }
    }
}