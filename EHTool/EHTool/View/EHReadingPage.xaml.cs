using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Common.Helpers;
using EHTool.EHTool.Model;
using EHTool.EHTool.ViewModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EHTool.EHTool.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EHReadingPage : Page ,INotifyPropertyChanged
    {
        #region TitleBarMember
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarHeight"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarPadding"));
        }
        public void FullScreenClick()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreen)
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FullScreenButtonContent"));
        }

        #region Properties        
        public string FullScreenButtonContent
        {
            get
            {
                return ApplicationView.GetForCurrentView().IsFullScreenMode ? "\uE73F" : "\uE1D9";
            }
        }
        public Thickness TitleBarPadding
        {
            get
            {
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    return new Thickness() { Left = CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset, Right = CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset };
                }
                else
                {
                    return new Thickness() { Left = CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset, Right = CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset };
                }
            }
        }
        public double TitleBarHeight
        {
            get
            {
                return CoreApplication.GetCurrentView().TitleBar.Height;
            }
        }
        #endregion
        #endregion
        public ReadingViewModel ReadingVM { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsPaneOpen { get; set; }
        
        Windows.System.Display.DisplayRequest req = new Windows.System.Display.DisplayRequest();
        public EHReadingPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().ShowAsync();
            }
            req.RequestRelease();
            base.OnNavigatedFrom(e);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBarRect);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
            req.RequestActive();
            ReadingVM = e.Parameter as ReadingViewModel;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReadingVM)));
            base.OnNavigatedTo(e);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Right:
                    if (ReadingVM.ReadingDirection == FlowDirection.LeftToRight)
                    {
                        ReadingVM.ToNext();
                    }
                    else
                    {
                        ReadingVM.ToPrev();
                    }
                    e.Handled = true;
                    break;
                case Windows.System.VirtualKey.Left:
                    if (ReadingVM.ReadingDirection == FlowDirection.LeftToRight)
                    {
                        ReadingVM.ToPrev();
                    }
                    else
                    {
                        ReadingVM.ToNext();
                    }
                    e.Handled = true;
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        public async void BackClick()
        {
            await ReadingVM.CancelTask();
            Frame.GoBack();
        }


        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShowControlPanel.Begin();
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideControlPanel.Begin();
        }

        public void SettingButtonClick()
        {
            IsPaneOpen = !IsPaneOpen;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaneOpen)));
        }

        public async void RefreshClick()
        {
            await ReadingVM.Refresh();
        }

        private void Grid_PointerPressed(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (ControlPanel.Opacity == 0d)
            {
                ShowControlPanel.Begin();
            }
            else
            {
                HideControlPanel.Begin();
            }
        }
    }
}
