using hoikun.Data;
using hoikun.Models;

using System.Threading.Tasks;

public interface IDbContextService
{
    // --- Children テーブル関連 ---
    Task<List<Children>> GetChildrenAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier);
    Task UpdateChildrenAsync(int childrenId, Children updatedChild);
    Task AddChildrenAsync(Children childrenEntity);
    Task DeleteChildrenAsync(int childrenId);

    // --- PickupRecord テーブル関連 ---
    Task AddPickupRecordAsync(PickupRecord record);
    Task<List<PickupRecord>> GetPickupRecordsAsync(Func<IQueryable<PickupRecord>, IQueryable<PickupRecord>> queryModifier);


    // --- Employee テーブル関連 ---
    Task<List<Employee>> GetEmployeesAsync();
    Task AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);

    // --- ClassTeacher テーブル関連 ---
    Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId);
    Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId);
    Task AddClassTeacherAsync(ClassTeacher classTeacher);
    Task UpdateClassTeacherAsync(ClassTeacher classTeacher);
    Task DeleteClassTeacherAsync(ClassTeacher classTeacher);

    // --- User テーブル関連 ---
    Task<List<User>?> GetUserAsync(string? role);
    Task UpdateUserAsync(Func<IQueryable<User>, IQueryable<User>> queryModifier);
    Task<bool> UpdateOneUserAsync(User user);
    Task<List<User>> GetUsersAsync();

    // --- Class テーブル関連 ---
    Task<List<Class>> GetClassesAsync(Func<IQueryable<Class>, IQueryable<Class>> queryModifier);
    Task UpdateClassAsync(int classId, Action<Class> updateAction);
    Task AddClassAsync(Class classEntity);
    Task DeleteClassAsync(int classId);

    // --- Form/FormField/FormSubmission 関連 ---
    Task CreateFormAsync(FormModel form, List<FormFieldModel> fields);
    Task<List<Form>> GetFormAsync(Func<IQueryable<Form>, IQueryable<Form>> queryModifier);
    Task<Form?> GetFormByIdAsync(int formId);
    Task UpdateFormAsync(int formId, FormModel form, List<FormFieldModel> fields);
    Task SubmitFormAsync(FormSubmission submission);
    Task SubmitFormFieldsAsync(List<FormSubmissionField> formSubmissionFields);
    Task<FormSubmission?> GetSubmissionAsync(int formId, int userId, int childrenId);
    Task<List<FormSubmissionField>?> GetSubmissionFieldsAsync(int submissionId);
    Task<int> GetLastInsertedSubmissionIdAsync();
    Task UpdateSubmissionAsync(FormSubmission submission);
    Task<List<FormSubmission>> GetFormSubmissionsByUserIdAsync(int userId);
    Task<FormSubmission?> GetFormSubmissionDetailAsync(int submissionId);

    // --- Shift テーブル関連 ---
    Task<List<Shift>> GetShiftsAsync(int year, int month);
    Task<List<Shift>> GetShiftsAsync(Func<IQueryable<Shift>, IQueryable<Shift>> queryModifier);
    Task AddShiftAsync(Shift shift);
    Task UpdateShiftAsync(Shift shift);
    Task DeleteShiftAsync(int shiftId);

    // --- ShiftType テーブル関連 ---
    Task<List<ShiftType>> GetShiftTypesAsync();
    Task AddShiftTypeAsync(ShiftType shiftType);
    Task UpdateShiftTypeAsync(ShiftType shiftType);
    Task DeleteShiftTypeAsync(int shiftTypeId);

    // --- TimeCard テーブル関連 ---
    Task<List<TimeCard>> GetTimeCardAsync(Func<IQueryable<TimeCard>, IQueryable<TimeCard>> queryModifier);
    Task AddTimeCardAsync(TimeCard timeCard);
}
