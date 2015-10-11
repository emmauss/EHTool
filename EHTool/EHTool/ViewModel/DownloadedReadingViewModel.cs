using EHTool.EHTool.Common;
using EHTool.EHTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace EHTool.EHTool.ViewModel
{
    public class DownloadedReadingViewModel : ReadingViewModel
    {
        private DownloadItemModel _item;
        private ImageListModel _indexItem;
        internal DownloadedReadingViewModel(DownloadItemModel item) : base()
        {
            _item = item;
            LoadList();
        }
        internal DownloadedReadingViewModel(DownloadItemModel item,ImageListModel indexitem):base()
        {
            _indexItem = indexitem;
            _item = item;
            LoadList();
        }
        protected override async Task LoadListOverride()
        {
            try
            {
                if (_item.Items == null)
                {
                    GalleryDetail detail = new GalleryDetail(_item.ID, _item.Token, _item.ServerType);
                    var list = (await detail.GetImagePageList()).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        ImageList.Add(new DownloadedImageModel(_item, list[i].ImagePage, i));
                    }
                }
                else
                {
                    for (int i = 0; i < _item.Items.Count; i++)
                    {
                        ImageList.Add(new DownloadedImageModel(_item, _item.Items[i].Link, i));
                    }
                }
                if (_indexItem != null)
                {
                    _selectedIndex = ImageList.ToList().FindIndex((a) => a.ImagePage == _indexItem.ImagePage);
                }
            }
            catch (System.Net.WebException)
            {
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
        }
    }
}
