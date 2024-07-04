namespace ASPNETCoreChatGPT.Models
{
    public class JsonContent
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
    }

    public class Content
    {
        public JsonContent Json { get; set; }
        public string Question { get; set; }
        public string ChatId { get; set; }
        public string ChatMessageId { get; set; }
        public string SessionId { get; set; }
    }
}
