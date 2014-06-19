using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.SignalR
{
    internal static class ResponseExtensions
    {
        public static void Write(this HttpResponse response, ArraySegment<byte> data)
        {
            response.Body.Write(data.Array, data.Offset, data.Count);
        }


        public static Task Flush(this HttpResponse response)
        {
            return response.Body.FlushAsync();
        }
    }
}