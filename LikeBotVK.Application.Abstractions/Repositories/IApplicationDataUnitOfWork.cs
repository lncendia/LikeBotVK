namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IApplicationDataUnitOfWork
{
    Lazy<IJobDataRepository> JobDataRepository { get; set; }
    Lazy<IUserDataRepository> UserDataRepository { get; set; }
    Lazy<IPaymentDataRepository> PaymentDataRepository { get; set; }
}