namespace FossPDF.FontSubset;

public class SubsetException : Exception
{
    public SubsetException() { }
    public SubsetException(string message) : base(message) { }
    public SubsetException(string message, Exception inner) : base(message, inner) { }
}

