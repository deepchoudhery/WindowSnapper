using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WindowSnapper.Models
{
    public class Profile
    {
        public Profile(string name, IDictionary<string, ProcessInfo> processes, bool defaultProfile)
        {
            Name = name;
            Default = defaultProfile;
            Processes = processes;
        }

        public IDictionary<string, ProcessInfo> Processes { get; set; }

        public bool Default { get; set; }

        public string Name { get; set; }
    }
}
