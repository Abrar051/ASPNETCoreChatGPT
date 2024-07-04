using System.Text.Json.Serialization;

namespace ASPNETCoreChatGPT.Models
{
    public class ChatMemory
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("chatId")]
        public string ChatId { get; set; }

        [JsonPropertyName("chatMessageId")]
        public string ChatMessageId { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("memoryType")]
        public string MemoryType { get; set; }
    }
}
