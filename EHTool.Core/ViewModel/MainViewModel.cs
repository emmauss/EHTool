using EHTool.Shared.ViewModelBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.Core.ViewModel
{
    public class MainViewModel : GalleryViewModel
    {
        protected override Task LoadOtherAsyncOverride()
        {
            throw new NotImplementedException();
        }

        protected override void OnExHentaiAccessOverride()
        {
            throw new NotImplementedException();
        }

        protected override void OnNoHitOverride()
        {
            throw new NotImplementedException();
        }

        protected override void OnWebErrorOverride()
        {
            throw new NotImplementedException();
        }
    }
}
