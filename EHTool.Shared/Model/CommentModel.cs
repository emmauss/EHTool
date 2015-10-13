using System.ComponentModel;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#endif

namespace EHTool.Shared.Model
{
    public class CommentModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
#if WINDOWS_UWP
        public CommentModel()
        {

            int b = (int)Window.Current.Bounds.Width / 200;
            MaxWidth = Window.Current.Bounds.Width / b - 10d;
            Window.Current.SizeChanged += Current_SizeChanged;
        }


        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            int b = (int)Window.Current.Bounds.Width / 200;
            MaxWidth = e.Size.Width / b - 10d;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxWidth)));
        }
#endif
        public double MaxWidth { get; private set; }
        public string Base { get; internal set; }
        public string Content { get; internal set; }
        public string Poster { get; internal set; }
        public string Score { get; internal set; }
    }
}
