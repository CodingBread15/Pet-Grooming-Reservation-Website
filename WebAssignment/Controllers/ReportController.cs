using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAssignment.Controllers;

public class ReportController(DB db) : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        return View();
    }

    //Booking Report
    [Authorize(Roles = "Admin")]
    public IActionResult ReportBookings(DateOnly? date)
    {
        var rb = db.Appointments.AsQueryable();

        if (date.HasValue)
        {
            var filterDate = date.Value;

            rb = rb.Where(a => a.AppointmentDate == filterDate);
        }

        ReportBookingsVM vm = new()
        {
            TotalCompleted = rb.Where(a => a.Status == "Completed").Count(),
            TotalPending = rb.Where(a => a.Status == "Pending").Count(),
            TotalCancelled = rb.Where(a => a.Status == "Cancelled").Count(),
            TotalAppointment = rb.Count(),

            ReportBookingDetails = rb.Select(a => new ReportBookingDetail
            {
                Id = a.Id,
                Date = a.AppointmentDate,
                Time = a.AppointmentTime,
                Status = a.Status,
                PetName = a.Pet.PetName,
                OwnerName = a.Pet.User.Name,
            }).ToList(),
        };

        if (Request.IsAjax())
        {
            return PartialView("_ReportBookingsData", vm);
        }

        return View(vm);
    }

    //Service Report
    [Authorize(Roles = "Admin")]
    public IActionResult ReportServices()
    {
        var s = db.Services.OrderByDescending(s => s.Appointments.Count()).Take(3).ToList();

        return View(s);
    }
}
