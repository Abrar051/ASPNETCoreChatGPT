using System.Text.Json.Serialization;

namespace ASPNETCoreChatGPT.Models
{
    public class HuggingFaceResponse
    {
        [JsonPropertyName("sequence")]
        public string Sequence { get; set; }

        [JsonPropertyName("labels")]
        public string[] Labels { get; set; }

        [JsonPropertyName("scores")]
        public float[] Scores { get; set; }
    }
}
