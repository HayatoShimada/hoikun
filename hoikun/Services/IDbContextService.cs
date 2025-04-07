// IDbContextService に PickupTimeSetting 関連のメソッドを追加
using hoikun.Data;
using hoikun.Models;

using System.Threading.Tasks;

public interface IDbContextService
{
    // --- ClassTeacher ---
    Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId);
    Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId);

    // --- User ---
    Task<List<User>?> GetUserAsync(string? role);
    Task UpdateUserAsync(User user);
    Task<bool> UpdateOneUserAsync(User user);
    Task<List<User>> GetUsersAsync();

    // --- ClassTeacher 操作 ---
    Task AddClassTeacherAsync(ClassTeacher classTeacher);
    Task UpdateClassTeacherAsync(ClassTeacher classTeacher);
    Task DeleteClassTeacherAsync(ClassTeacher classTeacher);

    // --- Class ---
    Task<List<Class>> GetClassesAsync(Func<IQueryable<Class>, IQueryable<Class>> queryModifier);
    Task UpdateClassAsync(int classId, Action<Class> updateAction);
    Task AddClassAsync(Class classEntity);
    Task DeleteClassAsync(int classId);

    // --- Children ---
    Task<List<Children>> GetChildrenAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier);
    Task UpdateChildrenAsync(int childrenId, Children updatedChild);
    Task AddChildrenAsync(Children childrenEntity);
    Task DeleteChildrenAsync(int childrenId);

    // --- Form ---
    Task CreateFormAsync(FormModel form, List<FormFieldModel> fields);
    Task<List<Form>> GetFormAsync(Func<IQueryable<Form>, IQueryable<Form>> queryModifier);
    Task<Form?> GetFormByIdAsync(int formId);
    Task UpdateFormAsync(int formId, FormModel form, List<FormFieldModel> fields);

    // --- Form Submission ---
    Task SubmitFormAsync(FormSubmission submission);
    Task SubmitFormFieldsAsync(List<FormSubmissionField> formSubmissionFields);
    Task<FormSubmission?> GetSubmissionAsync(int formId, int userId, int childrenId);
    Task<List<FormSubmissionField>?> GetSubmissionFieldsAsync(int submissionId);
    Task<int> GetLastInsertedSubmissionIdAsync();
    Task UpdateSubmissionAsync(FormSubmission submission);
    Task<List<FormSubmission>> GetFormSubmissionsByUserIdAsync(int userId);
    Task<FormSubmission?> GetFormSubmissionDetailAsync(int submissionId);

    // --- Employee ---
    Task<List<Employee>> GetEmployeesAsync();
    Task AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);

    // --- Shift & TimeCard ---
    Task<List<Shift>> GetShiftsAsync(int year, int month);
    Task<List<Shift>> GetShiftsAsync(Func<IQueryable<Shift>, IQueryable<Shift>> queryModifier);
    Task AddShiftAsync(Shift shift);
    Task UpdateShiftAsync(Shift shift);
    Task DeleteShiftAsync(int shiftId);

    // --- TimeCard ---
    Task UpdateTimeCardAsync(TimeCard timeCard);
    Task AddTimeCardAsync(TimeCard timeCard);
    Task<List<TimeCard>> GetTimeCardAsync(Func<IQueryable<TimeCard>, IQueryable<TimeCard>> queryModifier);

    Task DeleteTimeCardAsync(int timeCardId);

    // --- ShiftType ---
    Task<List<ShiftType>> GetShiftTypesAsync();
    Task AddShiftTypeAsync(ShiftType shiftType);
    Task UpdateShiftTypeAsync(ShiftType shiftType);
    Task DeleteShiftTypeAsync(int shiftTypeId);

    // --- PickupRecord ---
    Task AddPickupRecordAsync(PickupRecord record);
    Task UpdatePickupRecordAsync(PickupRecord record);
    Task<List<PickupRecord>> GetPickupRecordsAsync(Func<IQueryable<PickupRecord>, IQueryable<PickupRecord>> queryModifier);

    Task DeletePickupRecordAsync(int pickupRecordId);


    // --- PickupTimeSetting ---
    Task<List<PickupTimeSetting>> GetPickupTimeSettingsAsync();

    Task<PickupTimeSetting> GetPickupTimeSettingByTypeAsync(int id);
    Task AddPickupTimeSettingAsync(PickupTimeSetting setting);
    Task UpdatePickupTimeSettingAsync(PickupTimeSetting setting);

    // --- Blog Service ---

    Task<int> CreateBlogAsync(Blog blog, List<BlogContent> contents);
    Task<List<Blog>> GetAllBlogsAsync();
    Task<Blog?> GetBlogByIdAsync(int blogId);
    Task UpdateBlogAsync(Blog blog, List<BlogContent> contents);
    Task DeleteBlogAsync(int blogId);


}
