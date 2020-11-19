using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutoDJVUConverter
{
    public class Config : ISerializable
    {
        public Config()
        {
        }

        public string Hotfolder { get; set; } = string.Empty;
        public int DPI { get; set; }
        public string ProcessedCSVs { get; set; } = string.Empty;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Hotfolder", Hotfolder, typeof(string));
            info.AddValue("DPI", DPI, typeof(int));
            info.AddValue("ProcessedCSVs", ProcessedCSVs, typeof(string));
        }
    }
}
