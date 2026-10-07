using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAssignment.Controllers;

public class DashboardController(DB db) : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        //Total number of members
        var mCount = db.Users.Where(u => u.Role == "Member").Count();

        //Total number of Pets
        var pCount = db.Pets.Count();

        //Popular services
        var sPopular = db.Services
                         .Select(s => new
                         {
                             s.Name,
                             AppointmentCount = s.Appointments.Count(),
                         })
                         .OrderByDescending(a => a.AppointmentCount)
                         .FirstOrDefault();

        //Pending Bookings
        var aPendingCount = db.Appointments.Where(a => a.Status == "Pending")
                                           .Count();

        //Today's Bookings
        var aPendingTodayCount = db.Appointments.Where(a => a.Status == "Pending" && a.AppointmentDate == DateOnly.FromDateTime(DateTime.Today))
                                                .Count();

        DashboardAdminVM vm = new()
        {
            MemberCount = mCount,
            PetCount = pCount,
            MostPopularServiceName = sPopular?.Name ?? "no data",
            AppointmentPendingCount = aPendingCount,
            AppointmentPendingTodayCount = aPendingTodayCount,
        };

        return View(vm);
    }
}
