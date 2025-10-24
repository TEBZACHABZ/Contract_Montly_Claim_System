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
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public IActionResult Index() => View();

        // --- Registration ---
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

        // --- Lecturer Dashboard ---
        [HttpGet]
        public async Task<IActionResult> LecturerDashboard()
        {
            var lecturerClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();
            return View(lecturerClaims);
        }

        // --- Claim submission ---
        [HttpGet]
        public IActionResult SubmitClaim() => View();

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile? SupportingDocument)
        {
            if (ModelState.IsValid)
            {
                claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                // Handle file upload
                if (SupportingDocument != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, SupportingDocument.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await SupportingDocument.CopyToAsync(stream);
                    }

                    var document = new ClaimDocument
                    {
                        ClaimId = claim.ClaimId,
                        FileName = SupportingDocument.FileName,
                        FilePath = "/uploads/" + SupportingDocument.FileName
                    };
                    _context.ClaimDocuments.Add(document);
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Claim submitted successfully!";
                return RedirectToAction("TrackClaims");
            }

            return View(claim);
        }

        // --- Track claims ---
        public async Task<IActionResult> TrackClaims()
        {
            var claims = await _context.Claims.Include(c => c.Lecturer).ToListAsync();
            return View(claims);
        }

        // --- Coordinator actions ---
        public async Task<IActionResult> CoordinatorDashboard()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Pending)
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

        // --- Manager Dashboard ---
        public async Task<IActionResult> ManagerDashboard()
        {
            var verifiedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Verified)
                .ToListAsync();
            return View(verifiedClaims);
        }

        // --- Manager Approval Page ---
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

        // --- JSON API for JS Refresh ---
        [HttpGet]
        public IActionResult GetClaimsStatus()
        {
            var claims = _context.Claims
                .Include(c => c.Lecturer)
                .Select(c => new
                {
                    c.ClaimId,
                    LecturerName = c.Lecturer != null ? c.Lecturer.FullName : string.Empty,
                    c.HoursWorked,
                    c.HourlyRate,
                    c.Status,
                    c.SubmittedAt
                })
                .ToList();

            return Json(claims);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
