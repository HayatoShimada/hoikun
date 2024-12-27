using Microsoft.EntityFrameworkCore;

namespace hoikun.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserAppointment> UserAppointments { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string AADB2CUserId { get; set; }
        public string Role { get; set; }
        public string? AdditionalInfo { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserAppointment
    {
        public int UserAppointmentId { get; set; }
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
        public string Role { get; set; }
    }



}
