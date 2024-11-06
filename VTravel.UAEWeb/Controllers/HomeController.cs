using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VTravel.UAEWeb.Models;

namespace VTravel.UAEWeb.Controllers
{
    [ResponseCache(Duration = 30)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            try
            {
                _logger = logger;
                _configuration = configuration;
            }
            catch (Exception ex)
            {

            }
        }

        public IActionResult Index()
        {
            HomeViewModel indexViewModel = new HomeViewModel();

            try
            {
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT 
                                    meta_title,meta_keywords,meta_description                           
                                 from page p  
                                 where p.title='{0}' AND  p.is_active='Y'", "Home"
                                        );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    ViewData["Title"] = r["meta_title"].ToString();
                    ViewData["Keywords"] = r["meta_keywords"].ToString();
                    ViewData["Description"] = r["meta_description"].ToString();


                }



				query = string.Format(@"SELECT 
                                    temperature, humidity, details, last_updated, sunrise, sunset, weather_icon                         
                                 from weather_data;");

				ds = sqlHelper.GetDatasetByMySql(query);


				foreach (DataRow r in ds.Tables[0].Rows)
				{
					ViewData["Temp"] = Convert.ToInt32(r["temperature"]).ToString();
					ViewData["Humidity"] = r["humidity"].ToString();
					ViewData["details"] = r["details"].ToString();
                    //ViewData["icon"] = "http://openweathermap.org/img/wn/" + r["weather_icon"].ToString() + "@2x.png";
                    ViewData["icon"] = GetSvgData(r["weather_icon"].ToString());
					ViewData["sunrise"] = Convert.ToDateTime(r["sunrise"]).ToString("hh:mm tt");
					ViewData["sunset"] = Convert.ToDateTime(r["sunset"]).ToString("hh:mm tt");
					ViewData["last_up"] = Convert.ToDateTime(r["last_updated"]).ToString("hh:mm tt, MMM dd, yyyy");
				}


				indexViewModel.tagList = new List<Tag>();
                indexViewModel.bannerList = new List<HeroBanner>();
                indexViewModel.destinationList = new List<Destination>();
                indexViewModel.featureList = new List<Feature>();



                if (indexViewModel.tagList.Count == 0)
                {
                    // If the key does not exist in Redis, fetch the data from the database
                    query = string.Format(General.HOME_TAG_LIST_QUERY);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        indexViewModel.tagList.Add(JsonConvert.DeserializeObject<Tag>(r[0].ToString()));

                    }
                }



                //query = string.Format(General.HOME_TAG_LIST_QUERY
                //                );

                //ds = sqlHelper.GetDatasetByMySql(query);


                //foreach (DataRow r in ds.Tables[0].Rows)
                //{

                //    indexViewModel.tagList.Add(JsonConvert.DeserializeObject<Tag>(r[0].ToString()));

                //}

                //property sorting
                foreach (Tag tagr in indexViewModel.tagList)
                {
                    if (tagr.propertyList != null)
                    {
                        tagr.propertyList = tagr.propertyList.OrderBy(p => p.sortOrder).ToArray<Property>();
                    }

                }

                query = string.Format(@"SELECT b.id, b.image_url, b.navigate_url, IFNULL(b.title, '') title, IFNULL(b.navigate_url, '') navigate_url, IFNULL(b.description, '') description, IFNULL(b.property_id, 0) property_id, IFNULL(b.destination, 0) dest_id
                                           , IFNULL(p.title, '') property, IFNULL(d.title, '') destination, IFNULL(b.banner_type, 'Promotion') banner_type, IFNULL(b.offer_text, '') offer_text, IFNULL(oc.class_name, '') offer_class, IFNULL(b.coupon_code, '') coupon_code
                                           FROM hero_banner b
                                           left join property p on p.id = b.property_id
                                           left join destination d on d.id = b.destination
                                           left join offer_classes oc on oc.class_name = b.offer_class
                                           where b.is_active='Y' order by b.sort_order",
                                 0);

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                indexViewModel.bannerList.Add(
                                    new HeroBanner
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        image_url = r["image_url"].ToString(),
                                        navigate_url = r["navigate_url"].ToString() == "" ? (r["property_id"].ToString() == "0" ? (r["dest_id"].ToString() == "0" ? "" :
                                        "destination/" + @General.GetUrlSlug(r["destination"].ToString()) + "-" + @General.EncodeString(r["dest_id"].ToString()))
                                        : @General.GetUrlSlug(r["property"].ToString()) + "-" + @General.EncodeString(r["property_id"].ToString())) : r["navigate_url"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),
                                        property_id = Convert.ToInt32(r["property_id"].ToString()),
                                        destination = Convert.ToInt32(r["dest_id"].ToString()),
                                        bannertype = r["banner_type"].ToString(),
                                        offertext = r["offer_text"].ToString(),
                                        offerclass = r["offer_class"].ToString(),
                                        couponcode = r["coupon_code"].ToString()

                                    });

                            }


                        }

                    }

                }


                query = string.Format(@"SELECT id, title,description,thumbnail, img_url
                                           FROM destination WHERE  is_active='Y' ORDER BY sort_order");

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                indexViewModel.destinationList.Add(
                                    new Destination
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        img_url = r["img_url"].ToString(),
                                        thumbnail = r["thumbnail"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),

                                    });

                            }


                        }

                    }

                }


                query = string.Format(@"SELECT id, title,description,thumbnail  
                                           FROM feature WHERE  is_active='Y' ORDER BY sort_order");

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                indexViewModel.featureList.Add(
                                    new Feature
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        thumbnail = r["thumbnail"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),

                                    });

                            }


                        }

                    }

                }
            }
            catch (Exception ex)
            {
				General.LogException(ex);
			}
            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


		public static string GetSvgData(string condition)
		{
			return condition switch
			{
				"01d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" x=\"0px\" y=\"0px\" width=\"30\" height=\"30\" viewBox=\"0 0 24 24\"><path d=\"M 11 0 L 11 3 L 13 3 L 13 0 L 11 0 z M 4.2226562 2.8085938 L 2.8085938 4.2226562 L 4.9296875 6.34375 L 6.34375 4.9296875 L 4.2226562 2.8085938 z M 19.777344 2.8085938 L 17.65625 4.9296875 L 19.070312 6.34375 L 21.191406 4.2226562 L 19.777344 2.8085938 z M 12 5 A 7 7 0 0 0 5 12 A 7 7 0 0 0 12 19 A 7 7 0 0 0 19 12 A 7 7 0 0 0 12 5 z M 0 11 L 0 13 L 3 13 L 3 11 L 0 11 z M 21 11 L 21 13 L 24 13 L 24 11 L 21 11 z M 4.9296875 17.65625 L 2.8085938 19.777344 L 4.2226562 21.191406 L 6.34375 19.070312 L 4.9296875 17.65625 z M 19.070312 17.65625 L 17.65625 19.070312 L 19.777344 21.191406 L 21.191406 19.777344 L 19.070312 17.65625 z M 11 21 L 11 24 L 13 24 L 13 21 L 11 21 z\"></path></svg>",
				"01n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" x=\"0px\" y=\"0px\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\">  <path d=\"M32 46c7.7 0 14-6.3 14-14s-6.3-14-14-14c-1.2 0-2.4.2-3.6.5 2.2 2.4 3.6 5.6 3.6 9.1s-1.4 6.7-3.6 9.1c1.2.3 2.4.5 3.6.5z\" fill=\"#FFCC33\"></path></svg>",
			    "02d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><circle cx=\"22\" cy=\"22\" r=\"10\" fill=\"#FFCC33\"/><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path></svg>",
				"02n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M32 46c7.7 0 14-6.3 14-14s-6.3-14-14-14c-1.2 0-2.4.2-3.6.5 2.2 2.4 3.6 5.6 3.6 9.1s-1.4 6.7-3.6 9.1c1.2.3 2.4.5 3.6.5z\" fill=\"#FFCC33\"></path></svg>",
			    "03d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path></svg>",
			    "03n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path></svg>",
			    "04d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><ellipse cx=\"32\" cy=\"32\" rx=\"20\" ry=\"10\" fill=\"#A9A9A9\"/><ellipse cx=\"32\" cy=\"40\" rx=\"16\" ry=\"8\" fill=\"#808080\"/></svg>",
			    "04n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><ellipse cx=\"32\" cy=\"32\" rx=\"20\" ry=\"10\" fill=\"#A9A9A9\"/><ellipse cx=\"32\" cy=\"40\" rx=\"16\" ry=\"8\" fill=\"#808080\"/></svg>",
			    "09d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M28 44h-4v10h4zM36 44h-4v10h4zM44 44h-4v10h4z\" fill=\"#73C2FB\"></path></svg>",
			    "09n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M28 44h-4v10h4zM36 44h-4v10h4zM44 44h-4v10h4z\" fill=\"#73C2FB\"></path></svg>",
				"10d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M34 44h-4v8h4zM42 44h-4v8h4zM50 44h-4v8h4z\" fill=\"#73C2FB\"></path></svg>",
				"10n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M34 44h-4v8h4zM42 44h-4v8h4zM50 44h-4v8h4z\" fill=\"#73C2FB\"></path></svg>",
				"11d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M32 42l-4-8 8-6-4 8 8 6z\" fill=\"#FFCC33\"></path></svg>",
				"11n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M32 42l-4-8 8-6-4 8 8 6z\" fill=\"#FFCC33\"></path></svg>",
				"13d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M30 44l-2 4h4l-2-4zM38 44l-2 4h4l-2-4zM46 44l-2 4h4l-2-4z\" fill=\"#FFF\"></path></svg>",
				"13n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M30 44l-2 4h4l-2-4zM38 44l-2 4h4l-2-4zM46 44l-2 4h4l-2-4z\" fill=\"#FFF\"></path></svg>",
				"50d" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M28 44h-4v8h4zM36 44h-4v8h4zM44 44h-4v8h4z\" fill=\"#B3CDE0\" opacity=\"0.5\"></path></svg>",
				"50n" => "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"30\" height=\"30\" viewBox=\"0 0 64 64\"><path d=\"M50 42H20c-6.6 0-12-5.4-12-12s5.4-12 12-12c1 0 2 .1 3 .3C25.2 15 28 12 31.5 12c5.5 0 10 4.5 10 10h1c6.6 0 12 5.4 12 12s-5.4 12-12 12z\" fill=\"#B3CDE0\"></path><path d=\"M28 44h-4v8h4zM36 44h-4v8h4zM44 44h-4v8h4z\" fill=\"#B3CDE0\" opacity=\"0.5\"></path></svg>",
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
		}
	}
}
