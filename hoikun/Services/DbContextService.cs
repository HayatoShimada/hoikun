using hoikun.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static hoikun.Pages.FormCreate;

public class DbContextService : IDbContextService
{
    private readonly ApplicationDbContext _dbContext;

    public DbContextService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ClassTeacher>> GetClassTeachersAsync(int? classId)
    {
        return classId == null
            ? await _dbContext.ClassTeachers
                .AsNoTracking()
                .ToListAsync()
            : (IEnumerable<ClassTeacher>)await _dbContext.ClassTeachers
                .Where(ct => ct.ClassId == classId)
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<ClassTeacher?> GetClassTeacherAsync(int classId, int userId)
    {
        return await _dbContext.ClassTeachers
            .FirstOrDefaultAsync(ct => ct.ClassId == classId && ct.UserId == userId);
    }

    public async Task AddClassTeacherAsync(ClassTeacher classTeacher)
    {
        _dbContext.ClassTeachers.Add(classTeacher);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateClassTeacherAsync(ClassTeacher classTeacher)
    {
        ClassTeacher? existingClassTeacher = await GetClassTeacherAsync(classTeacher.ClassId, classTeacher.UserId);
        if (existingClassTeacher != null)
        {
            _dbContext.Entry(existingClassTeacher).CurrentValues.SetValues(classTeacher);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteClassTeacherAsync(ClassTeacher classTeacher)
    {
        _dbContext.ClassTeachers.Remove(classTeacher);
        await _dbContext.SaveChangesAsync();
    }

    // Class クラスの取得
    // 全件取得　var allClasses = await GetClassesAsync(query => query);
    // 特定条件　var filteredClasses = await GetClassesAsync(query => query.Where(c => c.IsActive));
    // 並び替え　var sortedClasses = await GetClassesAsync(query => query.OrderBy(c => c.Name));
    // 複数条件　var filteredAndSortedClasses = await GetClassesAsync(query => query.Where(c => c.IsActive).OrderBy(c => c.Name));

    public async Task<List<Class>> GetClassesAsync(Func<IQueryable<Class>, IQueryable<Class>> queryModifier)
    {
        try
        {
            IQueryable<Class> items = _dbContext.Classes.AsQueryable();
            IQueryable<Class> modifiedQuery = queryModifier(items);
            return await modifiedQuery.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new DataException("Error retrieving classes.", ex);
        }
    }

    // Class の更新
    public async Task UpdateClassAsync(int classId, Action<Class> updateAction)
    {
        // 更新対象のクラスを取得
        Class? classEntity = await _dbContext.Classes.FindAsync(classId);
        if (classEntity == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        // 動的な更新処理を適用
        updateAction(classEntity);

        // データベースに保存
        await _dbContext.SaveChangesAsync();
    }

    // Class の新規追加
    public async Task AddClassAsync(Class classEntity)
    {
        if (classEntity == null)
            throw new ArgumentNullException(nameof(classEntity));

        // クラスをデータベースに追加
        _dbContext.Classes.Add(classEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Class の削除
    public async Task DeleteClassAsync(int classId)
    {
        Class? classEntity = await _dbContext.Classes.FindAsync(classId);
        if (classEntity == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        _dbContext.Classes.Remove(classEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Children の取得
    // 全件取得　var allClasses = await GetClassesAsync(query => query);
    // 特定条件　var filteredClasses = await GetClassesAsync(query => query.Where(c => c.IsActive));
    // 並び替え　var sortedClasses = await GetClassesAsync(query => query.OrderBy(c => c.Name));
    // 複数条件　var filteredAndSortedClasses = await GetClassesAsync(query => query.Where(c => c.IsActive).OrderBy(c => c.Name));

    public async Task<List<Children>> GetChildrensAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier)
    {
        try
        {
            IQueryable<Children> items = _dbContext.Childrens.AsQueryable().AsNoTracking();
            IQueryable<Children> modifiedQuery = queryModifier(items);
            return await modifiedQuery.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

    }

    // Children の更新
    public async Task UpdateChildrenAsync(int childrenId, Children updatedChild)
    {
        Children? childrenEntity = await _dbContext.Childrens.FindAsync(childrenId);
        if (childrenEntity == null)
            throw new KeyNotFoundException($"Children with ID {childrenId} not found.");

        // 既存エンティティに新しい値を適用
        _dbContext.Entry(childrenEntity).CurrentValues.SetValues(updatedChild);

        // データベースに保存
        await _dbContext.SaveChangesAsync();
    }


    // Children の追加
    public async Task AddChildrenAsync(Children childrenEntity)
    {
        if (childrenEntity == null)
            throw new ArgumentNullException(nameof(childrenEntity));

        // Children をデータベースに追加
        _dbContext.Childrens.Add(childrenEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Children の削除
    public async Task DeleteChildrenAsync(int childrenId)
    {
        Children? childrenEntity = await _dbContext.Childrens.FindAsync(childrenId);
        if (childrenEntity == null)
            throw new KeyNotFoundException($"Children with ID {childrenId} not found.");

        _dbContext.Childrens.Remove(childrenEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Form の新規追加
    public async Task CreateFormAsync(FormModel form, List<FormFieldModel> fields)
    {
        Form newForm = new()
        {
            Name = form.Name,
            Description = form.Description,
            CreatedAt = DateTime.UtcNow,
            FormFields = fields.Select(field => new FormField
            {
                Name = field.Name,
                Label = field.Label,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                OptionsJson = field.Options // JSONとして保存
            }).ToList()
        };

        _dbContext.Forms.Add(newForm);
        await _dbContext.SaveChangesAsync();
    }


}
