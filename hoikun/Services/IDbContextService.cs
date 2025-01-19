using hoikun.Data;
using static hoikun.Pages.FormCreate;

public interface IDbContextService
{
    Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId);
    Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId);
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

    Task CreateFormAsync(FormModel form, List<FormFieldModel> fields);

}
