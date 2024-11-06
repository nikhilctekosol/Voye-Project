using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEAdmin.Models
{
    public class DocType
    {
        public int id { get; set; }
        public string doc_type_name { get; set; }
        public string description { get; set; }
        public string doc_abbr { get; set; }
        public string default_doc { get; set; }
    }
  
}
