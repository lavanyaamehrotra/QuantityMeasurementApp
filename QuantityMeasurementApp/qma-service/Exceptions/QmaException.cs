namespace QmaService.Exceptions
{
    public class QmaException : Exception
    {
        public QmaException(string message) : base(message) { }
        public QmaException(string message, Exception inner) : base(message, inner) { }
    }
}
