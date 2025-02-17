namespace VTravel.UAEWeb.Models
{
	public class ContactModel
	{
		public string full_name { get; set; }
		public string custPhone { get; set; }
		public string custPhoneCode { get; set; }
		public string email { get; set; }
		public string details { get; set; }
	}

	public class ContactDetails
	{
		public List<CountryList> countryList { get; set; }
	}
}
