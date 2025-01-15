namespace Domain.Events
{
    public class ChatSavedEvent(int chatId)
    {
        public int ChatId { get; private set; } = chatId;

        public string Message => $"Chat {ChatId} saved";
    }
}
