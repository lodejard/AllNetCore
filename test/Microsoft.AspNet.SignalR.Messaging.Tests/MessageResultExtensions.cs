using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Messaging;

namespace Microsoft.AspNet.SignalR.Tests
{
    public static class MessageResultExtensions
    {
        public static IEnumerable<Message> GetMessages(this MessageResult result)
        {
            foreach (var segment in result.Messages)
            {
                foreach (var message in segment)
                {
                    yield return message;
                }
            }
        }
    }
}
