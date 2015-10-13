using EHTool.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace EHTool.Core.View
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);
            Init();
        }

        private async void Init()
        {
            Gallery gallery = new Gallery(Shared.Entities.ServerTypes.EHentai);
            listView.ItemsSource = await gallery.GetGalleryList();
        }
    }
}
