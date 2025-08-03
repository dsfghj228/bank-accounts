namespace bank_accounts.Account.MbResult;

public class MbResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private MbResult(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private MbResult(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static MbResult<T> Success(T value) => new MbResult<T>(value);
    public static MbResult<T> Failure(string error) => new MbResult<T>(error);
}