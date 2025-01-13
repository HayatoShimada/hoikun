using hoikun.Data;
using Microsoft.EntityFrameworkCore;

public class MessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 受信メッセージ一覧を取得
    public async Task<List<MessageDto>> GetReceivedMessagesAsync(int userId)
    {
        return await _context.MessageRecipients
            .Include(mr => mr.Message)
            .ThenInclude(m => m.User)
            .Where(mr => mr.UserId == userId)
            .Select(mr => new MessageDto
            {
                Id = mr.Message.Id,
                Subject = mr.Message.Subject,
                Body = mr.Message.Body,
                CreatedAt = mr.Message.CreatedAt ?? DateTime.MinValue,
                SenderName = mr.Message.User.Name,
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

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto { UserId = u.UserId, Name = u.Name })
            .ToListAsync();
    }

    // メッセージを作成（新規作成 & 返信）
    public async Task<int> CreateMessageAsync(int senderId, MessageDto messageDto, List<int> recipientIds, Dictionary<string, string> options, byte[]? photoData)
    {
        // メッセージカテゴリが存在するかチェック
        bool categoryExists = await _context.MessageCategories
            .AnyAsync(c => c.Id == messageDto.MessageCategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException($"MessageCategoryId {messageDto.MessageCategoryId} is invalid.");
        }

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

        await SaveMessageRecipientsAsync(message.Id, recipientIds);
        await SaveMessageOptionsAsync(message.Id, options);

        return message.Id;
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
    private async Task SaveMessageRecipientsAsync(int messageId, List<int> recipientIds)
    {
        List<MessageRecipients> recipients = recipientIds.Select(id => new MessageRecipients
        {
            MessageId = messageId,
            UserId = id,
            IsRead = false
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
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string SenderName { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}



public class MessageCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class MessageCategoryOptionsDto
{
    public string OptionKey { get; set; } = string.Empty;
}

public class UserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}