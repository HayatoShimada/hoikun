using hoikun.Data;
using hoikun.Models;

using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Text.Json;

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
        Class? classEntity = await _dbContext.Classes.FindAsync(classId) ?? throw new KeyNotFoundException($"Class with ID {classId} not found.");

        // 動的な更新処理を適用
        updateAction(classEntity);

        // データベースに保存
        await _dbContext.SaveChangesAsync();
    }

    // Class の新規追加
    public async Task AddClassAsync(Class classEntity)
    {
        ArgumentNullException.ThrowIfNull(classEntity);

        // クラスをデータベースに追加
        _dbContext.Classes.Add(classEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Class の削除
    public async Task DeleteClassAsync(int classId)
    {
        Class? classEntity = await _dbContext.Classes.FindAsync(classId) ?? throw new KeyNotFoundException($"Class with ID {classId} not found.");
        _dbContext.Classes.Remove(classEntity);
        await _dbContext.SaveChangesAsync();
    }

    // Children の取得
    // 全件取得　var allClasses = await GetClassesAsync(query => query);
    // 特定条件　var filteredClasses = await GetClassesAsync(query => query.Where(c => c.IsActive));
    // 並び替え　var sortedClasses = await GetClassesAsync(query => query.OrderBy(c => c.Name));
    // 複数条件　var filteredAndSortedClasses = await GetClassesAsync(query => query.Where(c => c.IsActive).OrderBy(c => c.Name));

    public async Task<List<Children>> GetChildrenAsync(Func<IQueryable<Children>, IQueryable<Children>> queryModifier)
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
            return new List<Children>();
        }

    }

    // Children の更新
    public async Task UpdateChildrenAsync(int childrenId, Children updatedChild)
    {
        Children? childrenEntity = await _dbContext.Childrens.FindAsync(childrenId) ?? throw new KeyNotFoundException($"Children with ID {childrenId} not found.");

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
        Children? childrenEntity = await _dbContext.Childrens.FindAsync(childrenId) ?? throw new KeyNotFoundException($"Children with ID {childrenId} not found.");
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
                Caption = field.Caption,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                OptionsJson = field.OptionList != null && field.OptionList.Any()
                ? JsonSerializer.Serialize(field.OptionList.Select(opt => opt.Option))
                : null // OptionListが空の場合はnullを保存
            }).ToList()
        };

        _dbContext.Forms.Add(newForm);
        await _dbContext.SaveChangesAsync();
    }

    // Form の取得
    public async Task<List<Form>> GetFormAsync(Func<IQueryable<Form>, IQueryable<Form>> queryModifier)
    {
        try
        {
            IQueryable<Form> items = _dbContext.Forms.Include(f => f.FormFields);
            IQueryable<Form> modifiedQuery = queryModifier(items);
            return await modifiedQuery.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new DataException("Error retrieving classes.", ex);
        }
    }

    public async Task<Form?> GetFormByIdAsync(int formId)
    {
        return await _dbContext.Forms
            .Include(f => f.FormFields)
            .FirstOrDefaultAsync(f => f.Id == formId);
    }

    public async Task UpdateFormAsync(int formId, FormModel formModel, List<FormFieldModel> formFields)
    {
        try
        {
            // 既存のフォームを取得
            Form? form = await _dbContext.Forms
                .Include(f => f.FormFields) // 関連データを含める
                .FirstOrDefaultAsync(f => f.Id == formId);

            if (form != null)
            {
                // フォームのプロパティを更新
                form.Name = formModel.Name;
                form.Description = formModel.Description;

                // フィールドを更新
                form.FormFields.Clear(); // 既存のフィールドをクリア
                foreach (FormFieldModel fieldModel in formFields)
                {
                    FormField newField = new()
                    {
                        Name = fieldModel.Name,
                        Label = fieldModel.Label,
                        Caption = fieldModel.Caption,
                        FieldType = fieldModel.FieldType,
                        IsRequired = fieldModel.IsRequired,
                        OptionsJson = fieldModel.OptionList != null && fieldModel.OptionList.Any()
                    ? JsonSerializer.Serialize(fieldModel.OptionList.Select(opt => opt.Option))
                    : null // OptionListが空の場合はnullを保存
                    };
                    form.FormFields.Add(newField);
                }

                // 変更を保存
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Form with ID {formId} not found.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating the form.", ex);
        }
    }

    public async Task<List<TimeCard>> GetTimeCardAsync(
            Func<IQueryable<TimeCard>, IQueryable<TimeCard>>? queryModifier = null)
    {
        IQueryable<TimeCard> query = _dbContext.TimeCards;

        if (queryModifier != null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    public async Task<List<User>?> GetUserAsync(string? role)
    {
        IQueryable<User> query = _dbContext.Users;

        if (role != null)
        {
            query = query.Where(q => q.Role == role);
        }

        return await query.ToListAsync();
    }

    public async Task UpdateUserAsync(Func<IQueryable<User>, IQueryable<User>> queryModifier)
    {
        IQueryable<User> query = _dbContext.Users.AsQueryable();
        List<User> usersToUpdate = await queryModifier(query).ToListAsync();

        if (usersToUpdate.Any())
        {
            foreach (User? user in usersToUpdate)
            {
                user.LineId = "UPDATE_VALUE"; // ここで `LineId` を更新
            }
            await _dbContext.SaveChangesAsync();
        }
    }


    public async Task SubmitFormAsync(FormSubmission submission)
    {
        _dbContext.FormSubmissions.Add(submission);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SubmitFormFieldsAsync(List<FormSubmissionField> formSubmissionFields)
    {
        if (formSubmissionFields == null || !formSubmissionFields.Any())
        {
            throw new ArgumentException("フォーム送信フィールドが存在しません。", nameof(formSubmissionFields));
        }

        _dbContext.FormSubmissionFields.AddRange(formSubmissionFields);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<FormSubmission?> GetSubmissionAsync(int formId, int userId, int childrenId)
    {
        return await _dbContext.FormSubmissions
            .Include(fs => fs.FormSubmissionFields)  // 回答フィールドも読み込む場合
            .FirstOrDefaultAsync(fs => fs.FormId == formId && fs.UserId == userId && fs.ChildrenId == childrenId);
    }

    public async Task<List<FormSubmissionField>?> GetSubmissionFieldsAsync(int submissionId)
    {
        return await _dbContext.FormSubmissionFields
            .Where(fs => fs.SubmissionId == submissionId).ToListAsync();
    }

    public async Task UpdateSubmissionAsync(FormSubmission submission)
    {
        // 既存のレコードを取得
        FormSubmission? existingSubmission = await _dbContext.FormSubmissions.FindAsync(submission.Id);
        if (existingSubmission == null)
        {
            throw new KeyNotFoundException($"FormSubmission with Id {submission.Id} not found.");
        }

        // エンティティの値を更新
        _dbContext.Entry(existingSubmission).CurrentValues.SetValues(submission);
        await _dbContext.SaveChangesAsync();
    }

}
