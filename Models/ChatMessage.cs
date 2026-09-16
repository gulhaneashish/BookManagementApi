namespace BookStoreApi.Models;

public class ChatMessage
{
    public int Id { get; set; }

    public string SenderUserId { get; set; }
        = string.Empty;

    public string ReceiverUserId { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}