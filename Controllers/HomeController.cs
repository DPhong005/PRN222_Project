using System.Diagnostics;
using System.Security.Claims;
using DevHub.Data;
using DevHub.Models;
using DevHub.ViewModels;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ItrecruitmentDbContext _context;
        private readonly IBookmarkService _bookmarkService;

        public HomeController(ILogger<HomeController> logger, ItrecruitmentDbContext context, IBookmarkService bookmarkService)
        {
            _logger = logger;
            _context = context;
            _bookmarkService = bookmarkService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredJobs = await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.Position)
                .Include(j => j.Teches)
                .Include(j => j.Provinces)
                .Where(j => j.Status != null && j.Status.ToLower() == "approved")
                .OrderByDescending(j => j.PriorityScore)
                .ThenByDescending(j => j.CreatedAt)
                .Take(6)
                .ToListAsync();

            var featuredCompanies = await _context.Companies
                .Where(c => c.IsVerified == true)
                .Select(c => new FeaturedCompanyViewModel
                {
                    RecruiterId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyLogoUrl = c.CompanyLogoUrl,
                    JobCount = _context.JobPosts.Count(j => j.CompanyId == c.CompanyId && j.Status != null && j.Status.ToLower() == "approved")
                })
                .OrderByDescending(c => c.JobCount)
                .Take(8)
                .ToListAsync();

            var featuredBlogs = await _context.BlogPosts
                .Where(b => b.Status == 1)
                .OrderByDescending(b => b.PublishedAt)
                .Take(5)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedJobs = featuredJobs,
                FeaturedCompanies = featuredCompanies,
                FeaturedBlogs = featuredBlogs
            };

            if (User.Identity?.IsAuthenticated == true && (User.IsInRole("CANDIDATE") || User.IsInRole("Candidate")))
            {
                var candidateIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(candidateIdStr, out int candidateId))
                    viewModel.BookmarkedJobIds = await _bookmarkService.GetBookmarkedJobIdsAsync(candidateId);
            }

            return View(viewModel);
        }

        public IActionResult Employer()
        {
            return View("~/Views/Recruiter/Home/Index.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error403()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Error403(string? reason, string? required, string? actual)
        {
            ViewBag.Reason = reason;
            ViewBag.Required = required;
            ViewBag.Actual = actual;
            return View();
        }
    }
}
