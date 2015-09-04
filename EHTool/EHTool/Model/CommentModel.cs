
using System.ComponentModel;
using Windows.UI.Xaml;

namespace EHTool.EHTool.Model
{
    public class CommentModel : INotifyPropertyChanged
    {
        public CommentModel()
        {

            int b = (int)Window.Current.Bounds.Width / 200;
            MaxWidth = Window.Current.Bounds.Width / b - 10d;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            int b = (int)Window.Current.Bounds.Width / 200;
            MaxWidth = e.Size.Width / b - 10d;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxWidth)));
        }
        public double MaxWidth { get; private set; }
        public string Base { get; internal set; }
        public string Content { get; internal set; }
        public string Poster { get; internal set; }
        public string Score { get; internal set; }
    }
}
