using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebAssignment.Models;

#nullable disable warnings

public class UserVM
{
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(255)]
    [EmailAddress]
    [Remote("CheckEmail", "Users", ErrorMessage = "Email already exists")]
    public string Email { get; set; }

    [StringLength(255, MinimumLength = 5, ErrorMessage = "Password should be between 5-255 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [StringLength(255, MinimumLength = 5, ErrorMessage = "Password should be between 5-255 characters")]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone number should be between 10-11 digits")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = " The {0} field must be digits")]
    public string? Phone { get; set; }
}


public class PetVM
{
    public int Id { get; set; }

    [StringLength(100)]
    [Display(Name = "Pet Name")]
    public string PetName { get; set; }

    [StringLength(100)]
    public string? Breed { get; set; }

    [Range(1,100)]
    public int Age { get; set; }

    [StringLength(1)]
    [RegularExpression(@"[FM]", ErrorMessage = "Invalid {0}")]
    public string Gender { get; set; }

    [Range(0.00, 99.99, ErrorMessage = "Weight can not exceed 99.99")]
    public decimal Weight { get; set; }

    public IFormFile? Photo { get; set; }

    public string? PhotoURL { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    // Foreign Key
    public int UserId { get; set; }
}


public class ServiceVM
{
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [Range(0.00, 9999.99, ErrorMessage = "Price can not exceed 9999.99")]
    public decimal Price { get; set; }

    [StringLength(150)]
    public string? Description { get; set; }
}


public class AppointmentVM
{
    public int Id { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    [StringLength(500)]
    public string? Remarks { get; set; }

    public int PetId { get; set; }

    public string? PetName { get; set; }

    public int ServiceId { get; set; }
}


//User
public class LoginVM
{
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(255, MinimumLength = 5, ErrorMessage = "Password should be between 5-255 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}


public class UpdateProfileVM
{
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone number should be between 10-11 digits")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = " The {0} field must be digits")]
    public string? Phone { get; set; }

    public IFormFile? Photo { get; set; }

    public string? PhotoURL { get; set; }
}


//Dashboard
public class DashboardAdminVM
{
    public int MemberCount { get; set; }

    public int PetCount { get; set; }

    public string MostPopularServiceName { get; set; }

    public int AppointmentPendingCount { get; set; }

    public int AppointmentPendingTodayCount { get; set; }
}


//Report
public class ReportBookingsVM
{
    public int TotalCompleted { get; set; }

    public int TotalPending { get; set; }

    public int TotalCancelled { get; set; }

    public int TotalAppointment { get; set; }

    public List<ReportBookingDetail> ReportBookingDetails { get; set; }
}

public class ReportBookingDetail
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Status { get; set; }

    public string PetName { get; set; }

    public string OwnerName { get; set; }
}