namespace QuizoDotnet.Application.Results;

public class TResult<T>
{
    public T? Result { get; init; }
    public int ErrorCode { get; init; } = 100;
    public bool HasResult => Result != null;
}