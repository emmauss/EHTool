using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EHTool.Common.Helpers;
using EHTool.EHTool.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace EHTool.EHTool.View
{
    public sealed partial class LoginDialog : ContentDialog
    {
        public bool IsSuccess { get; set; }
        public LoginDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var def = args.GetDeferral();
            errorTB.Visibility = Visibility.Collapsed;
            errorTB2.Visibility = Visibility.Collapsed;
            errorTB3.Visibility = Visibility.Collapsed;
            progressBar.IsIndeterminate = true;
            Login login = new Login(userNameTextBox.Text, passwordTextBox.Password);
            try
            {
                var cookie = await login.GetLoginCookie();
                CookieHelper.Cookie = cookie;
                IsSuccess = true;
                def.Complete();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                errorTB2.Visibility = Visibility.Visible;
            }
            catch (System.Net.WebException)
            {
                errorTB2.Visibility = Visibility.Visible;
            }
            catch (ExHentaiAccessException)
            {
                errorTB.Visibility = Visibility.Visible;
            }
            catch(LoginException)
            {
                errorTB3.Visibility = Visibility.Visible;
            }
            progressBar.IsIndeterminate = false;
        }
        public void WebLoginClicked()
        {
            IsPrimaryButtonEnabled = false;
            webView.Visibility = Visibility.Visible;
            webView.Navigate(new Uri("http://forums.e-hentai.org/index.php?act=Login"));
        }

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string cookie = await sender.InvokeScriptAsync("eval", new List<string>() { "document.cookie" });
            if (!string.IsNullOrEmpty(cookie))
            {
                try
                {
                    Login login = new Login(cookie);
                    CookieHelper.Cookie = await login.GetLoginCookie();
                    IsSuccess = true;
                }
                catch (ExHentaiAccessException)
                {
                    MessageDialog dialog = new MessageDialog(StaticResourceLoader.ExHentaiAccessDialogContent);
                    await dialog.ShowAsync();
                }
                Hide();
            }
        }
    }
}
