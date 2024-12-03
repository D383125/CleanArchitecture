using Application.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.MockServices
{
    public class MockChatClient : IChatClient
    {                
        async IAsyncEnumerable<string> IChatClient.CompleteChatStreaming(IDictionary<string, string> messages, string model)
        {
            await Task.Yield();

            yield return "Hello";
            yield return "How are you?";
        }
    }
}
