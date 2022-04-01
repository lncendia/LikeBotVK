using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Services;

public class PaymentDataRepository : IPaymentDataRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PaymentData?> GetAsync(string id)
    {
        return _context.PaymentsData.Where(data => data.Id == id)
            .Select(data => new PaymentData(data.UserId, data.Cost, data.PaymentDate)).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdateAsync(PaymentData data)
    {
        var payment = _context.PaymentsData.FirstOrDefault(data1 => data1.Id == data.Id);
        if (payment == null)
        {
            payment = new Models.PaymentData();
            await _context.AddAsync(payment);
        }

        payment.PaymentDate = data.PaymentDate;
        payment.Cost = data.Cost;
        payment.Id = data.Id;
        payment.UserId = data.UserId;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var data = _context.PaymentsData.FirstOrDefault(data => data.Id == id);
        if (data == null) return;
        _context.Remove(data);
        await _context.SaveChangesAsync();
    }

    public Task<List<PaymentData>> GetUserPaymentsAsync(long userId, int? take = null, int? skip = null)
    {
        var query = _context.PaymentsData.Where(p => p.UserId == userId);
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);
        return query.Select(data => new PaymentData(data.UserId, data.Cost, data.PaymentDate)).ToListAsync();
    }
}