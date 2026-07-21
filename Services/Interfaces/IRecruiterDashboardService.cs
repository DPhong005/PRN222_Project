using DevHub.Models;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces;

public interface IRecruiterDashboardService
{
    // Assembles the full recruiter dashboard (totals, package, profile, charts).
    Task<RecruiterDashboard> GetDashboardAsync(
        int companyId, 
        int recruiterId, 
        string? jobStatus, 
        string? jobQ, 
        string? jobSort, 
        int jobPage = 1);
}
