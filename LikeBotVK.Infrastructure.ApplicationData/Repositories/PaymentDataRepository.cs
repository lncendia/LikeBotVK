using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Repositories;

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
            .Select(data => new PaymentData(data.Id, data.UserId, data.Cost, data.PaymentDate)).FirstOrDefaultAsync();
    }

    public Task<List<PaymentData>> FindAsync(int? skip, int? take)
    {
        var query = _context.PaymentsData.AsQueryable();
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);
        return query.Select(data => new PaymentData(data.Id, data.UserId, data.Cost, data.PaymentDate)).ToListAsync();
    }

    public Task<int> CountAsync() => _context.PaymentsData.CountAsync();

    public async Task AddOrUpdateAsync(PaymentData data)
    {
        var payment = _context.PaymentsData.FirstOrDefault(data1 => data1.Id == data.Id);
        if (payment == null)
        {
            payment = new Models.PaymentData {Id = string.Empty};
            await _context.AddAsync(payment);
        }

        payment.PaymentDate = data.PaymentDate;
        payment.Cost = data.Cost;
        payment.UserId = data.UserId;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PaymentData data)
    {
        _context.Remove(_context.PaymentsData.First(data1 => data1.Id == data.Id));
        await _context.SaveChangesAsync();
    }

    public Task<List<PaymentData>> GetUserPaymentsAsync(long userId, int? take = null, int? skip = null)
    {
        var query = _context.PaymentsData.Where(p => p.UserId == userId);
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);
        return query.Select(data => new PaymentData(data.Id, data.UserId, data.Cost, data.PaymentDate)).ToListAsync();
    }
}