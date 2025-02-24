using hoikun.Data;
using hoikun.Models;

public interface IDbContextService
{
    Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId);
    Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId);

    Task<List<User>?> GetUserAsync(string? role);


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
    Task<List<Children>> GetChildrensAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier);

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

    // Form回答の更新
    Task UpdateSubmissionAsync(FormSubmission submission);

}
