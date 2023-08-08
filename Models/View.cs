using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Models
{
    public class View
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        [JsonProperty("@type")]
        public string Type { get; set; }
        public string First { get; set; }
        public string Next { get; set; }
        public string Last { get; set; }
    }
}
