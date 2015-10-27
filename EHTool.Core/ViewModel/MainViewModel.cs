using EHTool.Shared.Entities;
using EHTool.Shared.ViewModelBase;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EHTool.Core.ViewModel
{
    public class MainViewModel : GalleryViewModel
    {
        private Page _page;
        public MainViewModel(Page page) : base() { _page = page; }
        public MainViewModel(ServerTypes type, Page page) : base(type) { _page = page; }
        protected override Task LoadOtherAsyncOverride()
        {
            return Task.Delay(1);
        }

        protected override void OnExHentaiAccessExceptionOverride()
        {
            _page.DisplayAlert("Error", "You don't have access to exhentai", "ok");
        }

        protected override void OnNoHitOverride()
        {
            _page.DisplayAlert("Warning", "No hits fond", "ok");
        }

        protected override void OnWebErrorOverride()
        {
            _page.DisplayAlert("Error", "Can not connect to the server", "ok");
        }
    }
}
