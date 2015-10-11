using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EHTool.EHTool.View
{
    public sealed partial class SearchBarControl : UserControl,INotifyPropertyChanged
    {
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<RoutedEventArgs> RefreshClicked;
        public event EventHandler<RoutedEventArgs> SettingClicked;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsMiniAutoSuggestBoxShowed { get; private set; }

        public SearchBarControl()
        {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmitted?.Invoke(sender, args);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshClicked?.Invoke(sender, e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SettingClicked?.Invoke(sender, e);
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            IsMiniAutoSuggestBoxShowed = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMiniAutoSuggestBoxShowed)));
            MiniAutoSuggestBox.Focus(FocusState.Programmatic);
        }

        private void MiniAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsMiniAutoSuggestBoxShowed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMiniAutoSuggestBoxShowed)));
        }
    }
}
