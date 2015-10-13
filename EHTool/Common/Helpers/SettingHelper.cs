using System;
using System.Threading.Tasks;
using Windows.Storage;

using static EHTool.Shared.Helpers.JsonHelper;

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
        public static async Task SetFileSetting<T>(string fileName, T data)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var jsStr = ToJson(data);
            await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
        }
        public static async Task<T> GetFileSetting<T>(string fileName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            return jsStr != "" ? FromJson<T>(jsStr) : default(T);
        }

        public static void SetSetting<T>(string settingName, T setValue)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(settingName))
            {
                settings.Values.Remove(settingName);
            }
            settings.Values.Add(settingName, setValue);
        }

        public static T GetSetting<T>(string settingName,T defaultValue = default(T), bool isThrowException = false)
        {
            var settings = ApplicationData.Current.LocalSettings;
            T chackValue = defaultValue;
            if (settings.Values.ContainsKey(settingName))
            {
                chackValue = (T)settings.Values[settingName];
            }
            else
            {
                if (isThrowException)
                {
                    throw new SettingException("Can not Find " + settingName + " value");
                }
            }
            return chackValue;
        }


    }
}
