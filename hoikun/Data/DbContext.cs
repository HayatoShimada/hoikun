using Microsoft.EntityFrameworkCore;

namespace hoikun.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Children> Childrens { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserAppointment> UserAppointments { get; set; }
        public DbSet<Rout> Routs { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string AADB2CUserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Children> Children { get; } = new List<Children>();

    }

    public class UserAppointment
    {
        public int UserAppointmentId { get; set; }
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
        public string Role { get; set; } = string.Empty;


    }

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int? ClassId { get; set; }
        public int? UserId { get; set; }
        public string Caption { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int Label { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int AppointmentType { get; set; }
        public bool AllDay { get; set; } = false;
        public bool IsAll { get; set; } = false;
        public bool IsTeacher { get; set; } = false;
        public bool IsClass { get; set; } = false;
        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;


    }

    public partial class Children
    {
        public int Id { get; set; }

        public int ClassId { get; set; }

        public int UserId { get; set; }

        public string? Name { get; set; }

        public DateTime? Birthday { get; set; }

        public string? AllergyInfo { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? Rank { get; set; }

        public virtual Class Class { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }

    public partial class Class
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public string? Name { get; set; }

        public virtual ICollection<Children> Children { get; } = new List<Children>();
    }

    public partial class EmergencyContact
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? Relation { get; set; }

        public int? Rank { get; set; }

        public string? Type { get; set; }

        public string? Tel { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }

    public partial class Rout
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? CommuteCategory { get; set; }

        public string? PhotoLocation { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }



}
