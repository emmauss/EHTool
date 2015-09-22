using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.EHTool.Model
{
    [DataContract]
    public class LanguageModel : INotifyPropertyChanged
    {
        public LanguageModel(string name,int id,bool original,bool translated,bool rewrite)
        {
            Name = name;
            ID = id;
            Original = original;
            Translated = translated;
            Rewrite = rewrite;
        }
        [DataMember]
        public string Name { get; internal set; }
        [DataMember]
        public int ID { get; internal set; }
        [DataMember]
        public bool Original { get; set; }
        [IgnoreDataMember]
        public string OriginalID => $"{ID}";
        [DataMember]
        public bool Translated { get; set; }
        [IgnoreDataMember]
        public string TranslatedID => $"{ID + 1024}";
        [DataMember]
        public bool Rewrite { get; set; }
        [IgnoreDataMember]
        public string RewriteID => $"{ID + 2048}";
        [IgnoreDataMember]
        public bool All
        {
            get
            {
                return Original && Translated && Rewrite;
            }
            set
            {
                Original = value;
                Translated = value;
                Rewrite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Original)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Translated)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rewrite)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
