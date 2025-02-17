using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEWeb.Models
{
    public class PropertyViewModel
    {
       public Property property { get; set; }
       public string terms { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string short_desc { get; set; }
        public string banner_url { get; set; }
        public string long_desc { get; set; }


        public List<Property> propertyList { get; set; }
        public List<Property> promoPropertyList { get; set; }
		public List<CountryList> countryList { get; set; }
	}


	public class CountryList
	{
		public int id { get; set; }
		public string phonecode { get; set; }
		public string name { get; set; }
		public string nicename { get; set; }
		public string regexvalue { get; set; }
		public string iso2 { get; set; }
		public string iso3 { get; set; }
	}

}
