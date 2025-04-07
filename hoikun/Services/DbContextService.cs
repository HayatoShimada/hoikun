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
            return await modifiedQuery.Include(c => c.Class).ToListAsync();
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

    public async Task UpdateTimeCardAsync(TimeCard timeCard)
    {
        var local = _dbContext.TimeCards.Local.FirstOrDefault(tc => tc.TimeCardId == timeCard.TimeCardId);
        if (local != null)
        {
            _dbContext.Entry(local).State = EntityState.Detached;
        }

        _dbContext.TimeCards.Update(timeCard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTimeCardAsync(int timeCardId)
    {
        var entity = await _dbContext.TimeCards.FindAsync(timeCardId);
        if (entity != null)
        {
            _dbContext.TimeCards.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<User>?> GetUserAsync(string? role)
    {
        IQueryable<User> query = _dbContext.Users.Include(u => u.Employee);

        if (role != null)
        {
            query = query.Where(q => q.Role == role);
        }

        return await query.ToListAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
        if (existingUser != null)
        {
            // 全プロパティを更新
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.AADB2CUserId = user.AADB2CUserId;
            existingUser.PostalCode = user.PostalCode;
            existingUser.State = user.State;
            existingUser.City = user.City;
            existingUser.Street = user.Street;


            await _dbContext.SaveChangesAsync();
        }
    }


    public async Task<bool> UpdateOneUserAsync(User user)
    {
        try
        {
            User? existingUser = await _dbContext.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                return false; // ユーザーが見つからない場合
            }

            _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
            await _dbContext.SaveChangesAsync();

            return true; // 更新成功
        }
        catch (Exception ex)
        {
            // 例外をログに記録する（ログ機能を実装している場合）
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false; // 更新失敗
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
    public async Task<int> GetLastInsertedSubmissionIdAsync()
    {
        FormSubmission? lastSubmission = await _dbContext.FormSubmissions
            .OrderByDescending(s => s.Id)
            .FirstOrDefaultAsync();
        return lastSubmission?.Id ?? 0;
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

    public async Task<List<FormSubmission>> GetFormSubmissionsByUserIdAsync(int userId)
    {
        return await _dbContext.FormSubmissions
            .Include(fs => fs.Form)
            .Where(fs => fs.UserId == userId)
            .OrderByDescending(fs => fs.SubmittedAt)
            .ToListAsync();
    }

    public async Task<FormSubmission?> GetFormSubmissionDetailAsync(int submissionId)
    {
        return await _dbContext.FormSubmissions
            .Include(fs => fs.Form)
            .Include(fs => fs.FormSubmissionFields)
            .ThenInclude(fsf => fsf.FormField)
            .FirstOrDefaultAsync(fs => fs.Id == submissionId);
    }


    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _dbContext.Employees.ToListAsync();
    }

    public async Task<List<Shift>> GetShiftsAsync(int year, int month)
    {
        return await _dbContext.Shifts
            .Where(s => s.WorkDate.Year == year && s.WorkDate.Month == month)
            .ToListAsync();
    }

    public async Task AddShiftAsync(Shift shift)
    {
        _dbContext.Shifts.Add(shift);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateShiftAsync(Shift shift)
    {
        _dbContext.Shifts.Update(shift);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ShiftType>> GetShiftTypesAsync()
    {
        return await _dbContext.ShiftTypes.ToListAsync();
    }

    public async Task AddShiftTypeAsync(ShiftType shiftType)
    {
        _dbContext.ShiftTypes.Add(shiftType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateShiftTypeAsync(ShiftType shiftType)
    {
        _dbContext.ShiftTypes.Update(shiftType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteShiftTypeAsync(int shiftTypeId)
    {
        var shiftType = await _dbContext.ShiftTypes.FindAsync(shiftTypeId);
        if (shiftType != null)
        {
            _dbContext.ShiftTypes.Remove(shiftType);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteShiftAsync(int shiftId)
    {
        var shift = await _dbContext.Shifts.FindAsync(shiftId);
        if (shift != null)
        {
            _dbContext.Shifts.Remove(shift);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AddTimeCardAsync(TimeCard timeCard)
    {
        _dbContext.TimeCards.Add(timeCard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Shift>> GetShiftsAsync(Func<IQueryable<Shift>, IQueryable<Shift>> queryModifier)
    {
        return await queryModifier(_dbContext.Shifts).ToListAsync();
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPickupRecordAsync(PickupRecord record) // ★ 実装
    {
        _dbContext.Add(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<PickupRecord>> GetPickupRecordsAsync(Func<IQueryable<PickupRecord>, IQueryable<PickupRecord>> queryModifier)
    {
        return await queryModifier(_dbContext.Set<PickupRecord>()).ToListAsync();
    }

    // --- PickupTimeSetting 関連 ---
    public async Task<List<PickupTimeSetting>> GetPickupTimeSettingsAsync()
    {
        return await _dbContext.Set<PickupTimeSetting>()
            .Include(p => p.Children)
            .ThenInclude(c => c.User)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task UpdatePickupRecordAsync(PickupRecord record)
    {
        var local = _dbContext.PickupRecords.Local
            .FirstOrDefault(p => p.PickupRecordId == record.PickupRecordId);

        if (local != null)
        {
            _dbContext.Entry(local).State = EntityState.Detached;
        }

        _dbContext.PickupRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PickupTimeSetting?> GetPickupTimeSettingByTypeAsync(int id)
    {
        return await _dbContext.Set<PickupTimeSetting>()
            .FirstOrDefaultAsync(p => p.PickupTimeSettingId == id);
    }

    public async Task AddPickupTimeSettingAsync(PickupTimeSetting setting)
    {
        _dbContext.Set<PickupTimeSetting>().Add(setting);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePickupTimeSettingAsync(PickupTimeSetting setting)
    {
        var existing = await _dbContext.PickupTimeSettings
            .FirstOrDefaultAsync(p => p.PickupTimeSettingId == setting.PickupTimeSettingId);

        if (existing == null)
            throw new KeyNotFoundException($"PickupTimeSetting with Id {setting.PickupTimeSettingId} not found.");

        // 明示的に必要なプロパティのみ更新（Children は更新しない）
        existing.PickupType = setting.PickupType;
        existing.Hour = setting.Hour;
        existing.Minute = setting.Minute;

        await _dbContext.SaveChangesAsync();
    }


    public async Task DeletePickupRecordAsync(int pickupRecordId)
    {
        var entity = await _dbContext.PickupRecords.FindAsync(pickupRecordId);
        if (entity != null)
        {
            _dbContext.PickupRecords.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> CreateBlogAsync(Blog blog, List<BlogContent> contents)
    {
        blog.CreatedAt = DateTime.Now;
        blog.PublishedAt = DateTime.Now;

        _dbContext.Blogs.Add(blog);
        await _dbContext.SaveChangesAsync();

        foreach (var content in contents)
        {
            content.BlogId = blog.BlogId;
        }

        _dbContext.BlogContents.AddRange(contents);
        await _dbContext.SaveChangesAsync();

        return blog.BlogId;
    }

    public async Task<List<Blog>> GetAllBlogsAsync()
    {
        return await _dbContext.Blogs
            .Include(b => b.BlogContents.OrderBy(c => c.Order))
            .OrderByDescending(b => b.PublishedAt)
            .ToListAsync();
    }

    public async Task<Blog?> GetBlogByIdAsync(int blogId)
    {
        return await _dbContext.Blogs
            .Include(b => b.BlogContents.OrderBy(c => c.Order))
            .FirstOrDefaultAsync(b => b.BlogId == blogId);
    }

    public async Task UpdateBlogAsync(Blog blog, List<BlogContent> contents)
    {
        blog.UpdatedAt = DateTime.Now;
        _dbContext.Blogs.Update(blog);

        var oldContents = _dbContext.BlogContents.Where(c => c.BlogId == blog.BlogId);
        _dbContext.BlogContents.RemoveRange(oldContents);

        foreach (var content in contents)
        {
            content.BlogId = blog.BlogId;
        }

        _dbContext.BlogContents.AddRange(contents);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteBlogAsync(int blogId)
    {
        var blog = await _dbContext.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            var contents = _dbContext.BlogContents.Where(c => c.BlogId == blogId);
            _dbContext.BlogContents.RemoveRange(contents);
            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();
        }
    }

}
