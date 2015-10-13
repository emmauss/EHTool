using PCLStorage;
using System;
using System.Threading.Tasks;

using static EHTool.Shared.Helpers.JsonHelper;
using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;

namespace Common.Helpers
{
    public class SettingException : Exception
    {
        public SettingException() { }
        public SettingException(string message) : base(message) { }
        public SettingException(string message, Exception inner) : base(message, inner) { }
    }
    public static class SettingHelper
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static async Task SetFileSetting<T>(string fileName, T data)
        {
            var file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var jsStr = ToJson(data);
            await file.WriteAllTextAsync(jsStr);
        }
        public static async Task<T> GetFileSetting<T>(string fileName)
        {
            var file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var jsStr = await file.ReadAllTextAsync();
            return jsStr != "" ? FromJson<T>(jsStr) : default(T);
        }
        public static void SetSetting<T>(string settingName, T setValue) => AppSettings.AddOrUpdateValue(settingName, setValue);

        public static T GetSetting<T>(string settingName, T defaultValue = default(T)) => AppSettings.GetValueOrDefault(settingName, defaultValue);
    }
}
