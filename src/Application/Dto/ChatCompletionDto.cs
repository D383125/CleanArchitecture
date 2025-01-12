using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ChatCompletionDto
    {
        public string Model { get; set; } = "gpt-4o"; // Default value, can be overridden
        public List<ChatCompletionMessageDto> Messages { get; set; } = [];
    }

    public class ChatCompletionMessageDto
    {
        public string Role { get; set; } // Values: "system", "user", "assistant"
        public string Content { get; set; }
    }
}
