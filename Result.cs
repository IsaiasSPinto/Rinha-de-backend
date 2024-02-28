namespace Rinha_de_backend;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Error? Error { get; set; } = null;

    public Result(bool success, string message, Error error)
    {
        Success = success;
        Message = message;
        Error = error;
    }

    public static Result Ok => new Result(true, string.Empty, new Error(0, ""));
    public static Result Failure(Error error) => new Result(false, "", error);
}

public class Result<T> : Result
{
    public Result(T data, bool success, string message, Error error)
        : base(success, message, error)
    {
        Data = data;
    }

    public T Data { get; set; }

    public static Result<T> Ok(T data) => new Result<T>(data, true, string.Empty, new Error(0, ""));
    public static Result<T> Failure(T data, Error erro) => new Result<T>(data, false, string.Empty, erro);

}

public class Error
{
    public Error(int statusCode, string Message)
    {
        StatusCode = statusCode;
        Message = Message;
    }
    public int StatusCode { get; set; }
    public string Message { get; set; }
}
