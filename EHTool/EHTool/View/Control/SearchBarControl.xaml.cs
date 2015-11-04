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
using static Common.Helpers.SettingHelper;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EHTool.EHTool.View
{
    public sealed partial class SearchBarControl : UserControl,INotifyPropertyChanged
    {
        private List<string> _searchHistory = new List<string>();
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<RoutedEventArgs> RefreshClicked;
        public event EventHandler<RoutedEventArgs> SettingClicked;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsMiniAutoSuggestBoxShowed { get; private set; }
        public bool AllowHistory { get; set; } = true;

        public SearchBarControl()
        {
            this.InitializeComponent();
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(sender.Text) && !_searchHistory.Contains(sender.Text))
            {
                _searchHistory.Add(sender.Text);
                await SetFileSetting("SearchHistory.json", _searchHistory);
            }
            QuerySubmitted?.Invoke(sender, args);
            if (MiniAutoSuggestBox.FocusState != FocusState.Unfocused)
            {
                MiniAutoSuggestBox.Focus(FocusState.Unfocused);
            }
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
            MiniAutoSuggestBox.Focus(FocusState.Pointer);
        }

        private void MiniAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsMiniAutoSuggestBoxShowed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMiniAutoSuggestBoxShowed)));
        }

        private async void autoSuggestBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AllowHistory)
            {
                if (_searchHistory.Count == 0)
                {
                    _searchHistory = await GetFileSetting<List<string>>("SearchHistory.json");
                }
                (sender as AutoSuggestBox).ItemsSource = _searchHistory;
                (sender as AutoSuggestBox).IsSuggestionListOpen = true;
            }
        }

        private void autoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (AllowHistory && _searchHistory.Count != 0)
            {
                sender.ItemsSource = _searchHistory.Where(s => s.Contains(sender.Text));
                sender.IsSuggestionListOpen = true;
            }
        }
    }
}
