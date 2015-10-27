using EHTool.Core.ViewModel;
using EHTool.Shared;
using EHTool.Shared.Entities;
using EHTool.Shared.Helpers;
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
        }
        private void ListView_Refreshing(object sender, EventArgs e)
        {

        }
    }
}
