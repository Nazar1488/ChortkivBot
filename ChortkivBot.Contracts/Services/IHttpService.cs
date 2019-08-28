using System.Net;
using System.Threading.Tasks;

namespace ChortkivBot.Contracts.Services
{
    public interface IHttpService
    {
        Task<HttpWebResponse> GetRequestAsync(string url, WebHeaderCollection headers,
            bool keepAlive = true);

        Task<HttpWebResponse> PostRequestAsync(string url, string data);
    }
}