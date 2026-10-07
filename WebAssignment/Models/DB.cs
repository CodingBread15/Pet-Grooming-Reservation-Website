using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebAssignment.Models;

#nullable disable warnings

public class DB(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}


public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Email { get; set; }

    [MaxLength(255)]
    public string Password { get; set; }

    [MaxLength(20)]
    public string Role { get; set; } = "Member";

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Photo { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }

    // Navigation Property
    public List<Pet> Pets { get; set; } = [];

}


public class Pet
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string PetName { get; set; }

    [MaxLength(100)]
    public string? Breed { get; set; }

    public int Age { get; set; }

    [MaxLength(1)]
    public string Gender { get; set; }

    [Precision(4,2)]
    public decimal Weight { get; set; }

    [MaxLength(500)]
    public string? Photo { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Foreign Key
    public int UserId { get; set; }

    // Navigation Property
    public User User { get; set; }

    public List<Appointment> Appointments { get; set; } = [];
}


public class Service
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [Precision(6, 2)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation Property
    public List<Appointment> Appointments { get; set; } = [];
}


public class Appointment
{
    [Key]
    public int Id { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Foreign Key
    public int PetId { get; set; }

    public int ServiceId { get; set; }

    // Navigation Property
    public Pet Pet { get; set; }

    public Service Service { get; set; }
}