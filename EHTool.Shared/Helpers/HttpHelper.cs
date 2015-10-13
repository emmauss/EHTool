using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.Shared.Helpers
{
    static class HttpHelper
    {

        internal static async Task<byte[]> GetByteArrayWith(string method, string link, string cookie, string referer)
        {
            var req = WebRequest.CreateHttp(link);
#if WINDOWS_UWP
            req.Headers["Cookie"] = cookie;
            req.Headers["Referer"] = referer;
#else
            //if (!string.IsNullOrEmpty(cookie))
            //{
            //    req.Headers[HttpRequestHeader.Cookie] = cookie;
            //}
            //req.Headers[HttpRequestHeader.Referer] = referer;
#endif
            req.Method = method;
            var res = await req.GetResponseAsync();
            using (var resStream = res.GetResponseStream())
            {
                return await ConvertHelper.StreamToBytes(resStream);
            }
        }

        internal static async Task<byte[]> GetByteArray(string link, string cookie)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                return await client.GetByteArrayAsync(link);
            }
        }

        internal async static Task<string> GetStringWithPostString(string uriString, string postStr, string contentType)
        {
            string returnStr;
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            HttpWebRequest req = WebRequest.CreateHttp(uriString);
            req.Method = "POST";
            req.ContentType = contentType;
            using (Stream stream = await req.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            using (var res = await req.GetResponseAsync() as HttpWebResponse)
            {
                using (var getcontent = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    returnStr = getcontent.ReadToEnd();
                }
            }
            return returnStr;
        }

        internal async static Task<string> GetStringWithCookie(string uriString, string cookie)
        {
            string returnStr;
            HttpWebRequest webRequest = WebRequest.CreateHttp(uriString);
            webRequest.Headers["Cookie"] = cookie;
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                using (var getContent = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    returnStr = await getContent.ReadToEndAsync();
                }
            }
            return returnStr;
        }
    }
}
