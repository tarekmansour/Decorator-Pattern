namespace Api.Client.ErrorManagement;

public class ProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public string? TraceId { get; set; }
    public int Status { get; set; }
    public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>();
}
