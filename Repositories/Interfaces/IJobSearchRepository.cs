using DevHub.Models;
using DevHub.ViewModels.Jobs;

namespace DevHub.Repositories.Interfaces;

public interface IJobSearchRepository
{
    Task<(List<JobPost> Items, int TotalCount)> SearchAsync(JobSearchFilterViewModel filter);

    Task<JobPost?> GetByIdAsync(int id);

    Task<List<string>> GetDistinctWorkingModelsAsync();

    Task<List<string>> GetDistinctExperienceLevelsAsync();

    Task<List<(int TechId, string TechName, int JobCount)>> GetTopTechsAsync(int top);

    Task<List<(string Location, int JobCount)>> GetTopLocationsAsync(int top, int? techId = null);

    Task<List<(int CompanyId, string CompanyName, string? LogoUrl, int JobCount)>> GetTopCompaniesAsync(int top, int? techId = null, string? filterLocation = null);
}

