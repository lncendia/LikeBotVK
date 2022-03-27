using System.Linq;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Payments.Entities;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public Task<List<Payment>> FindAsync(ISpecification<Payment> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Payments.ProjectTo<Payment>(_mapper.ConfigurationProvider), specification).ToListAsync();

    public Task<int> CountAsync(ISpecification<Payment> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Payments.ProjectTo<Payment>(_mapper.ConfigurationProvider), specification).CountAsync();

    public async Task AddAsync(Payment entity)
    {
        var payment = _mapper.Map<PaymentModel>(entity);
        await _context.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Payment> entities)
    {
        var payments = _mapper.Map<List<PaymentModel>>(entities);
        await _context.AddRangeAsync(payments);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Payment entity)
    {
        var model = _context.Payments.First(x => x.Id == entity.Id);
        _mapper.Map(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Payment> entities)
    {
        var ids = entities.Select(payment => payment.Id);
        var payments = await _context.Payments.Where(payment => ids.Contains(payment.Id)).ToListAsync();
        foreach (var entity in entities)
            _mapper.Map(entity, payments.First(paymentModel => paymentModel.Id == entity.Id));
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Payment entity)
    {
        _context.Remove(_context.Payments.First(payment => payment.Id == entity.Id));
        return _context.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(List<Payment> entities)
    {
        var ids = entities.Select(payment => payment.Id);
        _context.RemoveRange(_context.Payments.Where(payment => ids.Contains(payment.Id)));
        return _context.SaveChangesAsync();
    }

    public Task<Payment?> GetAsync(string id) => _context.Payments.ProjectTo<Payment>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(model => model.Id == id);

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr => { expr.CreateMap<Payment, PaymentModel>().ReverseMap(); }));
    }
}