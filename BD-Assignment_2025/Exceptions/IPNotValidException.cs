namespace BD_Assignment_2025.Exceptions
{
    public sealed class IPNotValidException : Exception
    {
        public IPNotValidException(string message)
            : base(message) { }
    }
}
