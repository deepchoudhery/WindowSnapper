using Newtonsoft.Json;
using System.Collections.Generic;

namespace WindowSnapper.Models
{
    internal class Profile
    {
        [JsonConstructor]
        public Profile()
        {
            
        }

        public List<ProcessInfo> Processes { get; set; }

        public bool Default { get; set; }

        public string Name { get; set; }

    }
}
