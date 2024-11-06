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
				setViewData("Contact us");
				return View();
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
                ViewData["CanonicalUrl"] = "page/partner-with-us";
                return View();
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



		//Enquiry
		[HttpPost]
		public IActionResult Sendpartner(PartnerEnquiryModel model)
		{
			if (ModelState.IsValid)
			{


				try
				{
					MySqlHelper sqlHelper = new MySqlHelper();



					var query = string.Format(@"INSERT INTO partner_enquiry(full_name,	mobile,	email,property_location,details)
                                  VALUES('{0}','{1}','{2}','{3}','{4}');SELECT LAST_INSERT_ID() AS id;"
									 , model.full_name, model.mobile, model.email, model.property_location, model.details);
					var ds = sqlHelper.GetDatasetByMySql(query);

					query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='partner_enquiry_email_admin'";
					ds = sqlHelper.GetDatasetByMySql(query);
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							var emailBody = ds.Tables[0].Rows[0]["content"].ToString();


							emailBody = emailBody.Replace("#full_name#", model.full_name)
							.Replace("#mobile#", model.mobile)
							.Replace("#email#", model.email)
							.Replace("#property_location#", model.property_location)
							.Replace("#details#", model.details);



							var subject = General.GetSettingsValue("partner_enquiry_email_subject")
								.Replace("#full_name#", model.full_name).Replace("#property_location#", model.property_location);

							General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("partner_enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("partner_enquiry_from_display_name"));

							TempData["ContactSuccess"] = "Your message has been sent!";
							return View("Partner");




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
			if (ModelState.IsValid)
			{


				try
				{
					MySqlHelper sqlHelper = new MySqlHelper();



					var query = string.Format(@"INSERT INTO contact_enquiry(full_name,	mobile,	email,details)
                                  VALUES('{0}','{1}','{2}','{3}');SELECT LAST_INSERT_ID() AS id;"
									 , model.full_name, model.mobile, model.email, model.details);
					var ds = sqlHelper.GetDatasetByMySql(query);

					query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='contact_enquiry_email_admin'";
					ds = sqlHelper.GetDatasetByMySql(query);
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							var emailBody = ds.Tables[0].Rows[0]["content"].ToString();


							emailBody = emailBody.Replace("#full_name#", model.full_name)
							.Replace("#mobile#", model.mobile)
							.Replace("#email#", model.email)
							.Replace("#details#", model.details);



							var subject = General.GetSettingsValue("contact_enquiry_email_subject")
								.Replace("#full_name#", model.full_name);

							General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("contact_enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("contact_enquiry_from_display_name"));

							TempData["ContactSuccess"] = "Your message has been sent!";
							return View();




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
	}
}
