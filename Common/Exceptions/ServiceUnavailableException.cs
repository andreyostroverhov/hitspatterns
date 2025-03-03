namespace Common.Exceptions;

public class ServiceUnavailableException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ServiceUnavailableException()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ServiceUnavailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
