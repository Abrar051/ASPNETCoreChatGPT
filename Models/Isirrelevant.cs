using Newtonsoft.Json;

namespace YourNamespace.Models
{
    public class Isirrelevant
    {
        [JsonProperty("json")]
        public JsonDetail Json { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("chatId")]
        public string ChatId { get; set; }

        [JsonProperty("chatMessageId")]
        public string ChatMessageId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    public class JsonDetail
    {
        [JsonProperty("ifirrelevant")]
        public string IfIrrelevant { get; set; }
    }
}
