using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using EHTool.Core.Control;
using Xamarin.Forms;
using EHTool.Droid.Renderer;

[assembly: ExportRenderer(typeof(TextBlock), typeof(TextBlockRenderer))]
namespace EHTool.Droid.Renderer
{
    public class TextBlockRenderer:LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var control = e.NewElement as TextBlock;
            Control.SetMaxLines(control.MaxLine);
        }
    }
}