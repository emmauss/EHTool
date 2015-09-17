using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.EHTool.Model
{
    [DataContract]
    public class LocalFolderModel
    {
        [DataMember]
        public string FolderToken { get; internal set; }
        [DataMember]
        public string FolderName { get; internal set; }
    }
}
