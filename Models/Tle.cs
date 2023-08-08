using MyConsoleApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Models
{
    public class Tle
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        [JsonProperty("@type")]
        public string Type { get; set; }
        public long SatelliteId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }
}
