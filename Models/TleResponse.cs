using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Models
{
    public class TleResponse
    {
        public TleResponse()
        {
            Member = new List<Tle>();
        }

        public List<Tle> Member { get; set; }
        public Parameters Parameters { get; set; }
        public View View { get; set; }
    }
}
