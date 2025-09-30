using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FischbachTicketing.Common
{
    public class SearchFilter
    {
        public string groupOp { get; set; }
        public List<Rule> rules { get; set; }
    }

    public class Rule
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }
}
