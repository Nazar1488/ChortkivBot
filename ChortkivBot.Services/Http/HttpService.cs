using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;

namespace ChortkivBot.Services.Http
{
    public class HttpService : IHttpService
    {
        public async Task<HttpWebResponse> GetRequestAsync(string url, WebHeaderCollection headers, bool keepAlive = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            if (headers != null)
            {
                request.Headers = headers;
            }

            request.KeepAlive = keepAlive;
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public async Task<HttpWebResponse> PostRequestAsync(string url, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            var dataArray = Encoding.ASCII.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataArray.Length;
            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(dataArray, 0, dataArray.Length);
            }

            return (HttpWebResponse)await request.GetResponseAsync();
        }
    }
}
