using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightClub3AudioModder.Helpers
{
    /// <summary>
    /// Simple JSON parser for .NET Framework 4.7.2 without external dependencies
    /// </summary>
    public static class SimpleJsonParser
    {
        public static T Deserialize<T>(string json) where T : class, new()
        {
            if (typeof(T) == typeof(Models.StreamsData))
            {
                return ParseStreamsData(json) as T;
            }
            return null;
        }

        private static Models.StreamsData ParseStreamsData(string json)
        {
            var data = new Models.StreamsData();
            
            // Parse source_file
            var sourceFileMatch = Regex.Match(json, @"""source_file""\s*:\s*""([^""]+)""");
            if (sourceFileMatch.Success)
                data.SourceFile = sourceFileMatch.Groups[1].Value;

            // Parse total_streams
            var totalStreamsMatch = Regex.Match(json, @"""total_streams""\s*:\s*(\d+)");
            if (totalStreamsMatch.Success)
                data.TotalStreams = int.Parse(totalStreamsMatch.Groups[1].Value);

            // Parse streams array
            var streamsMatch = Regex.Match(json, @"""streams""\s*:\s*\[(.*)\]", RegexOptions.Singleline);
            if (streamsMatch.Success)
            {
                string streamsContent = streamsMatch.Groups[1].Value;
                var streamObjects = SplitJsonObjects(streamsContent);

                foreach (var streamJson in streamObjects)
                {
                    var stream = ParseStreamInfo(streamJson);
                    if (stream != null)
                        data.Streams.Add(stream);
                }
            }

            return data;
        }

        private static Models.StreamInfo ParseStreamInfo(string json)
        {
            var stream = new Models.StreamInfo();

            // Parse name
            var nameMatch = Regex.Match(json, @"""name""\s*:\s*""([^""]+)""");
            if (nameMatch.Success)
                stream.Name = nameMatch.Groups[1].Value;

            // Parse offset
            var offsetMatch = Regex.Match(json, @"""offset""\s*:\s*""(0x[0-9A-Fa-f]+)""");
            if (offsetMatch.Success)
                stream.Offset = offsetMatch.Groups[1].Value;

            // Parse offset_decimal
            var offsetDecMatch = Regex.Match(json, @"""offset_decimal""\s*:\s*(\d+)");
            if (offsetDecMatch.Success)
                stream.OffsetDecimal = long.Parse(offsetDecMatch.Groups[1].Value);

            // Parse size
            var sizeMatch = Regex.Match(json, @"""size""\s*:\s*(\d+)");
            if (sizeMatch.Success)
                stream.Size = long.Parse(sizeMatch.Groups[1].Value);

            // Parse size_hex
            var sizeHexMatch = Regex.Match(json, @"""size_hex""\s*:\s*""(0x[0-9A-Fa-f]+)""");
            if (sizeHexMatch.Success)
                stream.SizeHex = sizeHexMatch.Groups[1].Value;

            return stream;
        }

        private static List<string> SplitJsonObjects(string jsonArray)
        {
            var objects = new List<string>();
            int braceCount = 0;
            int startIndex = -1;

            for (int i = 0; i < jsonArray.Length; i++)
            {
                char c = jsonArray[i];

                if (c == '{')
                {
                    if (braceCount == 0)
                        startIndex = i;
                    braceCount++;
                }
                else if (c == '}')
                {
                    braceCount--;
                    if (braceCount == 0 && startIndex != -1)
                    {
                        objects.Add(jsonArray.Substring(startIndex, i - startIndex + 1));
                        startIndex = -1;
                    }
                }
            }

            return objects;
        }
    }
}
