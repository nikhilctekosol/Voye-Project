using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEWeb.Models
{
    public class PartnerEnquiryModel
    {
      
        public string full_name { get; set; }
        public string custPhone { get; set; }
		public string custPhoneCode { get; set; }
		public string email { get; set; }
        public string property_location { get; set; }
        public string details { get; set; }

	}
	public class PartnerDetails
	{
		public List<CountryList> countryList { get; set; }
	}
}
