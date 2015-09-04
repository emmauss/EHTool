using static Common.Helpers.SettingHelpers;
using static System.Text.RegularExpressions.Regex;

namespace EHTool.Common.Helpers
{
    public static class CookieHelper
    {
        public static string Unconfig =>
            ";uconfig=tl_m-uh_y-rc_0-cats_0-xns_0-ts_l-tr_2-prn_y-dm_t-ar_0-rx_0-ry_0-ms_n-mt_n-cs_a-to_a-pn_0-sc_0-sa_y-oi_n-qb_n-tf_n-hp_-hk_-xl_";


        public static string Cookie
        {
            get
            {
                return GetSetting<string>("cookie");
            }
            set
            {
                SetSetting("cookie", value);
            }
        }

        public static bool CheckCookie()
        {
            if (Cookie == null)
            {
                return false;
            }
            const string memberidRegex = @"ipb_member_id=([^;]*)";
            const string passhashRegex = @"ipb_pass_hash=([^;]*)";
            const string igneousRegex = @"igneous=([^;]*)";
            var memberidStr = Match(Cookie, memberidRegex);
            var passhashStr = Match(Cookie, passhashRegex);
            var igneousStr = Match(Cookie, igneousRegex);
            return memberidStr.Success && passhashStr.Success && igneousStr.Success;
        }
    }
}
