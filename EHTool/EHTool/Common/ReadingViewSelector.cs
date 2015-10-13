﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EHTool.Shared.Entities;

namespace EHTool.EHTool.Common
{

    public class ReadingViewSelector : DataTemplateSelector
    {
        public DataTemplate CommonView { get; set; }
        public DataTemplate FlipBookView { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SettingHelper.GetSetting<bool>(SettingNames.IsReadingDoublePage) ? FlipBookView : CommonView;
        }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return this.SelectTemplateCore(item);
        }
    }
}
