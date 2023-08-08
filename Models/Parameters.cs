using MyConsoleApp.Consts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Models
{
    public class Parameters
    {
        public string Search { get; set; }
        public SortTypeEnum Sort { get; set; } = SortTypeEnum.name;
        [JsonProperty("sort-dir")]
        public SortDirTypeEnum SortDir { get; set; } = SortDirTypeEnum.asc;
        public int Page 
        { 
            get => _page;
            set 
            {
                if (value > 0)
                    _page = value;
                else
                    _page = 1;
            }
        }
        [JsonProperty("page-size")]
        public int PageSize 
        { 
            get => _pageSize;
            set 
            {
                if (value < 1 || value > 100)
                    _pageSize = 20;
                else
                    _pageSize = value;
            } 
        }

        private int _page = 1;
        private int _pageSize = 20;
    }
}
