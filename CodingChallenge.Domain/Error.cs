namespace CodingChallenge.Domain;

public class Error
{
    public string ExceptionType { get; set; }

    public int HttpStatusCode { get; set; }

    public string Message { get; set; }
}