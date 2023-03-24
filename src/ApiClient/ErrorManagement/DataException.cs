namespace Api.Client.ErrorManagement;

[Serializable]
public class DataException: Exception
{
    public int StatusCode { get; }
    public ProblemDetails? Problem { get; }

    public DataException()
    {}

    public DataException(
        string message,
        int statusCode,
        Exception? innerException = default)
        : base(message, innerException)
        => StatusCode = statusCode;

    public DataException(
        ProblemDetails problem,
        Exception? innerException = default)
        : base(problem.Title, innerException)
    {
        StatusCode = problem.Status;
        Problem = problem;
    }
}
