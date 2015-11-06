using EHTool.EHTool.Model;
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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using EHTool.Shared.Model;
using Common.Helpers;
using EHTool.Shared.Entities;

using static EHTool.Shared.Entities.SettingNames;
using static Common.Helpers.SettingHelper;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace EHTool.EHTool.View
{
    public sealed partial class AddDownloadDialog : ContentDialog,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private GalleryListModel _item;
        private StorageFolder _folder;
        private bool? _isDefaultDownloadFolder = false;
        public bool IsDownloadInApp { get; set; }
        public string FolderLocation { get; set; } = "Click To Pick";
        public string FolderName { get; set; }
        public string Token { get; set; }
        public bool? IsDefaultDownloadFolder
        {
            get
            {
                return _isDefaultDownloadFolder;
            }
            set
            {
                if (_folder != null)
                {
                    _isDefaultDownloadFolder = value;
                    if (value.HasValue)
                    {
                        SetSetting(DefaultDownloadFolder, value.Value ? StorageApplicationPermissions.FutureAccessList.Add(_folder) : null);
                    }       
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDefaultDownloadFolder)));
            }
        }


        public AddDownloadDialog(GalleryListModel item)
        {
            InitializeComponent();
            Init();
            _item = item;
            FolderName = _item.Title;
            foreach (var charitem in Path.GetInvalidFileNameChars())
            {
                FolderName = FolderName.Replace($"{charitem}", "");
            }
        }

        private async void Init()
        {
            if (GetSetting<string>(DefaultDownloadFolder) != null && StorageApplicationPermissions.FutureAccessList.ContainsItem(GetSetting<string>(DefaultDownloadFolder)))
            {
                _folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(GetSetting<string>(DefaultDownloadFolder));
                FolderLocation = _folder.Path;
                _isDefaultDownloadFolder = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderLocation)));
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_folder == null && !IsDownloadInApp)
            {
                args.Cancel = true;
            }
            else
            {
                var def = args.GetDeferral();
                if (!IsDownloadInApp)
                {
                    var folder = await _folder.CreateFolderAsync(FolderName, CreationCollisionOption.GenerateUniqueName);
                    Token = StorageApplicationPermissions.FutureAccessList.Add(folder);
                }
                def.Complete();
            }
        }

        public async void SetFolderClick()
        {
            FolderPicker picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".jpg");
            picker.ViewMode = PickerViewMode.Thumbnail;
            _folder = await picker.PickSingleFolderAsync();
            if (_folder != null)
            {
                FolderLocation = _folder.Path;
                _isDefaultDownloadFolder = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDefaultDownloadFolder)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderLocation)));
            }
        }
    }
}
