using DevHub.Models;
using DevHub.ViewModels;

namespace DevHub.Services.Interfaces;

public interface IPackageTransactionService
{
    Task<AdminDashboardViewModel> GetAdminDashboardDataAsync(int month, int year);
}
