namespace FPPR_System.Common.Models;

public class ServiceResult
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    private ServiceResult(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult Success() => new(true, string.Empty);

    public static ServiceResult Failure(string errorMessage) => new(false, errorMessage);
}
