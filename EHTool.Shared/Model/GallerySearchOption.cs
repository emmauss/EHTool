using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EHTool.Shared.Model
{
    public class GallerySearchOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string KeyWord { get; set; }

        public bool Doujinshi { get; set; } = true;
        public bool Manga { get; set; } = true;
        public bool ArtistCG { get; set; } = true;
        public bool GameCG { get; set; } = true;
        public bool Western { get; set; } = true;
        public bool NonH { get; set; } = true;
        public bool ImageSet { get; set; } = true;
        public bool Cosplay { get; set; } = true;
        public bool AsianPorn { get; set; } = true;
        public bool Misc { get; set; } = true;
        private bool _advSearch = false;
        public bool AdvSearch
        {
            get { return _advSearch; }
            set
            {
                _advSearch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvSearch)));
            }
        }

        public bool SearchName { get; set; } = true;
        public bool SearchTags { get; set; } = true;
        public bool SearchDescription { get; set; } = false;
        public bool SearchTorrentFileNames { get; set; } = false;
        public bool OnlyShowWithTorrents { get; set; } = false;
        public bool SearchLowPowerTags { get; set; } = false;
        public bool SearchDownvotedTags { get; set; } = false;
        public bool ShowExpunged { get; set; } = false;
        private bool _minimumRating;

        public bool MinimumRating
        {
            get { return _minimumRating; }
            set
            {
                _minimumRating = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumRating)));
            }
        }

        public int MinimumRatingValue { get; set; } = 2;

        public GallerySearchOption() : this(null)
        {

        }

        public GallerySearchOption(string keyword)
        {
            KeyWord = keyword;
        }

        public string GetLinkExtension()
        {
            return $"&f_doujinshi={(Doujinshi ? 1 : 0)}&f_manga={(Manga ? 1 : 0)}&f_artistcg={(ArtistCG ? 1 : 0)}&f_gamecg={(GameCG ? 1 : 0)}&f_western={(Western ? 1 : 0)}&f_non-h={(NonH ? 1 : 0)}&f_imageset={(ImageSet ? 1 : 0)}&f_cosplay={(Cosplay ? 1 : 0)}&f_asianporn={(AsianPorn ? 1 : 0)}&f_misc={(Misc ? 1 : 0)}&f_search={KeyWord}&f_apply=Apply+Filter&advsearch={(AdvSearch ? 1 : 0)}{(SearchName ? "&f_sname=on" : null)}{(SearchTags ? "&f_stags=on" : null)}{(SearchDescription ? "&f_sdesc=on" : null)}{(SearchTorrentFileNames ? "&f_storr=on" : null)}{(OnlyShowWithTorrents ? "&f_sto=on" : null)}{(SearchLowPowerTags ? "&f_sdt1=on" : null)}{(SearchDownvotedTags ? "&f_sdt2=on" : null)}{(ShowExpunged ? "&f_sh=on" : null)}{(MinimumRating ? "&f_sr=on" : null)}&f_srdd={MinimumRatingValue}";
        }

    }
}
