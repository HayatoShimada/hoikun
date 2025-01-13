using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageCategory> MessageCategories { get; set; }
        public DbSet<MessageRecipients> MessageRecipients { get; set; }
        public DbSet<MessageOptions> MessageOptions { get; set; }
        public DbSet<MessageCategoryOptions> MessageCategoryOptions { get; set; }

        public DbSet<ClassTeacher> ClassTeachers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Children (1:N)
            modelBuilder.Entity<Children>()
                .HasOne(c => c.User)
                .WithMany(u => u.Children)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - EmergencyContact (1:N)
            modelBuilder.Entity<EmergencyContact>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.EmergencyContacts) // ここを修正
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Rout (1:N)
            modelBuilder.Entity<Rout>()
                .HasOne(r => r.User)
                .WithMany(u => u.Routs)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Messages の関係を修正 (ON DELETE CASCADE → NO ACTION)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Message - MessageRecipients (1:N)
            modelBuilder.Entity<MessageRecipients>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.MessageRecipients)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - MessageRecipients (1:N) （重複修正）
            modelBuilder.Entity<MessageRecipients>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageRecipients)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Message - MessageOptions (1:N)
            modelBuilder.Entity<MessageOptions>()
                .HasOne(mo => mo.Message)
                .WithMany(m => m.MessageOptions)
                .HasForeignKey(mo => mo.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // MessageCategory - MessageCategoryOptions (1:N)
            modelBuilder.Entity<MessageCategoryOptions>()
                .HasOne(mco => mco.MessageCategory)
                .WithMany()
                .HasForeignKey(mco => mco.MessageCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ClassTeacher の複合キー設定
            modelBuilder.Entity<ClassTeacher>()
                .HasKey(ct => new { ct.ClassId, ct.UserId });
        }

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

        public virtual ICollection<UserAppointment> UserAppointments { get; } = new List<UserAppointment>();
        public virtual ICollection<Appointment> Appointments { get; } = new List<Appointment>();
        public virtual ICollection<Rout> Routs { get; } = new List<Rout>();
        public virtual ICollection<Photo> Photos { get; } = new List<Photo>();
        public virtual ICollection<Message> Messages { get; } = new List<Message>();
        public virtual ICollection<MessageRecipients> MessageRecipients { get; } = new List<MessageRecipients>();
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; } = new List<EmergencyContact>();
        public virtual ICollection<ClassTeacher> ClassTeachers { get; } = new List<ClassTeacher>(); // 多対多の関係

    }

    public class ClassTeacher
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
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
        public string? Name { get; set; }

        public virtual ICollection<ClassTeacher> ClassTeachers { get; } = new List<ClassTeacher>(); // 多対多の関係
        public virtual ICollection<Children> Children { get; } = new List<Children>();

        [NotMapped] // DB に保存しない
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();
    }


    public partial class EmergencyContact
    {
        public int Id { get; set; }

        // 外部キーを明示的に定義
        public int UserId { get; set; }

        public string? Relation { get; set; }

        public int? Rank { get; set; }

        public string? Type { get; set; }

        public string? Tel { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
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

    public partial class Photo
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? CommuteCategory { get; set; }

        public string? PhotoLocation { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }

        public virtual ICollection<Message> Message { get; } = new List<Message>();

    }

    public partial class Message
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int MessageCategoryId { get; set; } // FK (メッセージカテゴリ)

        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public int? PhotoId { get; set; }

        public DateTime? CreatedAt { get; set; }

        // 外部キーの関連付け
        public virtual User? User { get; set; }
        public virtual Photo? Photo { get; set; }

        // 1つのカテゴリに属する
        public virtual MessageCategory MessageCategory { get; set; } = null!;

        // 1つのメッセージは複数のオプションを持てる
        public virtual ICollection<MessageOptions> MessageOptions { get; set; } = new List<MessageOptions>();

        // 1つのメッセージは複数の受信者を持てる
        public virtual ICollection<MessageRecipients> MessageRecipients { get; set; } = new List<MessageRecipients>();
    }


    public partial class MessageCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // 1つのカテゴリには複数のメッセージが属する
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }


    public partial class MessageRecipients
    {
        public int Id { get; set; }

        public int? MessageId { get; set; }

        public int? UserId { get; set; }

        public DateTime? ReadAt { get; set; }

        public bool IsRead { get; set; } = false;

        public virtual User? User { get; set; }

        public virtual Message? Message { get; set; }

    }

    public class MessageOptions
    {
        public int Id { get; set; }
        public int MessageId { get; set; } // FK (メッセージ)
        public string OptionKey { get; set; } = string.Empty; // 例: "Weather"
        public string OptionValue { get; set; } = string.Empty; // 例: "Sunny"

        // 1つのメッセージに対して複数のオプションを持つ
        public virtual Message Message { get; set; } = null!;
    }


    public class MessageCategoryOptions
    {
        public int Id { get; set; }
        public int MessageCategoryId { get; set; } // FK (メッセージカテゴリ)
        public string OptionKey { get; set; } = string.Empty; // 例: "Weather"

        public virtual MessageCategory MessageCategory { get; set; } = null!;
    }



}