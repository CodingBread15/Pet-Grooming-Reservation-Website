using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebAssignment.Controllers;

public class UserController(DB db, Helper hp) : Controller
{

    //User SignUp 
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Signup(UserVM vm)
    {
        if (ModelState.IsValid("Email") && db.Users.Any(u => u.Email == vm.Email))
        {
            ModelState.AddModelError("Email", "Email already exists");
        }

        if (ModelState.IsValid)
        {
            db.Users.Add(new()
            {
                Name = vm.Name,
                Email = vm.Email,
                Password = hp.HashPassword(vm.Password),
                Phone = vm.Phone,
            });
            db.SaveChanges();

            return RedirectToAction("Login", "User");
        }

        return View();
    }


    //User Login 
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginVM vm, string? returnUrl)
    {   
        var u = db.Users.FirstOrDefault(u => u.Email == vm.Email);

        if (u == null)
        {
            ModelState.AddModelError("", "Invalid Email or Password");
            return View();
        }

        if (u.LockoutEnd != null && u.LockoutEnd > DateTime.Now)
        {
            ModelState.AddModelError("", "Account temporary disabled (5 minutes)");
            return View();
        }
        else if(u.LockoutEnd != null && u.LockoutEnd <= DateTime.Now)
        {
            u.FailedLoginAttempts = 0;
            u.LockoutEnd = null;
            db.SaveChanges();
        }

        if (vm.Password != null)
        {
            if (!hp.VerifyPassword(u.Password, vm.Password))
            {
                u.FailedLoginAttempts++;

                if (u.FailedLoginAttempts >= 3)
                {
                    u.LockoutEnd = DateTime.Now.AddMinutes(5);
                }

                db.SaveChanges();
                ModelState.AddModelError("", "Invalid Email or Password");
            }
        }
        else
        {
            ModelState.AddModelError("", "Invalid Email or Password");
        }

        if (ModelState.IsValid)
        {
            hp.SignIn(u!.Email, u.Role, vm.RememberMe);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
        }

        return View();
    }


    //User Profile
    [Authorize]
    public IActionResult Profile()
    {
        var m = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        return View(m);
    }

    [Authorize(Roles = "Member")]
    public IActionResult UpdateProfile()
    {
        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");
        UpdateProfileVM vm = new()
        {
            Name = u.Name,
            Phone = u.Phone,
            PhotoURL = u.Photo,
        };
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult UpdateProfile(UpdateProfileVM vm)
    {
        var u = db.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
        if (u == null) return RedirectToAction("Index", "Home");

        if (vm.Photo != null)
        {
            var err = hp.ValidatePhoto(vm.Photo!);
            if (err != "") ModelState.AddModelError("Photo", err);
        }

        if (ModelState.IsValid)
        {
            u.Name = vm.Name;
            u.Phone = vm.Phone;
            if (vm.Photo != null)
            {
                if (!string.IsNullOrEmpty(u.Photo)) hp.DeletePhoto(u.Photo!, "images");
                u.Photo = hp.SavePhoto(vm.Photo, "images");
            }
            db.SaveChanges();
        }

        vm.Name = u.Name;
        vm.Phone = u.Phone;
        vm.PhotoURL = u.Photo;

        return View(vm);
    }

    public IActionResult Logout(string? returnUrl)
    {
        hp.SignOut();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied(string? returnUrl)
    {
        return View();
    }


    //User Maintence(Admin)
    [Authorize(Roles = "Admin")]
    public IActionResult MemberIndex(string? email)
    {
        var u = db.Users.AsQueryable();

        if (string.IsNullOrEmpty(email))
        {
            u = u.Where(u => u.Role == "Member");
        }
        else
        {
            u = u.Where(u => u.Role == "Member" && u.Email.Contains(email));
        }

        var users = u.ToList();

        if (Request.IsAjax())
        {
            return PartialView("_MemberIndexData", users);
        }

        return View(users);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult UnblockLogin(string id)
    {
        int userId = int.Parse(id);

        var u = db.Users.FirstOrDefault(u => u.Id == userId);
        if (u == null) return RedirectToAction("Index", "Home");

        u.FailedLoginAttempts = 0;
        u.LockoutEnd = null;
        db.SaveChanges();

        return RedirectToAction("MemberIndex", "User");
    }


    //User Delete
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(string id)
    {
        int userId = int.Parse(id);
        var u = db.Users.Find(userId);
        if (u == null) return RedirectToAction("Index", "Home");

        db.Users.Remove(u);
        db.SaveChanges();

        return RedirectToAction("MemberIndex", "User");
    }


    //Others
    public bool CheckEmail(string email)
    {
        return !db.Users.Any(u => u.Email == email);
    }
}