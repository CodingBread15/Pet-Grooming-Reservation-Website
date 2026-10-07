using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAssignment.Controllers;

public class AppointmentController(DB db) : Controller
{
    // MEMBER LIST
    [Authorize(Roles = "Member")]
    public IActionResult Index()
    {
        var user = db.Users
            .FirstOrDefault(u => u.Email == User.Identity!.Name);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var appointments = db.Appointments
            .Where(a => a.Pet.UserId == user.Id)
            .Include(a => a.Pet)
            .Include(a => a.Service)
            .ToList();

        return View(appointments);
    }


    // CREATE
    [Authorize(Roles = "Member")]
    public IActionResult Create()
    {
        ViewBag.Services = db.Services.ToList();

        return View();
    }


    [HttpPost]
    [Authorize(Roles = "Member")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AppointmentVM vm)
    {
        var user = db.Users
            .FirstOrDefault(u => u.Email == User.Identity!.Name);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var pet = db.Pets.FirstOrDefault(p =>
            p.PetName == vm.PetName &&
            p.UserId == user.Id);

        if (pet == null)
        {
            ModelState.AddModelError(
                "PetName",
                "Pet not found."
            );
        }

        var service = db.Services
            .FirstOrDefault(s => s.Id == vm.ServiceId);

        if (service == null)
        {
            ModelState.AddModelError(
                "ServiceId",
                "Please select a service."
            );
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Services = db.Services.ToList();

            return View(vm);
        }

        var appointment = new Appointment
        {
            AppointmentDate = vm.AppointmentDate,
            AppointmentTime = vm.AppointmentTime,
            Status = "Pending",
            Remarks = vm.Remarks,
            PetId = pet!.Id,
            ServiceId = vm.ServiceId
        };

        db.Appointments.Add(appointment);

        db.SaveChanges();

        TempData["Info"] = "Appointment booked successfully.";

        return RedirectToAction("Index");
    }


    // UPDATE
    [Authorize(Roles = "Member")]
    public IActionResult Update(int id)
    {
        var user = db.Users
            .FirstOrDefault(u => u.Email == User.Identity!.Name);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var appointment = db.Appointments
            .Include(a => a.Pet)
            .FirstOrDefault(a =>
                a.Id == id &&
                a.Pet.UserId == user.Id);

        if (appointment == null)
            return RedirectToAction("Index");

        ViewBag.Services = db.Services.ToList();

        var vm = new AppointmentVM
        {
            Id = appointment.Id,
            AppointmentDate = appointment.AppointmentDate,
            AppointmentTime = appointment.AppointmentTime,
            Status = appointment.Status,
            Remarks = appointment.Remarks,
            PetId = appointment.PetId,
            PetName = appointment.Pet.PetName,
            ServiceId = appointment.ServiceId
        };

        return View(vm);
    }


    [HttpPost]
    [Authorize(Roles = "Member")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(AppointmentVM vm)
    {
        var user = db.Users
            .FirstOrDefault(u => u.Email == User.Identity!.Name);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var appointment = db.Appointments
            .FirstOrDefault(a =>
                a.Id == vm.Id &&
                a.Pet.UserId == user.Id);

        if (appointment == null)
            return RedirectToAction("Index");

        var pet = db.Pets.FirstOrDefault(p =>
            p.PetName == vm.PetName &&
            p.UserId == user.Id);

        if (pet == null)
        {
            ModelState.AddModelError(
                "PetName",
                "Pet not found."
            );
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Services = db.Services.ToList();

            return View(vm);
        }

        appointment.AppointmentDate = vm.AppointmentDate;
        appointment.AppointmentTime = vm.AppointmentTime;
        appointment.Remarks = vm.Remarks;
        appointment.PetId = pet!.Id;
        appointment.ServiceId = vm.ServiceId;

        db.SaveChanges();

        TempData["Info"] = "Appointment updated successfully.";

        return RedirectToAction("Index");
    }


    // DELETE
    [HttpPost]
    [Authorize(Roles = "Member")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var user = db.Users
            .FirstOrDefault(u => u.Email == User.Identity!.Name);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var appointment = db.Appointments
            .FirstOrDefault(a =>
                a.Id == id &&
                a.Pet.UserId == user.Id);

        if (appointment != null)
        {
            db.Appointments.Remove(appointment);

            db.SaveChanges();

            TempData["Info"] = "Appointment deleted successfully.";
        }

        return RedirectToAction("Index");
    }


    // =========================
    // ADMIN MEMBER LIST
    // =========================

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        var appointments = db.Appointments
            .Include(a => a.Pet)
            .ThenInclude(p => p.User)
            .Include(a => a.Service)
            .ToList();

        return View(appointments);
    }


    // =========================
    // ADMIN CALENDAR
    // =========================

    [Authorize(Roles = "Admin")]
    public IActionResult Calendar()
    {
        var appointments = db.Appointments
            .Include(a => a.Pet)
            .ThenInclude(p => p.User)
            .Include(a => a.Service)
            .ToList();

        return View(appointments);
    }


    // =========================
    // ADMIN UPDATE STATUS
    // =========================

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(int id, string status)
    {
        var appointment = db.Appointments
            .FirstOrDefault(a => a.Id == id);

        if (appointment == null)
            return RedirectToAction("Admin");

        appointment.Status = status;

        db.SaveChanges();

        return RedirectToAction("Admin");
    }


    // =========================
    // ADMIN - AJAX SORTING
    // =========================

    [Authorize(Roles = "Admin")]
    public IActionResult SortBookings(string sort, bool descending)
    {
        var appointments = db.Appointments
            .Include(a => a.Pet)
            .ThenInclude(p => p.User)
            .Include(a => a.Service)
            .ToList();


        if (sort == "member")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.Pet.User.Name).ToList()
                : appointments.OrderBy(a => a.Pet.User.Name).ToList();
        }


        if (sort == "pet")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.Pet.PetName).ToList()
                : appointments.OrderBy(a => a.Pet.PetName).ToList();
        }


        if (sort == "service")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.Service.Name).ToList()
                : appointments.OrderBy(a => a.Service.Name).ToList();
        }


        if (sort == "date")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.AppointmentDate).ToList()
                : appointments.OrderBy(a => a.AppointmentDate).ToList();
        }


        if (sort == "time")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.AppointmentTime).ToList()
                : appointments.OrderBy(a => a.AppointmentTime).ToList();
        }


        if (sort == "status")
        {
            appointments = descending
                ? appointments.OrderByDescending(a => a.Status).ToList()
                : appointments.OrderBy(a => a.Status).ToList();
        }


        return PartialView("_BookingTable", appointments);
    }
}

