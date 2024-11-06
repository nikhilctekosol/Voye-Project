using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using VTravel.UAEWeb.Models;

namespace VTravel.UAEWeb.Controllers
{
    public class PropertyController : Controller
    {
        public IActionResult Detail(string id)
        {
			PropertyViewModel propertyViewModel = new PropertyViewModel();
            try
            {
				var idArray = id.Split('-');
				var encodedId = idArray[idArray.Length - 1];
				var decodedId = General.DecodeString(encodedId);

				propertyViewModel.property = new Property();

				//get top 10 property list grouped by tags in each group for home displayabl tags

				MySqlHelper sqlHelper = new MySqlHelper();

				var query = string.Format(@"SET SESSION group_concat_max_len = 100000;
                                     SELECT 
                                      JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'reserveAllowed',p.reserve_allowed,'reserveAlert',p.reserve_alert,'thumbnail',p.thumbnail,'longDescription',p.long_description
                                    ,'metaTitle',p.meta_title,'metaKeywords',p.meta_keywords,'metaDescription',p.meta_description,'bookingUrl',p.booking_url,'sellOnline',p.sell_online                                        
                                    ,'propertyTypeName',pt.type_name,'city',c.city_name,'state',s.state_name,'country',cn.country_name
                                    ,'maxOccupancy',p.max_occupancy,'roomCount',p.room_count,'bathroomCount',p.bathroom_count
                                    ,'latitude',p.latitude,'longitude',p.longitude
                                    ,'priceList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                    ']')
                             AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order)
                                   
                            ,'imageList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',im.id,'url',im.url,'image_alt',im.image_alt,'sortOrder',im.sort_order,'categoryid',IFNULL(im.category,0),'subcategoryid',IFNULL(im.subcategory,0),'roomid',IFNULL(im.room,0)
                                        ,'category',IFNULL(c.category_name,''),'subcategory',IFNULL(sc.subcategory_name,''),'room',IFNULL(r.title,''))),
                                    ']')
                             AS JSON) from property_image im
                             left join img_category c on c.id = im.category 
                             left join img_subcategory sc on sc.id = im.subcategory
                             left join room r on r.id = im.room where im.property_id = p.id ORDER BY im.sort_order) 

                            ,'attributeList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pa.id,'longDescription',pa.long_description,'attributeName',at.attribute_name)),
                                    ']')
                             AS JSON) from property_attribute pa INNER JOIN attribute at ON pa.attribute_id=at.id where pa.property_id = p.id ORDER BY at.sort_order)
 
                           ,'amenityList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',am.id,'amenityName',am.amenity_name,'image1',am.image1)),
                                    ']')
                             AS JSON) from property_amenity pam INNER JOIN amenity am ON pam.amenity_id=am.id AND am.is_active='Y' where pam.property_id = p.id ORDER BY am.sort_order)
                            ,'contactList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',ac.id,'name',ac.contact_name,'contact',ac.contact_no)),
                                    ']')
                             AS JSON)
                             from property_contacts pc INNER JOIN alternate_contacts ac ON ac.id = pc.contact_id where pc.property_id = p.id)
                             ,'roomList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',r.id,'title',r.title,'noofrooms', r.noofrooms)),
                                    ']')
                             AS JSON)
                             from room r where r.property_id = p.id and r.is_active = 'Y')
                             ,'attractionList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',a.id,'attraction_name',a.attraction_name,'location', a.location,'image', a.image,'distance', a.distance)),
                                    ']')
                             AS JSON)
                             from nearest_attraction a where a.property_id = p.id and a.is_active = 'Y'))
                             
 
                           
                          
                             from property p 
                             INNER JOIN property_type pt ON p.property_type_id=pt.id
                             INNER JOIN city c ON p.city=c.city_code 
                             INNER JOIN state s ON p.state=s.state_code 
                             INNER JOIN country cn ON p.country=cn.country_code  
                             where p.id={0} AND  p.is_active='Y' AND p.property_status='ACTIVE'", Convert.ToInt32(decodedId)
								);

				DataSet ds = sqlHelper.GetDatasetByMySql(query);

				if (ds.Tables.Count > 0)
				{
					foreach (DataRow r in ds.Tables[0].Rows)
					{

						propertyViewModel.property = JsonConvert.DeserializeObject<Property>(r[0].ToString());

					}

					//sorting
					propertyViewModel.property.imageList = propertyViewModel.property.imageList.OrderBy(im => im.sortOrder).ToArray<PropertyImage>();

					TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

					foreach (var attribute in propertyViewModel.property.attributeList)
					{
						attribute.attributeName = textInfo.ToTitleCase(attribute.attributeName.ToLower());
					}


					ViewData["Title"] = propertyViewModel.property.metaTitle;
					ViewData["Keywords"] = propertyViewModel.property.metaKeywords;
					ViewData["Description"] = propertyViewModel.property.metaDescription;
					ViewData["Thumbnail"] = propertyViewModel.property.thumbnail;
					ViewData["CanonicalUrl"] = General.GetUrlSlug(propertyViewModel.property.perma_title) + "-" + encodedId;

					//get terms

					query = string.Format(@"SELECT content FROM page WHERE url_slug='terms-and-conditions'
                                  AND is_active='Y'AND page_status='ACTIVE'"
									);

					ds = sqlHelper.GetDatasetByMySql(query);


					foreach (DataRow r in ds.Tables[0].Rows)
					{

						propertyViewModel.terms = r["content"].ToString();

					}

					//promo properties

					propertyViewModel.promoPropertyList = new List<Property>();
					//get top 10 property list grouped by tags in each group for home displayabl tags

					var condition = string.Format(" p.id IN(SELECT property_id FROM property_tag WHERE tag_id={0})", 10);

					query = string.Format(@"SET SESSION group_concat_max_len = 100000;
                                     SELECT 
                                      JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'thumbnail',p.thumbnail  
                                     ,'sortOrder',p.sort_order
                                    ,'city',c.city_name,'state',s.state_name,'country',cn.country_name
                                    ,'priceList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                    ']')
                             AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order))
                                
                          
                             from property p 
                             INNER JOIN city c ON p.city=c.city_code 
                             INNER JOIN state s ON p.state=s.state_code 
                             INNER JOIN country cn ON p.country=cn.country_code  
                             where p.is_active='Y' AND p.property_status='ACTIVE' AND {0} AND p.id !={1}", condition, decodedId
									);

					ds = sqlHelper.GetDatasetByMySql(query);


					foreach (DataRow r in ds.Tables[0].Rows)
					{

						propertyViewModel.promoPropertyList.Add(JsonConvert.DeserializeObject<Property>(r[0].ToString()));

					}
				}
				else
				{
					return Redirect("Home/Error");
				}
			}
            catch(Exception ex)
            {
				General.LogException(ex);

				return Redirect("Home/Error");
			}
			return View(propertyViewModel);
		}

		[HttpGet]
		public IActionResult Check(ReservModel model)
		{
			//if (!ModelState.IsValid)
			//{
			return View();
			//}
		}

		[HttpPost]
		public IActionResult Reserve(ReservModel model)
		{
			if (ModelState.IsValid && model.agreeTerms.ToLower() == "yes")
			{

				if (model.referralCode == null)
				{
					model.referralCode = string.Empty;
				}
				try
				{
					MySqlHelper sqlHelper = new MySqlHelper();


					var custId = 0;

					//check if customer exists
					var query = string.Format(@"SELECT id,cust_name,cust_email FROM customer WHERE cust_phone='{0}' AND cust_email='{1}'"
								 , model.custPhone, model.custEmail);
					DataSet ds = sqlHelper.GetDatasetByMySql(query);
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							custId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
							//update existing customer with OTP
							query = string.Format(@"UPDATE customer SET cust_name='{0}' WHERE id={1}"
								 , model.custName, custId);
							ds = sqlHelper.GetDatasetByMySql(query);
						}
						else
						{
							//create new customer 
							query = string.Format(@"INSERT INTO customer(cust_name,cust_email,cust_phone,referral_code)
					                             VALUES('{0}','{1}','{2}','{3}');SELECT LAST_INSERT_ID() AS id;"
									 , model.custName, model.custEmail, model.custPhone,
									  model.referralCode.ToUpper());
							ds = sqlHelper.GetDatasetByMySql(query);
							if (ds.Tables.Count > 0)
							{
								if (ds.Tables[0].Rows.Count > 0)
								{
									custId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
								}
							}
						}
					}
					if (custId > 0)
					{
						//insert enquiry



						query = string.Format(@"INSERT INTO enquiry(cust_id,property_id,checkin_date,checkout_date,adults_count,children_count,room_id,room_name,price_list_json,total_price_per_room, referral_person, referral_code)
					                             VALUES({0},{1},'{2}','{3}',{4},{5},{6},'{7}','{8}',{9}, '{10}', '{11}');SELECT LAST_INSERT_ID() AS id;"
										  , custId, model.propertyId,
										  model.startDate.ToString("yyyy-MM-dd"), model.endDate.ToString("yyyy-MM-dd"),
										  model.adultsCount, model.childrenCount, model.roomId, model.roomName, model.priceListJson, model.totalPricePerRoom, model.referralPerson, model.referralCode);
						ds = sqlHelper.GetDatasetByMySql(query);
						if (ds.Tables.Count > 0)
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								var enquiryId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());


								//send email to admin

								query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='enquiry_email_admin'";
								ds = sqlHelper.GetDatasetByMySql(query);
								if (ds.Tables.Count > 0)
								{
									if (ds.Tables[0].Rows.Count > 0)
									{
										var emailBody = ds.Tables[0].Rows[0]["content"].ToString();

										query = string.Format(@"SELECT t1.id,t1.property_id,t1.cust_id,t1.adults_count,t1.children_count,
					                                              t1.checkin_date,t1.checkout_date,t1.room_name,t1.room_id,t1.price_list_json,t1.total_price_per_room,t2.cust_name,t2.cust_email,t2.cust_phone,t1.referral_code,
					                                               t1.referral_person,t3.id AS property_id,t3.thumbnail,t3.title,t3.perma_title,t3.short_description FROM enquiry t1 LEFT JOIN customer t2
					                                               ON t1.cust_id=t2.id LEFT JOIN property t3 ON t1.property_id=t3.id WHERE t1.is_active='Y' AND t1.id={0}", enquiryId);
										ds = sqlHelper.GetDatasetByMySql(query);
										DataRow r = ds.Tables[0].Rows[0];

										var priceListText = string.Empty;
										if (r["price_list_json"].ToString() != null)
										{
											var priceListJson = JsonConvert.DeserializeObject<List<PriceData>>(r["price_list_json"].ToString());
											foreach (var priceData in priceListJson)
											{
												priceListText += priceData.invDate.ToString("yyy MMM dd") + " : " + priceData.price + "<br/>";
											}
										}

										emailBody = emailBody.Replace("#ADULTS#", r["adults_count"].ToString())
										.Replace("#CHILDREN#", r["children_count"].ToString())
										.Replace("#CHECKIN#", DateTime.Parse(r["checkin_date"].ToString()).ToString("yyyy MMM dd"))
										.Replace("#CHECKOUT#", DateTime.Parse(r["checkout_date"].ToString()).ToString("yyyy MMM dd"))
										.Replace("#NAME#", r["cust_name"].ToString())
										.Replace("#EMAIL#", r["cust_email"].ToString())
										.Replace("#PHONE#", r["cust_phone"].ToString())
										.Replace("#PROPERTY#", r["title"].ToString())
										.Replace("#REFERRAL_CODE#", r["referral_code"].ToString())
										.Replace("#REFERRAL_PERSON#", r["referral_person"].ToString())
										.Replace("#ROOM_NAME#", r["room_name"].ToString())
										.Replace("#TOTAL_PRICE_PER_ROOM#", r["total_price_per_room"].ToString())
										.Replace("#PRICE_LIST_JSON#", priceListText)

										;



										var subject = General.GetSettingsValue("enquiry_email_subject")
											.Replace("#PROPERTY#", r["title"].ToString()).Replace("#ENQUIRYID#", enquiryId.ToString());

										General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("enquiry_from_display_name"));



										var notifySms = General.GetSettingsValue("reserve_notify_sms").Replace("#PROPERTY#", r["title"].ToString());
										var notifySmsTo = General.GetSettingsValue("reserve_notify_sms_to");

										General.sendSMS(notifySms, notifySmsTo);

										//send email to customer
										query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='enquiry_email_customer'";
										ds = sqlHelper.GetDatasetByMySql(query);
										var emailBodyCustomer = ds.Tables[0].Rows[0]["content"].ToString();

										var propertyUrl = "https://voyehomes.com/" + General.GetUrlSlug(r["perma_title"].ToString() + "-" + General.EncodeString(r["property_id"].ToString()));

										emailBodyCustomer = emailBodyCustomer.Replace("#ADULTS#", r["adults_count"].ToString())
									   .Replace("#CHILDREN#", r["children_count"].ToString())
									   .Replace("#CHECKIN#", DateTime.Parse(r["checkin_date"].ToString()).ToString("yyyy MMM dd"))
									   .Replace("#CHECKOUT#", DateTime.Parse(r["checkout_date"].ToString()).ToString("yyyy MMM dd"))
									   .Replace("#NAME#", r["cust_name"].ToString())
									   .Replace("#EMAIL#", r["cust_email"].ToString())
									   .Replace("#PHONE#", r["cust_phone"].ToString())
									   .Replace("#PROPERTY#", r["title"].ToString())
									   .Replace("#REFERRAL_CODE#", r["referral_code"].ToString())
									   .Replace("#REFERRAL_PERSON#", r["referral_person"].ToString())
									   .Replace("#ENQUIRYID#", enquiryId.ToString())
									   .Replace("#THUMBNAIL#", r["thumbnail"].ToString())
									   .Replace("#SHORT_DESCRIPTION#", r["short_description"].ToString())
									   .Replace("#PROPERTY_URL#", propertyUrl);

										var subjectCustomer = General.GetSettingsValue("enquiry_email_subject_customer");

										General.SendMailMailgun(subjectCustomer, emailBodyCustomer, r["cust_email"].ToString(), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("enquiry_from_display_name"));


									}
								}



							}
						}
						TempData["SuccessMessage"] = "Your reservation was successful!";
						return RedirectToAction("Detail", new { id = getpropertyurlid(model.propertyId) });
					}

				}
				catch (Exception ex)
				{

					General.LogException(ex);
				}


			}
			TempData["ErrorMessage"] = "Something went wrong!";
			return RedirectToAction("Detail", new { id = getpropertyurlid(model.propertyId) });
		}

		public string getpropertyurlid(int id)
		{
			MySqlHelper sqlHelper = new MySqlHelper();

			var query = string.Format(@"SELECT perma_title FROM property WHERE id = {0} ", id);

			DataSet ds = sqlHelper.GetDatasetByMySql(query);

			string propertyid = General.GetUrlSlug(ds.Tables[0].Rows[0]["perma_title"].ToString()) + "-" + @General.EncodeString(id.ToString());

			return propertyid;
		}
	}
}
