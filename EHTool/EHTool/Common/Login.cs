using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using static System.Text.RegularExpressions.Regex;

namespace EHTool.EHTool.Common
{
    public sealed class LoginException : Exception
    {
        public LoginException() : base() { }
        public LoginException(string message) : base(message) { }
        public LoginException(string message, Exception innerException) : base(message, innerException) { }
    }
    public sealed class ExHentaiAccessException : Exception
    {
        public ExHentaiAccessException() { }
        public ExHentaiAccessException(string message) : base(message) { }
        public ExHentaiAccessException(string message, Exception inner) : base(message, inner) { }
    }

    public sealed class Login
    {
        private string _userName;
        private string _passWord;

        public Login(string userName, string passWord)
        {
            _userName = userName;
            _passWord = passWord;
        }

        public async Task<string> GetLoginCookie()
        {
            string cookie = await GetCookie();
            string manberid = CheckForMenberID(cookie);
            string passhash = CheckForPassHash(cookie);
            string igneous = await CheckCookieForAccess(manberid, passhash);
            return manberid + ";" + passhash + ";" + igneous;
        }

        private async Task<string> CheckCookieForAccess(string manberid, string passhash)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp("http://exhentai.org/");
            webRequest.Headers["Cookie"] = manberid + ";" + passhash;
            string imgCookie = "";
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                if (webResponse.ContentType == "image/gif")
                {
                    throw new ExHentaiAccessException("No Access");
                }
                imgCookie = webResponse.Headers["Set-Cookie"];
            }
            string igneousRegexPattern = @"igneous=([^;]*)";
            var igneousRegex = Match(imgCookie, igneousRegexPattern);
            var igneous = igneousRegex.Value;
            return igneous;
        }

        private string CheckForMenberID(string cookie)
        {
            string memberidRegexPattern = @"ipb_member_id=([^;]*)";
            var memberidRegex = Match(cookie, memberidRegexPattern);
            if (memberidRegex.Success)
            {
                return memberidRegex.Value;
            }
            else
            {
                throw new LoginException("Login Error");
            }
        }

        private string CheckForPassHash(string cookie)
        {
            string passhashRegexPattern = @"ipb_pass_hash=([^;]*)";
            var passhashRegex = Match(cookie, passhashRegexPattern);
            if (passhashRegex.Success)
            {
                return passhashRegex.Value;
            }
            else
            {
                throw new LoginException("Login Error");
            }
        }


        private async Task<string> GetCookie()
        {
            string postStr = $"UserName={_userName}&PassWord={_passWord}&x=0&y=0";
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            HttpWebRequest loginRequest = WebRequest.CreateHttp("http://forums.e-hentai.org/index.php?act=Login&CODE=01&CookieDate=1");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            string logCookie = "";
            using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            {
                logCookie = logResponse.Headers["Set-Cookie"];
            }
            return logCookie;
        }
    }
}
