using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EHTool.Shared.Entities;

using static EHTool.Shared.Helpers.JsonHelper;
using static Common.Helpers.SettingHelper;
using static System.Text.RegularExpressions.Regex;
using EHTool.Shared.Model;

namespace EHTool.Shared.Helpers
{
    internal static class CookieHelper
    {
        private const string DEFAULT_UNCONFIG = ";uconfig=tl_m-uh_y-tr_2-ts_l-dm_t-ar_0-xns_0-rc_0-rx_0-ry_0-cs_a-to_a-pn_0-sc_0-sa_y-oi_n-qb_n-tf_n-hp_-hk_-cats_0-prn_y-ms_n-mt_n-xl_";
        public static string Unconfig => GetSetting(SettingNames.Unconfig, DEFAULT_UNCONFIG);

        internal static async Task UpdateUnconfig(List<LanguageModel> languageList)
        {
            string unconfig = DEFAULT_UNCONFIG;
            foreach (var item in languageList)
            {
                unconfig += item.Original && item.ID != 0 ? $"{item.OriginalID}x" : null;
                unconfig += item.Translated ? $"{item.TranslatedID}x" : null;
                unconfig += item.Rewrite ? $"{item.RewriteID}x" : null;
            }
            if (unconfig[unconfig.Length - 1] == 'x')//Excluded language changed
            {
                unconfig = unconfig.Remove(unconfig.Length - 2);
            }
            SetSetting(SettingNames.Unconfig, unconfig);
            await SetFileSetting("LanguageSetting.json", languageList);
        }

        internal static async Task<IEnumerable<LanguageModel>> GetLanguageSetting()
        {
            var list = await GetFileSetting<List<LanguageModel>>("LanguageSetting.json");
            return list != default(List<LanguageModel>) ? list : new List<LanguageModel>()
            {
                new LanguageModel("Japanese",0,false,false,false),
                new LanguageModel("English",1,false,false,false),
                new LanguageModel("Chinese",10,false,false,false),
                new LanguageModel("Dutch",20,false,false,false),
                new LanguageModel("French",30,false,false,false),
                new LanguageModel("German",40,false,false,false),
                new LanguageModel("Hungarian",50,false,false,false),
                new LanguageModel("Italian",60,false,false,false),
                new LanguageModel("Korean",70,false,false,false),
                new LanguageModel("Polish",80,false,false,false),
                new LanguageModel("Portuguese",90,false,false,false),
                new LanguageModel("Russian",100,false,false,false),
                new LanguageModel("Spanish",110,false,false,false),
                new LanguageModel("Thai",120,false,false,false),
                new LanguageModel("Vietnamese",130,false,false,false),
                new LanguageModel("N/A",254,false,false,false),
                new LanguageModel("Other",255,false,false,false),
            };
        }

        //-xl_ : excluded language 
        internal static string Cookie
        {
            get
            {
                return GetSetting<string>(SettingNames.Cookie);
            }
            set
            {
                SetSetting(SettingNames.Cookie, value);
            }
        }

        internal static bool CheckCookie()
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
