using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.UAEAdmin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Claims;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Transactions;
//using Microsoft.Extensions.Hosting;

namespace VTravel.UAEAdmin.Controllers
{

    [Route("api/property"), Authorize(Roles  = "ADMIN,SUB_ADMIN,OPERATIONS,MARKETING")]
    public class PropertyController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PropertyController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }

        [Authorize(Roles  = "ADMIN")]
        [HttpPost, Route("sort")]
        public IActionResult Sort([FromBody] SortData model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE property SET sort_order={1} WHERE id={2}", model.pushDownValue,
                                     model.sortOrder, model.itemId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);



                    response.ActionStatus = "SUCCESS";
                    response.Message = "properties sorted";
                }
                else
                {
                    return BadRequest("Invalid sort property");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Property> properties = new List<Property>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,address,city,property_status,sort_order 
                 FROM property WHERE is_active='Y' ORDER BY sort_order"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    properties.Add(
                        new Property
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            address = r["address"].ToString(),
                            city = r["city"].ToString(),
                            propertyStatus = r["property_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = properties;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-active-list")]
        public IActionResult GetActiveList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Property> properties = new List<Property>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,address,city,property_status,sort_order 
                 FROM property WHERE is_active='Y' and property_status!='SUSPENDED' ORDER BY sort_order"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    properties.Add(
                        new Property
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            address = r["address"].ToString(),
                            city = r["city"].ToString(),
                            propertyStatus = r["property_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = properties;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-inactive-list")]
        public IActionResult GetInactiveList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Property> properties = new List<Property>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,address,city,property_status,sort_order 
                 FROM property WHERE is_active='Y' and property_status='SUSPENDED' ORDER BY sort_order"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    properties.Add(
                        new Property
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            address = r["address"].ToString(),
                            city = r["city"].ToString(),
                            propertyStatus = r["property_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = properties;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-list-sorted-by-name")]
        public IActionResult GetListSortName()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Property> properties = new List<Property>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,address,city,property_status,sort_order 
                 FROM property WHERE is_active='Y' ORDER BY title"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    properties.Add(
                        new Property
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            address = r["address"].ToString(),
                            city = r["city"].ToString(),
                            propertyStatus = r["property_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = properties;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get")]
        public IActionResult Get(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                Property property = new Property();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,reserve_alert,reserve_allowed,destination_id,thumbnail,address,city,property_status,sort_order 
                 ,short_description,long_description,latitude,longitude,state,country,property_type_id
                 ,display_radius,meta_title,meta_keywords,meta_description,email,phone,max_occupancy  
                 ,room_count,bathroom_count,user_name, hide_property, IFNULL(is_gst, 0) is_gst, owner FROM property WHERE is_active='Y' AND id={0} ORDER BY sort_order", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    property = new Property
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        title = r["title"].ToString(),
                        thumbnail = r["thumbnail"].ToString(),
                        address = r["address"].ToString(),
                        city = r["city"].ToString(),
                        propertyStatus = r["property_status"].ToString(),
                        sortOrder = Convert.ToInt32(r["sort_order"].ToString()),
                        shortDescription = r["short_description"].ToString(),
                        longDescription = r["long_description"].ToString(),
                        latitude = Double.Parse(r["latitude"].ToString()),
                        longitude = Double.Parse(r["longitude"].ToString()),
                        state = r["state"].ToString(),
                        country = r["country"].ToString(),
                        displayRadius = float.Parse(r["display_radius"].ToString()),
                        metaTitle = r["meta_title"].ToString(),
                        metaKeywords = r["meta_keywords"].ToString(),
                        metaDescription = r["meta_description"].ToString(),
                        propertyTypeId = r["property_type_id"].ToString(),
                        destinationId = r["destination_id"].ToString(),
                        email = r["email"].ToString(),
                        phone = r["phone"].ToString(),
                        maxOccupancy = Convert.ToInt32(r["max_occupancy"].ToString()),
                        roomCount = Convert.ToInt32(r["room_count"].ToString()),
                        bathroomCount = Convert.ToInt32(r["bathroom_count"].ToString()),

                        reserveAlert = r["reserve_alert"].ToString(),
                        reserveAllowed = r["reserve_allowed"].ToString(),
                        userName = r["user_name"].ToString(),
                        hideProperty = r["hide_property"].ToString(),
                        gst = r["is_gst"].ToString(),
                        owner = r["owner"].ToString(),

                    };

                }

                response.Data = property;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] Property model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO property(property_type_id,title,perma_title) VALUES({0},'{1}','{1}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.propertyTypeId, model.title);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow r = ds.Tables[0].Rows[0];
                                model.id = Convert.ToInt32(r["id"].ToString());
                                response.Data = model;
                                response.ActionStatus = "SUCCESS";
                            }
                        }
                    }

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET title='{0}',property_type_id={1},short_description='{2}'
                           ,address='{3}',country='{4}',state='{5}',city='{6}',display_radius={7}
                           ,max_occupancy={8} ,room_count={9} ,bathroom_count={10},destination_id={11}
                            ,reserve_allowed='{12}',reserve_alert='{13}', hide_property='{18}', is_gst='{19}', owner= {20},user_name='{14}',updated_on='{15}',updated_by={16} WHERE id={17}",
                                     model.title, model.propertyTypeId, model.shortDescription
                                     , model.address, model.country, model.state, model.city
                                     , model.displayRadius,model.maxOccupancy, model.roomCount
                                     , model.bathroomCount,model.destinationId, model.reserveAllowed,model.reserveAlert,model.userName,
                                     DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),userId,id, model.hideProperty, model.gst, model.owner);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-contact")]
        public IActionResult UpdateContact([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET phone='{0}',email='{1}' WHERE id={2}",
                                     model.phone, model.email, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpPut, Route("update-meta")]
        public IActionResult UpdateMeta([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET meta_title='{0}',meta_keywords='{1}',meta_description='{2}' WHERE id={3}",
                                     model.metaTitle, model.metaKeywords, model.metaDescription, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-location")]
        public IActionResult UpdateLocation([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET latitude={0},longitude={1} WHERE id={2}",
                                     model.latitude, model.longitude, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpPut, Route("update-about")]
        public IActionResult UpdateAbout([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET long_description='{0}' WHERE id={1}",
                           model.longDescription, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-status")]
        public IActionResult UpdateStatus([FromBody] Property model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET property_status='{0}' WHERE id={1}",
                           model.propertyStatus, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete")]
        public IActionResult Delete(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (id > 0)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        
        [HttpGet, Route("get-amenity-list")]
        public IActionResult GetAmenityList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyAmenity> amenityList = new List<PropertyAmenity>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT t1.id,t1.amenity_name,
                                    CASE  
                                     WHEN t2.property_id <> 'null' THEN 1
                                     ELSE 0
                                    END AS status
                                    FROM amenity t1
                                    LEFT OUTER JOIN property_amenity t2 ON t1.id=t2.amenity_id AND t2.property_id={0}
                                     WHERE t1.is_active='Y' 
                                    ORDER BY t1.sort_order, t1.amenity_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    amenityList.Add(
                        new PropertyAmenity
                        {
                            amenityId = Convert.ToInt32(r["id"].ToString()),
                            amenityName = r["amenity_name"].ToString(),
                            status = Convert.ToInt32(r["status"].ToString())
                        }
                        );

                }


                response.Data = amenityList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }
      
       
        [HttpGet, Route("get-price-list")]
        public IActionResult GetPriceList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyPrice> priceList = new List<PropertyPrice>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT id,price_name,mrp,price FROM property_price WHERE property_id={0} 
                                    ORDER BY sort_order, price_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    priceList.Add(
                        new PropertyPrice
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            priceName = r["price_name"].ToString(),
                            mrp=float.Parse(r["mrp"].ToString()),
                            price = float.Parse(r["price"].ToString())
                        }
                        );

                }


                response.Data = priceList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create-property-price")]
        public IActionResult CreatePropertyPrice([FromBody] PropertyPrice model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"INSERT INTO property_price(property_id,price_name,mrp,price)
                      VALUES({0},'{1}',{2},{3})",
                        model.propertyId, model.priceName, model.mrp, model.price);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-property-price")]
        public IActionResult UpdatePropertyPrice([FromBody] PropertyPrice model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"UPDATE property_price SET price_name='{0}', mrp={1},price={2} WHERE id={3}",
                        model.priceName, model.mrp, model.price, id);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-property-price")]
        public IActionResult DeletePropertyPrice(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();
                string query = string.Empty; 

                query = string.Format(@"DELETE FROM property_price WHERE id={0}",
                     id);
                //

                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-image-list")]
        public IActionResult GetImageList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyImage> imageList = new List<PropertyImage>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT pi.id,url,image_alt, c.id category_id, IFNULL(c.category_name,'') category, s.id subcat_id, IFNULL(s.subcategory_name, '') subcategory, r.id room_id, IFNULL(r.title,'') room
									FROM property_image pi
                                    left join img_category c on c.id = pi.category
                                    left join img_subcategory s on s.id = pi.subcategory
                                    left join room r on r.id = pi.room
                                    WHERE pi.property_id={0} 
                                    ORDER BY c.id, s.id,pi.sort_order, pi.id;", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    imageList.Add(
                        new PropertyImage
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            url = r["url"].ToString(),
                            image_alt = r["image_alt"].ToString(),
                            categoryid = r["category_id"] == DBNull.Value ? "0" : r["category_id"].ToString(),
                            category = r["category"] == DBNull.Value ? "0" : r["category"].ToString(),
                            subcategoryid = r["subcat_id"] == DBNull.Value ? "0" : r["subcat_id"].ToString(),
                            subcategory = r["subcategory"] == DBNull.Value ? "0" : r["subcategory"].ToString(),
                            roomid = r["room_id"] == DBNull.Value ? "0" : r["room_id"].ToString(),
                            room = r["room"] == DBNull.Value ? "" : r["room"].ToString(),
                        }
                        );

                }


                response.Data = imageList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-property-amenity")]
        public IActionResult UpdatePropertyAmenity([FromBody] PropertyAmenity model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    if (model.status == 0)
                    {
                        query = string.Format(@"DELETE FROM property_amenity WHERE amenity_id={0} AND property_id={1}",
                         model.amenityId, id);
                    }
                    else if (model.status == 1)
                    {
                        query = string.Format(@"INSERT INTO property_amenity(amenity_id,property_id) 
                        VALUES({0},{1})",
                          model.amenityId, id);
                    }


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-attribute-list")]
        public IActionResult GetAttributeList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyAttribute> attributeList = new List<PropertyAttribute>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT t1.id,t1.attribute_id,t2.attribute_name, t1.long_description FROM property_attribute t1 
                                INNER JOIN attribute t2 ON t1.attribute_id=t2.id WHERE t1.is_active='Y' AND t1.property_id={0}
                                ORDER BY t2.sort_order", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    attributeList.Add(
                        new PropertyAttribute
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            attributeId = r["attribute_id"].ToString(),
                            attributeName = r["attribute_name"].ToString(),
                            longDescription = r["long_description"].ToString()
                        }
                        );

                }


                response.Data = attributeList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create-property-attribute")]
        public IActionResult CreatePropertyAttribute([FromBody] PropertyAttribute model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"INSERT INTO property_attribute(property_id,attribute_id,long_description)
                      VALUES({0},{1},'{2}')",
                        model.propertyId, model.attributeId, model.longDescription);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-property-attribute")]
        public IActionResult UpdatePropertyAttribute([FromBody] PropertyAttribute model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"UPDATE property_attribute SET attribute_id={0}, long_description='{1}' WHERE id={2}",
                        model.attributeId, model.longDescription, id);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-property-attribute")]
        public IActionResult DeletePropertyAttribute(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();
                string query = string.Empty; ;

                query = string.Format(@"DELETE FROM property_attribute WHERE id={0}",
                     id);


                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-tag-list")]
        public IActionResult GetTagList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyTag> tagList = new List<PropertyTag>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT t1.id,t1.tag_name,
                                    CASE  
                                     WHEN t2.property_id <> 'null' THEN 1
                                     ELSE 0
                                    END AS status
                                    FROM tag t1
                                    LEFT OUTER JOIN property_tag t2 ON t1.id=t2.tag_id AND t2.property_id={0} 
                                    WHERE t1.is_active='Y'
                                    ORDER BY t1.sort_order, t1.tag_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    tagList.Add(
                        new PropertyTag
                        {
                            tagId = Convert.ToInt32(r["id"].ToString()),
                            tagName = r["tag_name"].ToString(),
                            status = Convert.ToInt32(r["status"].ToString())
                        }
                        );

                }


                response.Data = tagList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-property-tag")]
        public IActionResult UpdatePropertyTag([FromBody] PropertyTag model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {///

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    if (model.status == 0)
                    {
                        query = string.Format(@"DELETE FROM property_tag WHERE tag_id={0} AND property_id={1}",
                         model.tagId, id);
                    }
                    else if (model.status == 1)
                    {
                        query = string.Format(@"INSERT INTO property_tag(tag_id,property_id) 
                        VALUES({0},{1})",
                          model.tagId, id);
                    }


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpPut, Route("update-thumbnail"), DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateThumbnail(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;
            try
            {
                var file = Request.Form.Files[0];
                //var folderName = Path.Combine("Resources", "Images");
                //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                
                if (file.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, guid+file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        
                        await file.CopyToAsync(fileStream);
                    }


                    Account account = new Account(
                         General.GetSettingsValue("cdy_cloud_name"),
                         General.GetSettingsValue("cdy_api_key"),
                         General.GetSettingsValue("cdy_api_secret"));
                    Cloudinary cloudinary = new Cloudinary(account);

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, filePath),
                        Folder = "property/" + id + "/thumbnail",
                        Overwrite = true,
                        PublicId = "thumbnail",
                        Invalidate = true
                    };


                    var uploadResult = cloudinary.Upload(uploadParams);
                    System.IO.File.Delete(filePath);

                    if (uploadResult.Url != null)
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();

                        var query = string.Format(@"UPDATE property SET thumbnail='{0}' WHERE id={1}",
                               uploadResult.SecureUrl, id);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                        response.ActionStatus = "SUCCESS";
                        response.Data = new { id = id, file = "" };
                    }



                }
                else
                {
                    return BadRequest("invalid file details");
                }
            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong "+ ex.Message;
            }

            return new OkObjectResult(response);

        }

        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpPut, Route("add-image"), DisableRequestSizeLimit]
        public async Task<IActionResult> AddImage(int id, int categoryid, int subcategoryid, int roomid)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;
            try
            {
                var files = Request.Form.Files;
                //var folderName = Path.Combine("Resources", "Images");
                //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, guid + file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {

                            await file.CopyToAsync(fileStream);
                        }


                        Account account = new Account(
                             General.GetSettingsValue("cdy_cloud_name"),
                             General.GetSettingsValue("cdy_api_key"),
                             General.GetSettingsValue("cdy_api_secret"));
                        Cloudinary cloudinary = new Cloudinary(account);


                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, filePath),
                            Folder = "property/" + id + "/image",
                            Overwrite = true,
                            PublicId = guid,
                            Invalidate = true
                        };


                        var uploadResult = cloudinary.Upload(uploadParams);
                        System.IO.File.Delete(filePath);

                        if (uploadResult.Url != null)
                        {
                            MySqlHelper sqlHelper = new MySqlHelper();

                            var query = string.Format(@"INSERT INTO property_image(property_id,url,public_id, category, subcategory, room) VALUES({0},'{1}','{2}', {3}, {4}, {5})",
                                    id, uploadResult.SecureUrl, guid, categoryid, subcategoryid, roomid);

                            DataSet ds = sqlHelper.GetDatasetByMySql(query);

                            response.ActionStatus = "SUCCESS";
                            response.Data = new { id = id, file = "" };
                        }
                    }




                }
                else
                {
                    return BadRequest("invalid file details");
                }
            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong " + ex.Message;
            }

            return new OkObjectResult(response);

        }

        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpDelete, Route("delete-property-image")]
        public IActionResult DeletePropertyImage(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (id > 0)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    //get publicId
                    var query = string.Format(@"SELECT url,public_id FROM property_image WHERE id={0}", id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    var publicId = ds.Tables[0].Rows[0]["public_id"].ToString();
                   

                    query = string.Format(@"DELETE FROM property_image WHERE id={0}",
                           id);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    //delete from cloud
                    Account account = new Account(
                           General.GetSettingsValue("cdy_cloud_name"),
                           General.GetSettingsValue("cdy_api_key"),
                           General.GetSettingsValue("cdy_api_secret"));
                    Cloudinary cloudinary = new Cloudinary(account);

                    //var deletionParams = new DeletionParams(publicId);                    
                    //var deletionResult = cloudinary.Destroy(deletionParams);

                    var deletionParams = new DeletionParams(publicId)
                    {
                        PublicId = publicId,
                        Invalidate=true,
                        ResourceType=ResourceType.Image
                        
                    };
                    var deletionResult = cloudinary.Destroy(deletionParams);

                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid image details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-room-list")]
        public IActionResult GetRoomList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Room> roomList = new List<Room>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT room.id,title,room_type_id,room.description,type_name, IFNULL(noofrooms, 0) noofrooms 
                            ,normal_occupancy, max_adults, max_children, IFNULL(room.base_rate, 0) base_rate
                            FROM room INNER JOIN room_type ON room.room_type_id=room_type.id WHERE property_id={0}
                            ORDER BY room.sort_order, room.id, title", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    roomList.Add(
                        new Room
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            roomTypeId = r["room_type_id"].ToString(),
                            description = r["description"].ToString(),
                            typeName = r["type_name"].ToString(),
                            noofrooms = Convert.ToInt32(r["noofrooms"].ToString()),
                            normalocc = Convert.ToInt32(r["normal_occupancy"].ToString()),
                            maxadults = Convert.ToInt32(r["max_adults"].ToString()),
                            maxchildren = Convert.ToInt32(r["max_children"].ToString()),
                            baserate = Convert.ToInt32(r["base_rate"].ToString())
                            //,
                            //years06 = Convert.ToInt32(r["years06"].ToString()),
                            //years612 = Convert.ToInt32(r["years612"].ToString()),
                            //years12 = Convert.ToInt32(r["years12"].ToString())
                        }
                        );

                }


                response.Data = roomList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-room-occupancy")]
        public IActionResult GetRoomOccupancy(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RoomOccupancy> roomList = new List<RoomOccupancy>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select o.id, IFNULL(ro.room_id, {0}) room_id, o.occupancy, o.occ_count, case when ro.id is null then 'false' else 'true' end checked from occupancy o
                                            left join room_occupancy ro on o.id = ro.occupancy and ro.room_id = {0} where o.is_default = 'N' ORDER BY o.id", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    roomList.Add(
                        new RoomOccupancy
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            roomid = Convert.ToInt32(r["room_id"].ToString()),
                            occupancy = r["occupancy"].ToString(),
                            check = r["checked"].ToString(),
                            occcount = Convert.ToInt32(r["occ_count"].ToString())
                        }
                        );

                }


                response.Data = roomList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-room-meals")]
        public IActionResult GetRoomMeals(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RoomMeals> mealList = new List<RoomMeals>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select m.id, IFNULL(rm.room_id, {0}) room_id, m.mealplan, case when rm.id is null then 'false' else 'true' end checked from mealplans m
                                            left join room_meals rm on m.id = rm.mealplan and rm.room_id = {0} ORDER BY m.id;", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    mealList.Add(
                        new RoomMeals
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            roomid = Convert.ToInt32(r["room_id"].ToString()),
                            mealplan = r["mealplan"].ToString(),
                            check = r["checked"].ToString()
                        }
                        );

                }


                response.Data = mealList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-room")]
        public IActionResult GetRoom(int propid, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Room> roomList = new List<Room>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT room.id,title,property_id,room_type_id,room.description,type_name FROM room INNER JOIN room_type ON room.room_type_id=room_type.id WHERE property_id={0} and room.id={1}
                           ORDER BY room.sort_order, title", propid, id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    roomList.Add(
                        new Room
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            roomTypeId = r["room_type_id"].ToString(),
                            propertyId = Convert.ToInt32(r["property_id"]),
                            description = r["description"].ToString(),
                            typeName = r["type_name"].ToString()
                        }
                        );

                }


                response.Data = roomList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create-room")]
        public IActionResult CreateRoom([FromBody] Room model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    //query = string.Format(@"INSERT INTO room(property_id,room_type_id,title,description, noofrooms)
                    //  VALUES({0},{1},'{2}','{3}', {4})",
                    //    model.propertyId, model.roomTypeId, model.title, model.description, model.noofrooms);


                    //DataSet ds = sqlHelper.GetDatasetByMySql(query);


                    sqlHelper.AddSetParameterToMySqlCommand("propertyId", MySqlDbType.Int32, Convert.ToInt32(model.propertyId));
                    sqlHelper.AddSetParameterToMySqlCommand("roomTypeId", MySqlDbType.Int32, Convert.ToInt32(model.roomTypeId));
                    sqlHelper.AddSetParameterToMySqlCommand("title1", MySqlDbType.String, model.title.ToString());
                    sqlHelper.AddSetParameterToMySqlCommand("description1", MySqlDbType.String, model.description.ToString());
                    sqlHelper.AddSetParameterToMySqlCommand("noofrooms1", MySqlDbType.Int32, Convert.ToInt32(model.noofrooms));
                    sqlHelper.AddSetParameterToMySqlCommand("normalocc", MySqlDbType.Int32, Convert.ToInt32(model.normalocc));
                    sqlHelper.AddSetParameterToMySqlCommand("maxadults", MySqlDbType.Int32, Convert.ToInt32(model.maxadults));
                    sqlHelper.AddSetParameterToMySqlCommand("maxchildren", MySqlDbType.Int32, Convert.ToInt32(model.maxchildren));
                    sqlHelper.AddSetParameterToMySqlCommand("baserate", MySqlDbType.Int32, Convert.ToInt32(model.baserate));
                    //sqlHelper.AddSetParameterToMySqlCommand("years_06", MySqlDbType.Int32, Convert.ToInt32(model.years06));
                    //sqlHelper.AddSetParameterToMySqlCommand("years_612", MySqlDbType.Int32, Convert.ToInt32(model.years612));
                    //sqlHelper.AddSetParameterToMySqlCommand("years_12", MySqlDbType.Int32, Convert.ToInt32(model.years12));
                    sqlHelper.AddSetParameterToMySqlCommand("userid", MySqlDbType.Int32, Convert.ToInt32(userId));

                    DataSet ds = sqlHelper.GetDatasetByCommand("insert_room");

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                response.Message = ds.Tables[0].Rows[0][0].ToString();
                            }
                        }
                    }

                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid room details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-room")]
        public IActionResult UpdateRoom([FromBody] Room model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"UPDATE room SET title='{0}', description='{1}',room_type_id={2}, noofrooms = {4}, normal_occupancy = {5}, max_adults = {6}, max_children = {7}, base_rate = {8} WHERE id={3}",
                        model.title, model.description, model.roomTypeId, id, model.noofrooms, model.normalocc, model.maxadults, model.maxchildren, model.baserate);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid room details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-room-occupancy")]
        public IActionResult UpdateRoomOccupancy([FromBody] List<RoomOccupancy> model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"DELETE FROM room_occupancy WHERE room_id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    for (int i = 0; i < model.Count; i++)
                    {
                        if (model[i].check == "true")
                        {
                            query = string.Format(@"INSERT INTO room_occupancy(room_id, occupancy) VALUES({0}, {1})",
                            id, model[i].id);

                            ds = sqlHelper.GetDatasetByMySql(query);
                        }
                    }
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid room details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-room-mealplan")]
        public IActionResult UpdateRoomMealplan([FromBody] List<RoomMeals> model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"DELETE FROM room_meals WHERE room_id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    for (int i = 0; i < model.Count; i++)
                    {
                        if (model[i].check == "true")
                        {
                            query = string.Format(@"INSERT INTO room_meals(room_id, mealplan) VALUES({0}, {1})",
                            id, model[i].id);

                            ds = sqlHelper.GetDatasetByMySql(query);
                        }
                    }
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid room details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-room")]
        public IActionResult DeleteRoom(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();
                string query = string.Empty;

                query = string.Format(@"DELETE FROM room WHERE id={0}",
                     id);
                //

                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [Authorize(Roles = "ADMIN,MARKETING")]
        [HttpPut, Route("update-image")]
        public IActionResult UpdateImage([FromBody] PropertyImage model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    query = string.Format(@"UPDATE property_image SET image_alt='{0}', category={2}, subcategory={3}, room={4} WHERE id={1}",
                        model.image_alt, id, model.categoryid, model.subcategoryid, model.roomid);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid room details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }



        [HttpGet, Route("availability")]
        public IActionResult GetAvailability(int propid, int room, DateTime fromdate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT
                            GROUP_CONCAT(DISTINCT CONCAT(
                            'SUM(CASE WHEN inv_date = ""', inv_date, '"" THEN total_qty - booked_qty ELSE 0 END)AS ', DATE_FORMAT(inv_date, ""%M_%d_%Y"")
                            )
                            )
                            INTO @sql
                            FROM inventory where inv_date between '{0}' and '{1}';

                            SET @sql = CONCAT('SELECT p.title property, r.title room, ', @sql, 
                            ' FROM inventory i
                            inner join property p on p.id = i.property_id
                            inner join room r on r.id = i.room_id 
                            where (i.property_id = {2} or {2} = 0) and (i.room_id = {3} or {3} = 0)
                            GROUP BY p.title, r.title');
  
                            PREPARE stmt FROM @sql;
                            EXECUTE stmt;
                            DEALLOCATE PREPARE stmt;", Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd"), Convert.ToDateTime(fromdate).AddDays(6).ToString("yyyy-MM-dd"), propid, room
                );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                var html = "";

                html = html + "<thead>" +
                    "<tr>" +
                    "<th scope=\"col\">Property</th>" +
                    "<th scope=\"col\">Room</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[2].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[3].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[4].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[5].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[6].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[7].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[8].ColumnName + "</th>" +
                    "</tr>" +
                    "</thead>";


                foreach (DataRow r in ds.Tables[0].Rows)
                {


                    html = html + "<tbody>" +
                        "<tr>" +
                        "<td scope=\"row\">" + r[0].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[1].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[2].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[3].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[4].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[5].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[6].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[7].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[8].ToString() + "</td  >" +
                        "</tr>" +
                        "</tbody>";

                }


                response.Data = html;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-charge-list")]
        public IActionResult GetChargeList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ChargeList> chargeList = new List<ChargeList>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT id, property_id, name, charge_type, amount, percentage, effective_from FROM app_charges where is_active = 'Y' and property_id = {0};", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    chargeList.Add(
                        new ChargeList
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            propertyid = Convert.ToInt32(r["property_id"].ToString()),
                            name = r["name"].ToString(),
                            chargetype = r["charge_type"].ToString(),
                            amount = Convert.ToDecimal(r["amount"].ToString()),
                            percentage = Convert.ToDecimal(r["percentage"].ToString()),
                            effective = DateTime.Parse(r["effective_from"].ToString()),
                        }
                        );

                }


                response.Data = chargeList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create-charge")]
        public IActionResult CreateCharge([FromBody] ChargeList model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        var query = string.Format(@"INSERT INTO app_charges(property_id, name, charge_type, amount, percentage, effective_from, is_active, created_by, created_on) VALUES({0},'{1}','{2}',{3},{4}, '{5}', '{6}', {7}, '{8}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                         model.propertyid, model.name, model.chargetype, model.amount, model.percentage, model.effective.ToString("yyyy-MM-dd"), 'Y', userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    model.id = Convert.ToInt32(r["id"].ToString());
                                    response.Data = model;
                                    response.ActionStatus = "SUCCESS";
                                }
                            }
                        }
                        scope.Complete();
                    }

                }
                else
                {
                    return BadRequest("Invalid Charge details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-charge")]
        public IActionResult UpdateCharge([FromBody] ChargeList model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        var query = string.Format(@"UPDATE app_charges set effective_to = '{0}', is_active = 'N', updated_by = {1}, updated_on = '{2}' where id = {3}",
                                         model.effective.AddDays(-1).ToString("yyyy-MM-dd"), userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.id);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                        if (ds != null)
                        {
                            query = string.Format(@"INSERT INTO app_charges(property_id, name, charge_type, amount, percentage, effective_from, is_active, created_by, created_on) VALUES({0},'{1}','{2}',{3},{4}, '{5}', '{6}', {7}, '{8}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                             model.propertyid, model.name, model.chargetype, model.amount, model.percentage, model.effective.ToString("yyyy-MM-dd"), 'Y', userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            ds = sqlHelper.GetDatasetByMySql(query);

                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow r = ds.Tables[0].Rows[0];
                                        model.id = Convert.ToInt32(r["id"].ToString());
                                        response.Data = model;
                                        response.ActionStatus = "SUCCESS";
                                    }
                                }
                            }
                        }
                        scope.Complete();
                    }

                }
                else
                {
                    return BadRequest("Invalid Charge details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-charge")]
        public IActionResult DeleteCharge(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();
                string query = string.Empty;

                query = string.Format(@"UPDATE app_charges set is_active = 'N' WHERE id={0}",
                     id);
                //

                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-property-contact")]
        public IActionResult UpdatePropertyContact([FromBody] PropertyContact model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty; ;

                    if (model.status == 0)
                    {
                        query = string.Format(@"DELETE FROM property_contacts WHERE contact_id={0} AND property_id={1}",
                         model.contact, id);
                    }
                    else if (model.status == 1)
                    {
                        query = string.Format(@"INSERT INTO property_contacts(contact_id,property_id, is_active) 
                        VALUES({0},{1}, 'Y')",
                          model.contact, id);
                    }


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-image-category-list")]
        public IActionResult GetImageCategories()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ImageCategory> imgcatList = new List<ImageCategory>();

                MySqlHelper sqlHelper = new MySqlHelper();

                    //get publicId
                    var query = string.Format(@"SELECT id, category_name, is_active from img_category where is_active = 'Y'");

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    imgcatList.Add(
                        new ImageCategory
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            category_name = r["category_name"].ToString(),
                            isactive = r["is_active"].ToString(),
                        }
                        );

                }


                response.Data = imgcatList;
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-image-subcategory-list")]
        public IActionResult GetImageSubCategories(int category)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ImageCategory> imgcatList = new List<ImageCategory>();

                MySqlHelper sqlHelper = new MySqlHelper();

                //get publicId
                var query = string.Format(@"SELECT id, subcategory_name, is_active from img_subcategory where is_active = 'Y' and category_id = {0}", category);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    imgcatList.Add(
                        new ImageCategory
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            category_name = r["subcategory_name"].ToString(),
                            isactive = r["is_active"].ToString(),
                        }
                        );

                }


                response.Data = imgcatList;
                response.ActionStatus = "SUCCESS";

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }
    }
}


