namespace PlantBackend.ExceptionHandlers;

public class ErrorResponse
{
    public string Error { get; set; } = "Unknown error occurred";
    public int StatusCode { get; set; }

    public ErrorResponse() { }
    public ErrorResponse(string error, int statusCode)
    {
        Error = string.IsNullOrEmpty(error) ? "An unexpected error occurred" : error;
        StatusCode = statusCode;
    }
}
