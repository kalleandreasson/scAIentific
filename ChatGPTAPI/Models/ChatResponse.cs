using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ChatGPTAPI.Models
{
    public class ChatResponse
    {
        public List<ChatMessage>? Messages { get; set; }
    }

    public class ChatMessage
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        public string? Object { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        public string? ThreadId { get; set; }
        public int Role { get; set; }
        public List<Content>? Content { get; set; }
        public string? AssistantId { get; set; }
        public string? RunId { get; set; }
        public List<string>? FileIds { get; set; }
        public object? Metadata { get; set; }
    }

    public class Content
    {
        public string? Type { get; set; }
        public Text? Text { get; set; }
    }

    public class Text
    {
        public string? Value { get; set; }

    }
}