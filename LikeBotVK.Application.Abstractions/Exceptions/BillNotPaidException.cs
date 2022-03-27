namespace LikeBotVK.Application.Abstractions.Exceptions;

public class BillNotPaidException : Exception
{
    public BillNotPaidException(string billId) : base($"Bill {billId} has not been paid.")
    {
    }
}