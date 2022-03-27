using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
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

    public Task<List<Job>> FindAsync(ISpecification<Job> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Jobs.ProjectTo<Job>(_mapper.ConfigurationProvider), specification).ToListAsync();

    public Task<int> CountAsync(ISpecification<Job> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Jobs.ProjectTo<Job>(_mapper.ConfigurationProvider), specification).CountAsync();

    public async Task AddAsync(Job entity)
    {
        var job = AddMap(entity);
        await _context.AddAsync(job);
        await _context.SaveChangesAsync();
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
        if (destination.Publications.SequenceEqual(publications)) return;
        destination.Publications.Clear();
        destination.Publications.AddRange(publications);
    }

    public async Task AddRangeAsync(List<Job> entities)
    {
        var jobs = entities.Select(AddMap).ToList();
        await _context.AddRangeAsync(jobs);
        await _context.SaveChangesAsync();
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

    public Task<Job?> GetAsync(int id) => _context.Jobs.ProjectTo<Job>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(model => model.Id == id);

    private static IMapper GetMapper() =>
        new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<Job, JobModel>().ForMember(j => j.Publications, expression => expression.Ignore());
            expr.CreateMap<JobModel, Job>();
            expr.CreateMap<Publication, PublicationModel>().ReverseMap();
        }));
}