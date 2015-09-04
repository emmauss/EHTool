using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Converters;

namespace Common.Helpers
{
    public static class HttpHelper
    {

        public static async Task<byte[]> GetByteArrayWith(string method, string link, string cookie, string referer)
        {
            var req = WebRequest.CreateHttp(link);
            req.Headers["Cookie"] = cookie;
            req.Headers["Referer"] = referer;
            req.Method = method;
            var res = await req.GetResponseAsync();
            using (var resStream = res.GetResponseStream())
            {
                return await Converter.StreamToBytes(resStream);
            }
        }

        public static async Task<byte[]> GetByteArray(string link, string cookie)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                return await client.GetByteArrayAsync(link);
            }
        }

        public async static Task<string> GetStringWithPostString(string uriString, string postStr, string contentType)
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

        public async static Task<string> GetStringWithCookie(string uriString, string cookie)
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
