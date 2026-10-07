using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace WebAssignment.Controllers;

public class PetController(DB db, Helper hp) : Controller
{
    [Authorize(Roles = "Member")]
    public IActionResult Index()
    {
        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        var p = db.Pets.Where(p => p.UserId == u.Id);

        return View(p);
    }


    //Pet Insert
    [Authorize(Roles = "Member")]
    public IActionResult Insert()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult Insert(PetVM vm)
    {
        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        if (vm.Photo != null)
        {
            var err = hp.ValidatePhoto(vm.Photo);
            if (err != "") ModelState.AddModelError("Photo", err);
        }

        if (ModelState.IsValid)
        {   
            if (vm.Photo != null)
            {
                vm.PhotoURL = hp.SavePhoto(vm.Photo, "images");
            }

            db.Pets.Add(new()
            {
                PetName = vm.PetName,
                Breed = vm.Breed,
                Age = vm.Age,
                Gender = vm.Gender,
                Weight = vm.Weight,
                Photo = vm.PhotoURL,
                Notes = vm.Notes,
                UserId  = u.Id,
            });
            db.SaveChanges();

            return RedirectToAction("Index", "Pet");
        }
        return View();
    }


    //Pet Update
    [Authorize(Roles = "Member")]
    public IActionResult Detail(string id)
    {   
        int petId = int.Parse(id);

        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        var p = db.Pets.Where(p => p.Id == petId && p.UserId == u!.Id).FirstOrDefault();
        if (p == null) return RedirectToAction("Index", "Home");

        return View(p);
    }

    [Authorize(Roles = "Member")]
    public IActionResult Update(string id)
    {
        int petId = int.Parse(id);

        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        var p = db.Pets.Where(p => p.Id == petId && p.UserId == u!.Id).FirstOrDefault();
        if (p == null) return RedirectToAction("Index", "Home");

        PetVM vm = new()
        {
            Id = petId,
            PetName = p.PetName,
            Breed = p.Breed,
            Age = p.Age,
            Gender = p.Gender,
            Weight = p.Weight,
            PhotoURL = p.Photo,
            Notes = p.Notes,
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult Update(PetVM vm)
    {
        var p = db.Pets.FirstOrDefault(p => p.Id == vm.Id);
        if (p == null) return RedirectToAction("Index", "Home");

        if (vm.Photo != null)
        {
            var err = hp.ValidatePhoto(vm.Photo);
            if (err != "") ModelState.AddModelError("Photo", err);
        }

        if (ModelState.IsValid)
        {
            p.PetName = vm.PetName;
            p.Breed = vm.Breed;
            p.Age = vm.Age;
            p.Gender = vm.Gender;
            p.Weight = vm.Weight;
            if (vm.Photo != null)
            {
                if (!string.IsNullOrEmpty(p.Photo)) hp.DeletePhoto(p.Photo, "images");
                p.Photo = hp.SavePhoto(vm.Photo, "images");
            }
            p.Notes = vm.Notes;
            db.SaveChanges();
            return RedirectToAction("Index", "Pet");
        }
        
        vm.PetName = p.PetName;
        vm.Breed = p.Breed;
        vm.Age = p.Age;
        vm.Gender = p.Gender;
        vm.Weight = p.Weight;
        vm.PhotoURL = p.Photo;
        vm.Notes = p.Notes;

        return View(vm);
    }


    //Pet Delete
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult Delete(string id)
    {
        int petId = int.Parse(id);

        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        var p = db.Pets.Where(p => p.Id == petId && p.UserId == u!.Id).FirstOrDefault();
        if (p == null) return RedirectToAction("Index", "Home");


        if (p != null)
        {
            if (!string.IsNullOrEmpty(p.Photo))
            {
                hp.DeletePhoto(p.Photo!, "images");
            }

            db.Pets.Remove(p);
            db.SaveChanges();
        }

        return RedirectToAction("Index", "Pet");
    }
}
