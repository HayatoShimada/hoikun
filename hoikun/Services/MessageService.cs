using hoikun.Data;
using hoikun.Services;

using Microsoft.EntityFrameworkCore;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class MessageService
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    public MessageService(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // 受信メッセージ一覧を取得
    public async Task<List<MessageDto>> GetReceivedMessagesAsync(int userId)
    {
        return await _context.MessageRecipients
            .Include(mr => mr.Message)
                .ThenInclude(m => m.User)
            .Include(mr => mr.Message)
                .ThenInclude(m => m.MessageCategory)
            .Where(mr => mr.UserId == userId && mr.Message != null)
            .Select(mr => new MessageDto
            {
                Id = mr.Message!.Id,
                MessageCategory = mr.Message.MessageCategory != null
                    ? new MessageCategoryDto
                    {
                        Id = mr.Message.MessageCategory.Id,
                        Name = mr.Message.MessageCategory.Name,
                        IsForm = mr.Message.MessageCategory.Name == "フォーム" // 「フォーム」のみtrue
                    }
                    : null,
                Subject = mr.Message.Subject ?? string.Empty,
                Body = mr.Message.Body ?? string.Empty,
                CreatedAt = mr.Message.CreatedAt ?? DateTime.MinValue,
                SenderName = mr.Message.User != null ? mr.Message.User.Name : "Unknown",
                IsRead = mr.IsRead
            })
            .ToListAsync();
    }



    // メッセージ詳細を取得
    public async Task<MessageDto> GetMessageByIdAsync(int messageId)
    {
        Message? message = await _context.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        return message == null
            ? throw new KeyNotFoundException("Message not found")
            : new MessageDto
            {
                Id = message.Id,
                Subject = message.Subject,
                Body = message.Body,
                CreatedAt = message.CreatedAt ?? DateTime.MinValue,
                SenderName = message.User.Name
            };
    }

    public async Task<bool> ReadMessageAsync(int messageId, int? UserId)
    {
        MessageRecipients? messageRecipient = await _context.MessageRecipients
            .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == UserId);

        if (messageRecipient == null) return false;
        messageRecipient.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // 返信メッセージを送信
    public async Task SendReplyAsync(int userId, int messageId, string replyBody)
    {
        Message? originalMessage = await _context.Messages.FindAsync(messageId);
        if (originalMessage == null) throw new KeyNotFoundException("Message not found");

        Message replyMessage = new()
        {
            UserId = userId,
            MessageCategoryId = originalMessage.MessageCategoryId,
            Subject = "Re: " + originalMessage.Subject,
            Body = replyBody,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(replyMessage);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Children>> GetAllChildrenAsync()
    {
        return await _context.Childrens
            .Include(c => c.Class)
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<List<Class>> GetAllClassAsync()
    {
        return await _context.Classes
            .Include(c => c.ClassTeachers)
            .ToListAsync();
    }

    public async Task<List<Form>> GetAllFormsAsync()
    {
        return await _context.Forms
            .ToListAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .ToListAsync();
    }
    public async Task<List<User>> GetUsersByIdsAsync(IEnumerable<int> userIds)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.UserId))
            .ToListAsync();
    }

    public async Task SendLineMessage(List<string> lineIds, string subject, string body)
    {
        string channelAccessToken = "MyIzgYlUxnOMFYAMpMEMzUAUAc/8plIUqtdgrUpBap6sCc+0ApcHgNj+XoVthw0BAo0R2jdyhFrxynx3KMZDqVzTkO3K6QUx4Z7w2V/wCDWb7YoIs3ms6j/BiPAgwWnNLPscC2DUQLnK/l+J/8DMEAdB04t89/1O/w1cDnyilFU="; // LINEチャネルアクセストークン

        var messages = new
        {
            to = (object)(lineIds.Count == 1 ? lineIds[0] : lineIds),
            messages = new[]
            {
            new
            {
                type = "text",
                text = $"📩 新しいメッセージが届きました\n\n件名: {subject}\n\n{body}"
            }
        }
        };

        string requestUrl = lineIds.Count == 1
            ? "https://api.line.me/v2/bot/message/push"
            : "https://api.line.me/v2/bot/message/multicast";

        string jsonContent = JsonSerializer.Serialize(messages);
        StringContent content = new(jsonContent, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

        HttpResponseMessage response = await client.PostAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"LINE メッセージ送信エラー: {await response.Content.ReadAsStringAsync()}");
        }
    }



    // メッセージを作成（新規作成 & 返信）
    public async Task<int> CreateMessageAsync(int senderId, MessageDto messageDto, HashSet<int> recipientIds, Dictionary<string, string> options, byte[]? photoData)
    {
        // メッセージカテゴリが存在するかチェック
        bool categoryExists = await _context.MessageCategories
            .AnyAsync(c => c.Id == messageDto.MessageCategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException($"MessageCategoryId {messageDto.MessageCategoryId} is invalid.");
        }

        // メッセージ作成
        Message message = new()
        {
            UserId = senderId,
            MessageCategoryId = messageDto.MessageCategoryId,
            Subject = messageDto.Subject,
            Body = messageDto.Body,
            CreatedAt = DateTime.UtcNow
        };

        if (photoData != null && photoData.Length > 0)
        {
            message.PhotoId = await SavePhotoAsync(senderId, photoData);
        }

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // 📌 受信者リストを保存（MessageRecipients に追加）
        await SaveMessageRecipientsAsync(message.Id, recipientIds);

        await SaveMessageOptionsAsync(message.Id, options);

        // 📧 メール送信
        await SendEmailsToRecipients(recipientIds, message.Subject, message.Body);

        return message.Id;
    }

    public async Task SendEmailsToRecipients(HashSet<int> recipientIds, string subject, string body)
    {
        // 受信者の `Email` を取得
        List<string> recipientEmails = await _context.Users
            .Where(u => recipientIds.Contains(u.UserId) && !string.IsNullOrEmpty(u.Email))
            .Select(u => u.Email!)
            .ToListAsync();

        if (recipientEmails.Any())
        {
            await _emailService.SendEmailsAsync(recipientEmails, subject, body);
        }
    }

    public async Task SendEmails(List<User> users, string subject, string body)
    {
        // 受信者の `Email` を取得
        List<string> recipientEmails = users
                    .Where(u => !string.IsNullOrEmpty(u.Email))
                    .Select(u => u.Email!)
                    .ToList();


        if (recipientEmails.Any())
        {
            await _emailService.SendEmailsAsync(recipientEmails, subject, body);
        }
    }



    // 画像を保存
    private async Task<int> SavePhotoAsync(int userId, byte[] photoData)
    {
        Photo photo = new()
        {
            UserId = userId,
            PhotoLocation = Convert.ToBase64String(photoData),
            CreatedAt = DateTime.UtcNow
        };
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
        return photo.Id;
    }

    // 宛先を保存
    private async Task SaveMessageRecipientsAsync(int messageId, HashSet<int> recipientIds)
    {
        List<MessageRecipients> recipients = recipientIds.Select(userId => new MessageRecipients
        {
            MessageId = messageId,
            UserId = userId,
            IsRead = false, // 新しいメッセージなので未読
            ReadAt = null   // 既読日時も null
        }).ToList();

        _context.MessageRecipients.AddRange(recipients);
        await _context.SaveChangesAsync();
    }


    // オプションを保存
    private async Task SaveMessageOptionsAsync(int messageId, Dictionary<string, string> options)
    {
        List<MessageOptions> messageOptions = options.Select(option => new MessageOptions
        {
            MessageId = messageId,
            OptionKey = option.Key,
            OptionValue = option.Value
        }).ToList();

        _context.MessageOptions.AddRange(messageOptions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MessageCategoryDto>> GetMessageCategoriesAsync()
    {
        return await _context.MessageCategories
            .Select(c => new MessageCategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
    }
    public async Task<List<MessageCategory>> GetMessageOptionsAsync()
    {
        return await _context.MessageCategories
            .ToListAsync();
    }



    public async Task<List<MessageCategoryOptionsDto>> GetCategoryOptionsAsync(int categoryId)
    {
        return await _context.MessageCategoryOptions
            .Where(co => co.MessageCategoryId == categoryId)
            .Select(co => new MessageCategoryOptionsDto { OptionKey = co.OptionKey })
            .ToListAsync();
    }
}

public class MessageDto
{
    public int Id { get; set; }

    public int MessageCategoryId { get; set; }
    public MessageCategoryDto? MessageCategory { get; set; } // メッセージカテゴリの詳細を含める

    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string SenderName { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}

public class MessageCategoryDto
{
    public int Id { get; set; }

    public bool IsForm { get; set; } = false;

    public string Name { get; set; } = string.Empty;
}

public class MessageCategoryOptionsDto
{
    public int Id { get; set; }  // MessageCategoryOptions の ID
    public string OptionKey { get; set; } = string.Empty; // `OptionKey` を使用する
}

public class UserDto
{
    public int UserId { get; set; }


    public string Name { get; set; } = string.Empty;
}