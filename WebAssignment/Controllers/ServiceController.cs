using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAssignment.Controllers;

public class ServiceController(DB db) : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var s = db.Services;

        return View(s);
    }


    //Service Insert
    [Authorize(Roles = "Admin")]
    public IActionResult Insert()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Insert(ServiceVM vm)
    {
        if (ModelState.IsValid)
        {
            db.Services.Add(new()
            {
                Name = vm.Name,
                Price = vm.Price,
                Description = vm.Description,
            });
            db.SaveChanges();

            return RedirectToAction("Index", "Service");
        }
        return View();
    }


    //Service Update
    [Authorize(Roles = "Admin")]
    public IActionResult Update(string id)
    {
        int serviceId = int.Parse(id);
        var s = db.Services.Find(serviceId);
        if (s == null) return RedirectToAction("Index", "Home");

        ServiceVM vm = new()
        {
            Id = s.Id,
            Name = s.Name, 
            Price = s.Price, 
            Description = s.Description,
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(ServiceVM vm)
    {
        var s = db.Services.Find(vm.Id);
        if (s == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            s.Name = vm.Name;
            s.Price = vm.Price;
            s.Description = vm.Description;
            db.SaveChanges();

            return RedirectToAction("Index", "Service");
        }

        return View(vm);
    }


    //Service Delete
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(string id)
    {
        int serviceId = int.Parse(id);
        var s = db.Services.Find(serviceId);
        if (s == null) return RedirectToAction("Index", "Home");

        db.Services.Remove(s);
        db.SaveChanges();

        return RedirectToAction("Index", "Service");
    }
}
