using hoikun.Data;
using hoikun.Models;

public interface IDbContextService
{
    Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId);
    Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId);

    Task<List<User>?> GetUserAsync(string? role);

    Task UpdateUserAsync(Func<IQueryable<User>, IQueryable<User>> queryModifier);

    Task<bool> UpdateOneUserAsync(User user);

    Task AddClassTeacherAsync(ClassTeacher classTeacher);
    Task UpdateClassTeacherAsync(ClassTeacher classTeacher);
    Task DeleteClassTeacherAsync(ClassTeacher classTeacher);

    Task<List<Class>> GetClassesAsync(Func<IQueryable<Class>, IQueryable<Class>> queryModifier);
    Task UpdateClassAsync(int classId, Action<Class> updateAction);
    // Class の新規追加
    Task AddClassAsync(Class classEntity);
    // Class の削除
    Task DeleteClassAsync(int classId);

    // Children の取得
    Task<List<Children>> GetChildrenAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier);

    // Children を更新
    Task UpdateChildrenAsync(int childrenId, Children updatedChild);

    // Children の新規追加
    Task AddChildrenAsync(Children childrenEntity);

    // Children の削除
    Task DeleteChildrenAsync(int childrenId);

    // Form の新規作成
    Task CreateFormAsync(FormModel form, List<FormFieldModel> fields);

    // Form の取得
    Task<List<Form>> GetFormAsync(Func<IQueryable<Form>, IQueryable<Form>> queryModifier);

    // Form の取得（単一)
    Task<Form?> GetFormByIdAsync(int formId);

    // Form の更新
    Task UpdateFormAsync(int formId, FormModel form, List<FormFieldModel> fields);

    // TimeCardの取得
    Task<List<TimeCard>> GetTimeCardAsync(Func<IQueryable<TimeCard>, IQueryable<TimeCard>> queryModifier);

    // Form回答の追加
    Task SubmitFormAsync(FormSubmission submission);

    // Form回答要素の追加
    Task SubmitFormFieldsAsync(List<FormSubmissionField> formSubmissionFields);

    // Form回答の取得
    Task<FormSubmission?> GetSubmissionAsync(int formId, int userId, int childrenId);

    Task<List<FormSubmissionField>?> GetSubmissionFieldsAsync(int submissionId);
    Task<int> GetLastInsertedSubmissionIdAsync();

    // Form回答の更新
    Task UpdateSubmissionAsync(FormSubmission submission);

    Task<List<FormSubmission>> GetFormSubmissionsByUserIdAsync(int userId);
    Task<FormSubmission?> GetFormSubmissionDetailAsync(int submissionId);

    // Employee の取得
    Task<List<Employee>> GetEmployeesAsync();

    // 指定した年・月の Shift を取得
    Task<List<Shift>> GetShiftsAsync(int year, int month);

    Task AddShiftAsync(Shift shift);
    Task UpdateShiftAsync(Shift shift);

    // ユーザー一覧を取得
    Task<List<User>> GetUsersAsync();

    // Employee の追加
    Task AddEmployeeAsync(Employee employee);

    // シフトタイプの取得
    Task<List<ShiftType>> GetShiftTypesAsync();

    Task AddShiftTypeAsync(ShiftType shiftType);
    Task UpdateShiftTypeAsync(ShiftType shiftType);
    Task DeleteShiftTypeAsync(int shiftTypeId);

    Task DeleteShiftAsync(int shiftId);

}
