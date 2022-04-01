using AutoMapper;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.EqualityComparers;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using LikeBotVK.Infrastructure.PersistentStorage.Visitors;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddAsync(Job entity)
    {
        var job = AddMap(entity);
        await _context.AddAsync(job);
        await _context.SaveChangesAsync();
        entity.Id = job.Id;
    }

    private JobModel AddMap(Job entity)
    {
        var job = _mapper.Map<JobModel>(entity);
        job.Publications = _mapper.Map<List<Publication>, List<PublicationModel>>(entity.Publications);
        return job;
    }

    private void UpdateMap(Job entity, JobModel destination)
    {
        _mapper.Map(entity, destination);
        var publications = _mapper.Map<List<Publication>, List<PublicationModel>>(entity.Publications);
        if (destination.Publications.SequenceEqual(publications, new PublicationEqualityComparer())) return;
        destination.Publications.Clear();
        destination.Publications.AddRange(publications);
    }

    public async Task AddRangeAsync(List<Job> entities)
    {
        var jobs = entities.Select(AddMap).ToList();
        await _context.AddRangeAsync(jobs);
        await _context.SaveChangesAsync();
        for (int i = 0; i < entities.Count; i++) entities[i].Id = jobs[i].Id;
    }

    public Task UpdateAsync(Job entity)
    {
        var model = _context.Jobs.Include(jobModel => jobModel.Publications).First(x => x.Id == entity.Id);
        UpdateMap(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Job> entities)
    {
        var ids = entities.Select(job => job.Id);
        var jobs = await _context.Jobs.Include(model => model.Publications).Where(job => ids.Contains(job.Id))
            .ToListAsync();
        foreach (var entity in entities)
            UpdateMap(entity, jobs.First(jobModel => jobModel.Id == entity.Id));
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Job entity)
    {
        _context.Remove(_context.Jobs.First(job => job.Id == entity.Id));
        return _context.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(List<Job> entities)
    {
        var ids = entities.Select(job => job.Id);
        _context.RemoveRange(_context.Jobs.Where(job => ids.Contains(job.Id)));
        return _context.SaveChangesAsync();
    }

    public async Task<Job?> GetAsync(int id)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(model => model.Id == id);
        return job == null ? null : _mapper.Map<JobModel, Job>(job);
    }

    public async Task<List<Job>> FindAsync(ISpecification<Job, IJobSpecificationVisitor>? specification,
        int? skip = null,
        int? take = null)
    {
        var query = _context.Jobs.AsQueryable();
        if (specification != null)
        {
            var visitor = new JobVisitor();
            specification.Accept(visitor);
            if (visitor.Expr != null) query = query.Where(visitor.Expr);
        }

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return _mapper.Map<List<JobModel>, List<Job>>(await query.ToListAsync());
    }

    public Task<int> CountAsync(ISpecification<Job, IJobSpecificationVisitor>? specification)
    {
        var query = _context.Jobs.AsQueryable();
        if (specification == null) return query.CountAsync();
        var visitor = new JobVisitor();
        specification.Accept(visitor);
        if (visitor.Expr != null) query = query.Where(visitor.Expr);

        return query.CountAsync();
    }

    private static IMapper GetMapper() =>
        new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<Job, JobModel>().ForMember(j => j.Publications, expression => expression.Ignore());
            expr.CreateMap<JobModel, Job>();
            expr.CreateMap<Publication, PublicationModel>().ReverseMap();
        }));
}