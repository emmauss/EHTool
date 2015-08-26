using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.Common.Helpers
{
    public static class FileTypeHelper
    {
        public static List<string> SupportTypeList { get; } = new List<string>()
        {
            ".3gp2",
            ".3gp",
            ".3gpp",
            ".amr",
            ".amv",
            ".asf",
            ".avi",
            ".divx",
            ".dpg",
            ".dpl",
            ".dvr-ms",
            ".evo",
            ".f4v",
            ".flv",
            ".ifo",
            ".k3g",
            ".m1v",
            ".m2t",
            ".m2ts",
            ".m2v",
            ".m4b",
            ".m4p",
            ".m4v",
            ".mkv",
            ".mov",
            ".mp2v",
            ".mp4",
            ".mpe",
            ".mpeg",
            ".mpg",
            ".mpv2",
            ".mts",
            ".nsr",
            ".nsv",
            ".ogm",
            ".ogv",
            ".qt",
            ".ram",
            ".rm",
            ".rmvb",
            ".rpm",
            ".skm",
            ".swf",
            ".tp",
            ".tpr",
            ".ts",
            ".vob",
            ".webm",
            ".wm",
            ".wmp",
            ".wmv",
            ".wtv",
        };

        public static bool CheckFileType(string type)
        {
            return SupportTypeList.Contains(type.ToLower());
        }

    }
}
