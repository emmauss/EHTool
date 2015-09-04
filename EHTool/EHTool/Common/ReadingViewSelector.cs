using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EHTool.EHTool.Common
{

    public class ReadingViewSelector : DataTemplateSelector
    {
        public DataTemplate CommonView { get; set; }
        public DataTemplate FlipBookView { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SettingHelpers.GetSetting<bool>("IsReadingDoublePage") ? FlipBookView : CommonView;
        }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return this.SelectTemplateCore(item);
        }
    }
}
