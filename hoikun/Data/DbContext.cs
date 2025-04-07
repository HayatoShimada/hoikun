using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace hoikun.Data
{
    /// <summary>
    /// メインの DbContext。
    /// エンティティと Fluent API 設定を行う。
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // -- DbSet 定義 --
        public DbSet<User> Users => Set<User>();
        public DbSet<Children> Childrens => Set<Children>();

        public DbSet<PickupRecord> PickupRecords => Set<PickupRecord>();

        public DbSet<PickupTimeSetting> PickupTimeSettings => Set<PickupTimeSetting>();


        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<UserAppointment> UserAppointments => Set<UserAppointment>();
        public DbSet<Rout> Routs => Set<Rout>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageCategory> MessageCategories => Set<MessageCategory>();
        public DbSet<MessageRecipients> MessageRecipients => Set<MessageRecipients>();
        public DbSet<MessageOptions> MessageOptions => Set<MessageOptions>();
        public DbSet<MessageCategoryOptions> MessageCategoryOptions => Set<MessageCategoryOptions>();
        public DbSet<ClassTeacher> ClassTeachers => Set<ClassTeacher>();
        public DbSet<Form> Forms => Set<Form>();
        public DbSet<FormField> FormFields => Set<FormField>();
        public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
        public DbSet<FormSubmissionField> FormSubmissionFields => Set<FormSubmissionField>();

        // --- 勤怠・給与関連 ---
        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Shift> Shifts => Set<Shift>();

        public DbSet<ShiftType> ShiftTypes => Set<ShiftType>();
        public DbSet<TimeCard> TimeCards => Set<TimeCard>();
        public DbSet<OvertimeRate> OvertimeRates => Set<OvertimeRate>();
        public DbSet<PaySlip> PaySlips => Set<PaySlip>();

        /// <summary>Fluent API 設定</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== リレーション (外部キー & ON DELETE) 設定 ==========

            // User - Children (1:N)
            modelBuilder.Entity<Children>()
                .HasOne(c => c.User)
                .WithMany(u => u.Children)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - EmergencyContact (1:N)
            modelBuilder.Entity<EmergencyContact>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.EmergencyContacts)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Rout (1:N)
            modelBuilder.Entity<Rout>()
                .HasOne(r => r.User)
                .WithMany(u => u.Routs)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Messages (1:N) → ON DELETE: NoAction
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

            // User - MessageRecipients (1:N)
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

            // ClassTeacher の複合キー (多対多)
            modelBuilder.Entity<ClassTeacher>()
                .HasKey(ct => new { ct.ClassId, ct.UserId });

            // Form - FormField (1:N)
            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormFields)
                .WithOne(ff => ff.Form)
                .HasForeignKey(ff => ff.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            // FormSubmission - FormSubmissionField (1:N)
            modelBuilder.Entity<FormSubmission>()
                .HasMany(fs => fs.FormSubmissionFields)
                .WithOne(fsf => fsf.FormSubmission)
                .HasForeignKey(fsf => fsf.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // FormSubmissionField - FormField (1:N)
            modelBuilder.Entity<FormSubmissionField>()
                .HasOne(fsf => fsf.FormField)
                .WithMany()
                .HasForeignKey(fsf => fsf.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee : User = 1 : 1
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee - TimeCard (1:N)
            modelBuilder.Entity<TimeCard>()
                .HasOne(tc => tc.Employee)
                .WithMany(e => e.TimeCards)
                .HasForeignKey(tc => tc.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee - PaySlip (1:N)
            modelBuilder.Entity<PaySlip>()
                .HasOne(ps => ps.Employee)
                .WithMany(e => e.PaySlips)
                .HasForeignKey(ps => ps.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== インデックスなど ==========

            // OvertimeRate
            modelBuilder.Entity<OvertimeRate>()
                .HasIndex(o => new { o.RateType, o.ValidFrom, o.ValidTo })
                .HasDatabaseName("IX_OvertimeRate_RateTypePeriod");

            // ========== カラムの型指定 (decimal等) ==========

            // Employee.BasePay
            modelBuilder.Entity<Employee>()
                .Property(e => e.BasePay)
                .HasColumnType("decimal(18, 2)");

            // OvertimeRate.Rate
            modelBuilder.Entity<OvertimeRate>()
                .Property(o => o.Rate)
                .HasColumnType("decimal(5, 2)");

            // PaySlip (decimalプロパティ一括設定)
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.BasePay).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.OvertimePay).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.LateNightPay).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.HolidayPay).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.OtherAllowance).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PaySlip>()
                .Property(ps => ps.TotalPay).HasColumnType("decimal(18, 2)");

            // FormField.OptionsJson を JSONカラムとして扱う (MySQLやPostgreSQLで活用?)
            modelBuilder.Entity<FormField>()
                .Property(f => f.OptionsJson)
                .HasColumnType("json");

            modelBuilder.Entity<PickupRecord>()
    .HasOne(pr => pr.Children)
    .WithMany()
    .HasForeignKey(pr => pr.ChildrenId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }

    // =====================================================================
    // 以下、エンティティ定義
    // (本来はファイル分割推奨。ここでは一括掲載)
    // =====================================================================

    #region User & Related

    /// <summary>
    /// システム利用ユーザー (保護者 / 先生 / 管理者 など)
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string AADB2CUserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? LineId { get; set; }

        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
        public DateTime CreatedAt { get; set; }

        // ナビゲーションプロパティ
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<Children> Children { get; } = new List<Children>();
        public virtual ICollection<UserAppointment> UserAppointments { get; } = new List<UserAppointment>();
        public virtual ICollection<Appointment> Appointments { get; } = new List<Appointment>();
        public virtual ICollection<Rout> Routs { get; } = new List<Rout>();
        public virtual ICollection<Photo> Photos { get; } = new List<Photo>();
        public virtual ICollection<Message> Messages { get; } = new List<Message>();
        public virtual ICollection<MessageRecipients> MessageRecipients { get; } = new List<MessageRecipients>();
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; } = new List<EmergencyContact>();
        public virtual ICollection<ClassTeacher> ClassTeachers { get; } = new List<ClassTeacher>();
    }

    /// <summary>緊急連絡先</summary>
    public class EmergencyContact
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Relation { get; set; }
        public int? Rank { get; set; }
        public string? Type { get; set; }
        public string? Tel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; } = new User();
    }

    #endregion

    #region Children & Class

    /// <summary>園児・子ども情報</summary>
    public class Children
    {
        public int Id { get; set; }
        public int? ClassId { get; set; } = 2;
        public int UserId { get; set; }
        public string? Name { get; set; }

        public int PickupType { get; set; }  // 第1種～第5種

        public TimeOnly? PickupExpectedTime { get; set; }


        public DateTime? Birthday { get; set; }
        public string? AllergyInfo { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Rank { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    public class PickupTimeSetting
    {
        public int PickupTimeSettingId { get; set; }

        public string PickupType { get; set; } = string.Empty;
        public int Hour { get; set; }
        public int Minute { get; set; }

        public virtual ICollection<Children> Children { get; set; } = new List<Children>();
    }



    public class PickupRecord
    {
        public int PickupRecordId { get; set; }
        public int ChildrenId { get; set; }

        public int? PickupType { get; set; }  //遅延時間（早ければマイナス）

        public DateTime PickupTime { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? DelayMinutes { get; set; }  // 遅延時間（早ければマイナス）


        public virtual Children? Children { get; set; }
    }

    /// <summary>クラス(組)情報</summary>
    public class Class
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<ClassTeacher> ClassTeachers { get; } = new List<ClassTeacher>();
        public virtual ICollection<Children> Children { get; } = new List<Children>();

        [NotMapped] // UIなどで使う一時的なプロパティ(データベースには持たない)
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();
    }

    /// <summary>クラスと先生(ユーザー)の多対多を実現する中間テーブル</summary>
    public class ClassTeacher
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    #endregion

    #region Appointment (スケジュール)

    /// <summary>ユーザーと予定の関係 (多対多の中間等)？</summary>
    public class UserAppointment
    {
        public int UserAppointmentId { get; set; }
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>カレンダー等で管理する予定</summary>
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
        public bool AllDay { get; set; }
        public bool IsAll { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsClass { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    #endregion

    #region Rout & Photo

    /// <summary>通勤やルート情報？</summary>
    public class Rout
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? CommuteCategory { get; set; }
        public string? PhotoLocation { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }

    /// <summary>写真 (通勤の写真など？)</summary>
    public class Photo
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

    #endregion

    #region Message & Category

    public class Message
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int MessageCategoryId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int? PhotoId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Photo? Photo { get; set; }
        public virtual MessageCategory MessageCategory { get; set; } = null!;
        public virtual ICollection<MessageOptions> MessageOptions { get; set; } = new List<MessageOptions>();
        public virtual ICollection<MessageRecipients> MessageRecipients { get; set; } = new List<MessageRecipients>();
    }

    public class MessageCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }

    public class MessageRecipients
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }

        public virtual User? User { get; set; }
        public virtual Message? Message { get; set; }
    }

    public class MessageOptions
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string OptionKey { get; set; } = string.Empty;
        public string OptionValue { get; set; } = string.Empty;

        public virtual Message Message { get; set; } = null!;
    }

    public class MessageCategoryOptions
    {
        public int Id { get; set; }
        public int MessageCategoryId { get; set; }
        public string OptionKey { get; set; } = string.Empty;

        public virtual MessageCategory MessageCategory { get; set; } = null!;
    }

    #endregion

    #region Form & Field

    /// <summary>フォーム (アンケート等) の定義</summary>
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public string FormType { get; set; } = string.Empty;
        public virtual ICollection<FormField> FormFields { get; set; } = new List<FormField>();
    }

    /// <summary>フォームの入力項目</summary>
    public class FormField
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string FieldType { get; set; } = "text";
        public bool IsRequired { get; set; }
        public int Order { get; set; }

        // JSON形式で保存されるオプション
        public string? OptionsJson { get; set; }

        [NotMapped]
        public List<string> Options
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OptionsJson))
                {
                    return new List<string>(); // `null` の場合は空リストを返す
                }

                try
                {
                    return JsonSerializer.Deserialize<List<string>>(OptionsJson) ?? new List<string>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON パースエラー: {ex.Message}"); // デバッグ用ログ
                    return new List<string>(); // エラー発生時も空リストを返す
                }
            }
            set => OptionsJson = JsonSerializer.Serialize(value ?? new List<string>());
        }


        public virtual Form Form { get; set; } = null!;
    }

    /// <summary>フォームへの提出 (回答) 情報</summary>
    public class FormSubmission
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public int UserId { get; set; }
        public int? ChildrenId { get; set; }
        public DateTime SubmittedAt { get; set; }

        public virtual Form Form { get; set; } = null!;
        public virtual ICollection<FormSubmissionField> FormSubmissionFields { get; set; } = new List<FormSubmissionField>();
    }

    /// <summary>フォーム回答の各項目</summary>
    public class FormSubmissionField
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int FieldId { get; set; }
        public string? StringValue { get; set; }

        public int? IntValue { get; set; }

        public DateTime? DateValue { get; set; }

        public virtual FormSubmission FormSubmission { get; set; } = null!;
        public virtual FormField FormField { get; set; } = null!;
    }

    #endregion

    #region 勤怠・給与管理

    /// <summary>
    /// 従業員(Teacherなど)の情報。Userと1:1対応
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public bool IsHourly { get; set; }
        public decimal BasePay { get; set; }      // ← Fluent APIで decimal(18,2) 指定
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual User User { get; set; } = default!;

        public List<TimeCard>? TimeCards { get; set; }
        public List<PaySlip>? PaySlips { get; set; }

        public List<Shift>? Shifts { get; set; }
    }

    public class ShiftType
    {
        public int ShiftTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }  // TimeOnly? → TimeSpan? に変更
        public TimeOnly EndTime { get; set; }    // TimeOnly? → TimeSpan? に変更
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }


    public class Shift
    {
        public int ShiftId { get; set; }
        public int EmployeeId { get; set; }
        public int ShiftTypeId { get; set; } // シフト種別ID
        public DateTime WorkDate { get; set; } // 勤務日 (時間なし)
        public DateTime StartTime { get; set; } // 開始時間 (シフトの時間設定に基づく)
        public DateTime EndTime { get; set; } // 終了時間 (シフトの時間設定に基づく)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Employee? Employee { get; set; }
        public ShiftType? ShiftType { get; set; }
    }


    /// <summary>
    /// タイムカード(日次の勤怠記録)
    /// </summary>
    public class TimeCard
    {
        public int TimeCardId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public TimeSpan? BreakTime { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsEarlyLeaving { get; set; }
        public TimeSpan? RegularWorkHours { get; set; }
        public TimeSpan? OvertimeHours { get; set; }
        public TimeSpan? LateNightOvertimeHours { get; set; }
        public TimeSpan? HolidayWorkHours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Employee? Employee { get; set; }
    }

    /// <summary>
    /// 割増率マスタ(残業/休日/深夜等)
    /// </summary>
    public class OvertimeRate
    {
        public int OvertimeRateId { get; set; }
        public string RateType { get; set; } = string.Empty;
        public decimal Rate { get; set; }   // ← Fluent APIで decimal(5,2) 指定
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 給与明細(支給履歴)
    /// </summary>
    public class PaySlip
    {
        public int PaySlipId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal BasePay { get; set; }        // decimal(18,2)
        public decimal OvertimePay { get; set; }    // decimal(18,2)
        public decimal LateNightPay { get; set; }   // decimal(18,2)
        public decimal HolidayPay { get; set; }     // decimal(18,2)
        public decimal OtherAllowance { get; set; } // decimal(18,2)
        public decimal TotalPay { get; set; }       // decimal(18,2)
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Employee? Employee { get; set; }
    }

    #endregion
}
