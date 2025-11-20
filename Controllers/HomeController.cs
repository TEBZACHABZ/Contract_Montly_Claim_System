using System.Diagnostics;
using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();

        // ------------------ REGISTER ------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Registration successful!";
                return RedirectToAction("LecturerDashboard");
            }
            return View(lecturer);
        }

        // ------------------ LECTURER DASHBOARD ------------------
        [HttpGet]
        public async Task<IActionResult> LecturerDashboard()
        {
            var lecturerClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();

            return View(lecturerClaims);
        }

        // ------------------ SUBMIT CLAIM ------------------
        [HttpGet]
        public IActionResult SubmitClaim() => View();

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Claim submitted successfully!";
                return RedirectToAction("TrackClaims");
            }

            return View(claim);
        }

        // ------------------ TRACK CLAIMS ------------------
        public async Task<IActionResult> TrackClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .ToListAsync();

            return View(claims);
        }

        // ------------------ COORDINATOR DASHBOARD ------------------
        public async Task<IActionResult> CoordinatorDashboard()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Pending)
                .Include(c => c.Lecturer)
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Verified;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CoordinatorDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CoordinatorDashboard");
        }

        // ------------------ MANAGER DASHBOARD ------------------
        public async Task<IActionResult> ManagerDashboard()
        {
            var verifiedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Verified)
                .Include(c => c.Lecturer)
                .ToListAsync();

            return View(verifiedClaims);
        }

        [HttpGet]
        public async Task<IActionResult> ManagerApproval()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Verified || c.Status == ClaimStatus.Approved)
                .Include(c => c.Lecturer)
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> FinalApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Approved;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManagerApproval");
        }

        // ------------------ HR DASHBOARD ------------------
        [HttpGet]
        public IActionResult HrDashboard() => View();

        [HttpGet]
        public IActionResult GenerateHrReport()
        {
            var approvedClaims = _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Approved)
                .Select(c => new
                {
                    claimId = c.ClaimId,
                    lecturer = c.Lecturer.FullName,
                    c.HoursWorked,
                    c.HourlyRate,
                    c.TotalAmount,
                    c.SubmittedAt
                })
                .ToList();

            return Json(approvedClaims);
        }

        // ------------------ SYSTEM ------------------
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

