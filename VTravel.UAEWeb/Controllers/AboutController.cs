using Microsoft.AspNetCore.Mvc;
using System.Data;
using VTravel.UAEWeb.Models;

namespace VTravel.UAEWeb.Controllers
{
    [ResponseCache(Duration = 30)]
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            try
			{
				ViewData["CanonicalUrl"] = "about/about";
				setViewData("About Us");
                return View();
            }
            catch (Exception ex)
            {
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
        }
        public IActionResult Contact()
        {
			try
			{
				ViewData["CanonicalUrl"] = "about/contact";


				ContactDetails contactDetails = new ContactDetails();
				List<CountryList> countryList = new List<CountryList>();

				setViewData("Contact us");

				countryList = fn_countryList();

				contactDetails.countryList = countryList;


				return View(contactDetails);
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}
        public IActionResult Experience()
        {
			try
			{
				ViewData["CanonicalUrl"] = "about/experience";
				setViewData("Home");
				return View();
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}
        public IActionResult Terms()
        {
			try
			{
				ViewData["CanonicalUrl"] = "about/terms";
				setViewData("Terms & Conditions");
				return View();
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}
		public IActionResult Partner()
		{
            try
			{
				ViewData["CanonicalUrl"] = "about/partner";


				PartnerDetails partnerDetails = new PartnerDetails();
				List<CountryList> countryList = new List<CountryList>();

				countryList = fn_countryList();

				partnerDetails.countryList = countryList;
				return View(partnerDetails);
            }
            catch (Exception ex) 
            {
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
        }
        public IActionResult Cancellation()
        {
			try
			{
				ViewData["CanonicalUrl"] = "about/cancellation";
				setViewData("Cancellations & Refund Policy");
				return View();
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}
		public IActionResult Jobs()
		{
			try
			{
				ViewData["CanonicalUrl"] = "about/jobs";
				setViewData("Jobs and Internships");
				return View();
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}
		public IActionResult Privacy()
		{
			try
			{
				ViewData["CanonicalUrl"] = "about/privacy";
				setViewData("Privacy Policy");
				return View();
			}
			catch (Exception ex)
			{
				General.LogException(ex);
				return Redirect("../Home/Error");
			}
		}

		public IActionResult ThankYou()
		{
			return View();
		}


		public void setViewData(string title)
		{
			MySqlHelper sqlHelper = new MySqlHelper();

			var query = string.Format(@"SELECT 
                                    meta_title,meta_keywords,meta_description                           
                                 from page p  
                                 where p.title='{0}' AND  p.is_active='Y'", title
									);

			DataSet ds = sqlHelper.GetDatasetByMySql(query);


			foreach (DataRow r in ds.Tables[0].Rows)
			{

				ViewData["Title"] = r["meta_title"].ToString();
				ViewData["Keywords"] = r["meta_keywords"].ToString();
				ViewData["Description"] = r["meta_description"].ToString();


			}
		}

		public List<CountryList> fn_countryList()
		{
			MySqlHelper sqlHelper = new MySqlHelper();
			List<CountryList> countryList = new List<CountryList>();
			var countryquery = string.Format(@"SELECT id, iso, name, nicename, iso3, phonecode, regexvalue FROM country_dump WHERE phonecode != 0 AND regexvalue IS NOT NULL;");

			DataSet countryds = sqlHelper.GetDatasetByMySql(countryquery);
			foreach (DataRow dr in countryds.Tables[0].Rows)
			{
				countryList.Add(
					new CountryList
					{
						id = Convert.ToInt32(dr["id"].ToString()),
						phonecode = "+" + dr["phonecode"].ToString(),
						name = dr["name"].ToString(),
						nicename = dr["nicename"].ToString(),
						iso2 = dr["iso"].ToString(),
						iso3 = dr["iso3"].ToString(),
						regexvalue = dr["regexvalue"].ToString(),
					});
			}
			return countryList;
		}

		//Enquiry
		[HttpPost]
		public IActionResult Sendpartner(PartnerEnquiryModel model)
		{
			if (ModelState.IsValid)
			{


				try
				{
					MySqlHelper sqlHelper = new MySqlHelper();


					PartnerDetails partnerDetails = new PartnerDetails();
					List<CountryList> countryList = new List<CountryList>();


					var query = string.Format(@"INSERT INTO partner_enquiry(full_name,	mobile,	email,property_location,details)
                                  VALUES('{0}','{1}','{2}','{3}','{4}');SELECT LAST_INSERT_ID() AS id;"
									 , model.full_name, model.custPhoneCode + ' ' + model.custPhone, model.email, model.property_location, model.details);
					var ds = sqlHelper.GetDatasetByMySql(query);

					query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='partner_enquiry_email_admin'";
					ds = sqlHelper.GetDatasetByMySql(query);
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							var emailBody = ds.Tables[0].Rows[0]["content"].ToString();


							emailBody = emailBody.Replace("#full_name#", model.full_name)
							.Replace("#mobile#", model.custPhoneCode + ' ' + model.custPhone)
							.Replace("#email#", model.email)
							.Replace("#property_location#", model.property_location)
							.Replace("#details#", model.details);



							var subject = General.GetSettingsValue("partner_enquiry_email_subject")
								.Replace("#full_name#", model.full_name).Replace("#property_location#", model.property_location);

							General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("partner_enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("partner_enquiry_from_display_name"));


							countryList = fn_countryList();

							partnerDetails.countryList = countryList;


							TempData["ContactSuccess"] = "Your message has been sent!";
							return View("Partner", partnerDetails);




						}
					}




				}
				catch (Exception ex)
				{

					General.LogException(ex);
				}


			}
			return View();
		}
		//Enquiry
		[HttpPost]
		public IActionResult Contact(ContactModel model)
		{
			//if (ModelState.IsValid)
			//{


			//	try
			//	{
			//		MySqlHelper sqlHelper = new MySqlHelper();


			//		ContactDetails contactDetails = new ContactDetails();
			//		List<CountryList> countryList = new List<CountryList>();

			//		var query = string.Format(@"INSERT INTO contact_enquiry(full_name,	mobile,	email,details)
			//                               VALUES('{0}','{1}','{2}','{3}');SELECT LAST_INSERT_ID() AS id;"
			//						 , model.full_name, model.custPhoneCode + ' ' + model.custPhone, model.email, model.details);
			//		var ds = sqlHelper.GetDatasetByMySql(query);

			//		query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='contact_enquiry_email_admin'";
			//		ds = sqlHelper.GetDatasetByMySql(query);
			//		if (ds.Tables.Count > 0)
			//		{
			//			if (ds.Tables[0].Rows.Count > 0)
			//			{
			//				var emailBody = ds.Tables[0].Rows[0]["content"].ToString();


			//				emailBody = emailBody.Replace("#full_name#", model.full_name)
			//				.Replace("#mobile#", model.custPhoneCode + ' ' + model.custPhone)
			//				.Replace("#email#", model.email)
			//				.Replace("#details#", model.details);



			//				var subject = General.GetSettingsValue("contact_enquiry_email_subject")
			//					.Replace("#full_name#", model.full_name);

			//				General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("contact_enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("contact_enquiry_from_display_name"));


			//				countryList = fn_countryList();

			//				contactDetails.countryList = countryList;


			//				TempData["ContactSuccess"] = "Your message has been sent!";
			//				return View(contactDetails);




			//			}
			//		}




			//	}
			//	catch (Exception ex)
			//	{

			//		General.LogException(ex);
			//	}


			//}
			//return View();
			return Redirect("Home/Error");
		}
	}
}
