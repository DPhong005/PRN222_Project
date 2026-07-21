using DevHub.ViewModels.Jobs;

namespace DevHub.Services.Interfaces;

public interface IJobSearchService
{
    Task<JobSearchPageViewModel> SearchJobsAsync(JobSearchFilterViewModel filter);
    Task<JobDetailViewModel?> GetJobDetailAsync(int id);
    /// Data cho mega menu trên header (top 20 kỹ năng/thành phố/công ty).
    Task<object> GetNavMenuDataAsync();
}
