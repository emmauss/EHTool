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
            progressBar.IsIndeterminate = true;
            Login login = new Login(userNameTextBox.Text, passwordTextBox.Password);
            try
            {
                var cookie = await login.GetLoginCookie();
                CookieHelper.Cookie = cookie;
                IsSuccess = true;
                def.Complete();
            }
            catch (ExHentaiAccessException)
            {
                errorTB.Visibility = Visibility.Visible;
            }
            progressBar.IsIndeterminate = false;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }
    }
}
