using System.Collections.Generic;

namespace MidnightClub3AudioModder.Models
{
    public class StreamsData
    {
        public string SourceFile { get; set; }

        public int TotalStreams { get; set; }

        public List<StreamInfo> Streams { get; set; }

        public StreamsData()
        {
            Streams = new List<StreamInfo>();
        }
    }
}
