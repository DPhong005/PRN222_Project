using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;
public class JobSearchRepository : IJobSearchRepository
{
    private readonly ItrecruitmentDbContext _context;

    public JobSearchRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<(List<JobPost> Items, int TotalCount)> SearchAsync(JobSearchFilterViewModel filter)
    {
        var query = _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .Include(j => j.Provinces)
            .Where(j => j.Status == "APPROVED");

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var kw = filter.Keyword.Trim();
            query = query.Where(j =>
                j.Title.Contains(kw) ||
                (j.Skill != null && j.Skill.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(filter.WorkingModel))
            query = query.Where(j => j.WorkingModel == filter.WorkingModel);

        if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel))
            query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel);

        if (filter.MinSalary.HasValue && filter.MinSalary.Value > 0)
        {
            var minVal = filter.MinSalary.Value;
            query = query.Where(j => j.SalaryMax == null || j.SalaryMax >= minVal);
        }
        if (filter.MaxSalary.HasValue && filter.MaxSalary.Value > 0)
        {
            var maxVal = filter.MaxSalary.Value;
            query = query.Where(j => j.SalaryMin == null || j.SalaryMin <= maxVal);
        }

        if (filter.TechId.HasValue)
            query = query.Where(j => j.Teches.Any(t => t.TechId == filter.TechId.Value));

        if (!string.IsNullOrWhiteSpace(filter.FilterLocation))
            query = query.Where(j => j.Provinces.Any(p => p.ProvinceName == filter.FilterLocation));

        if (filter.RecruiterId.HasValue)
            query = query.Where(j => j.CompanyId == filter.RecruiterId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<JobPost?> GetByIdAsync(int id)
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .Include(j => j.Provinces)
            .FirstOrDefaultAsync(j => j.JobId == id);
    }

    public async Task<List<string>> GetDistinctWorkingModelsAsync()
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED" && j.WorkingModel != null)
            .Select(j => j.WorkingModel!)
            .Distinct()
            .OrderBy(v => v)
            .ToListAsync();
    }

    public async Task<List<string>> GetDistinctExperienceLevelsAsync()
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED" && j.ExperienceLevel != null)
            .Select(j => j.ExperienceLevel!)
            .Distinct()
            .OrderBy(v => v)
            .ToListAsync();
    }

    public async Task<List<(int TechId, string TechName, int JobCount)>> GetTopTechsAsync(int top)
    {
        return await _context.CommonTechnologies
            .AsNoTracking()
            .Where(t => t.IsActive == true && t.Jobs.Any(j => j.Status == "APPROVED"))
            .Select(t => new
            {
                t.TechId,
                t.TechName,
                JobCount = t.Jobs.Count(j => j.Status == "APPROVED")
            })
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.TechId, x.TechName, x.JobCount))
            .ToListAsync();
    }

    public async Task<List<(string Location, int JobCount)>> GetTopLocationsAsync(int top, int? techId = null)
    {
        return await _context.Provinces
            .AsNoTracking()
            .Select(p => new
            {
                Location = p.ProvinceName,
                JobCount = p.JobPosts.Count(j => j.Status == "APPROVED"
                    && (techId == null || j.Teches.Any(t => t.TechId == techId)))
            })
            .Where(x => x.JobCount > 0)
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.Location, x.JobCount))
            .ToListAsync();
    }

    public async Task<List<(int CompanyId, string CompanyName, string? LogoUrl, int JobCount)>> GetTopCompaniesAsync(int top, int? techId = null, string? filterLocation = null)
    {
        var query = _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED");

        if (techId.HasValue)
            query = query.Where(j => j.Teches.Any(t => t.TechId == techId.Value));

        if (!string.IsNullOrWhiteSpace(filterLocation))
            query = query.Where(j => j.Provinces.Any(p => p.ProvinceName == filterLocation));

        return await query
            .GroupBy(j => new { j.CompanyId, j.Company.CompanyName, j.Company.CompanyLogoUrl })
            .Select(g => new
            {
                g.Key.CompanyId,
                g.Key.CompanyName,
                g.Key.CompanyLogoUrl,
                JobCount = g.Count()
            })
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.CompanyId, x.CompanyName, x.CompanyLogoUrl, x.JobCount))
            .ToListAsync();
    }
}

