using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Contracts.Responses;

public class Response<T>
{
    public bool Successful { get; set; }

    public T? Data { get; set; }

    public IEnumerable<Notification>? Errors { get; set; }

    private Response(bool successful, T? data = default, IEnumerable<Notification>? errors = null)
    {
        Successful = successful;
        Data = data;
        Errors = errors;
    }

    public static Response<T> Success(T data) => new(true, data);

    public static Response<T> Failure(IEnumerable<Notification> errors) =>
        new(false, default, errors);
}
