using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.EHTool.Common
{
    static class StaticResourceLoader
    {
        private static string GetString(string resource)
            => Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString(resource);
        internal static string WebErrorDialogContent => GetString(nameof(WebErrorDialogContent));
        internal static string WebErrorDialogTitle => GetString(nameof(WebErrorDialogTitle));
        internal static string ExHentaiAccessDialogContent => GetString(nameof(ExHentaiAccessDialogContent));
        internal static string WarningString => GetString(nameof(WarningString));
        internal static string DeleteDialogContent => GetString(nameof(DeleteDialogContent));
        internal static string DeleteDialogTitle => GetString(nameof(DeleteDialogTitle));
        internal static string CacheClearDialogContent => GetString(nameof(CacheClearDialogContent));
        internal static string SureString => GetString(nameof(SureString));
        internal static string LogoutString => GetString(nameof(LogoutString));
        internal static string NoHitsFoundDialogContent => GetString(nameof(NoHitsFoundDialogContent));
        internal static string NoImageDialogContent => GetString(nameof(NoImageDialogContent));
        internal static string RemoveDialogContent => GetString(nameof(RemoveDialogContent));
        internal static string RemoveDialogTitle => GetString(nameof(RemoveDialogTitle));
        internal static string YesString => GetString(nameof(YesString));
        internal static string NoString => GetString(nameof(NoString));
        internal static string AddFavorString => GetString(nameof(AddFavorString));
        internal static string RemoveFavorString => GetString(nameof(RemoveFavorString));
    }
}
