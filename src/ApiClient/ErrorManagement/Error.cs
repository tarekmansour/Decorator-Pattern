namespace Api.Client.ErrorManagement;

public class Error
{
    public string Code { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Message { get; set; } = string.Empty;

    public Error(string code, string? name = null, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Invalid code, it's empty or null", nameof(code));
        }

        Code = code;
        Name = name;
        Message = message;
    }
}
