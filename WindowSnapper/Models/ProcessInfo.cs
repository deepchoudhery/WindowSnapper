using Newtonsoft.Json;
using System;

namespace WindowSnapper.Models
{
    internal class ProcessInfo
    {
        [JsonConstructor]
        public ProcessInfo(
            string displayName,
            string imagePath,
            int monitorNumber,
            Tuple<int, int> coords)
        {
            DisplayName = displayName;
            IconPath = imagePath;
            MonitorNumber = monitorNumber;
            Coords = coords;
        }

        public string DisplayName { get; set; } 
        public string IconPath { get; set; }
        public int MonitorNumber { get; set; }
        public Tuple<int, int> Coords { get; set; }
    }
}
