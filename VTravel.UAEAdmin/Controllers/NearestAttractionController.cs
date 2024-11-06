using Microsoft.AspNetCore.Mvc;
using static VTravel.UAEAdmin.Models.NearestAttraction;
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace VTravel.UAEAdmin.Controllers
{
    [Route("api/nearest-attraction"), Authorize(Roles = "ADMIN")]
    public class NearestAttractionController : Controller
    {

        [HttpGet, Route("get-list")]
        public IActionResult GetList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Attraction> amenities = new List<Attraction>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,attraction_name as attractionName,image,location as attractLocation,distance, property_id
                 FROM nearest_attraction WHERE is_active='Y' AND property_id = {0} ORDER BY sort_order,attraction_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    amenities.Add(
                        new Attraction
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            attractionName = r["attractionName"].ToString(),
                            image = r["image"].ToString(),
                            attractLocation = r["attractLocation"].ToString(),
							distance = r["distance"] != DBNull.Value ? Math.Round(Convert.ToDouble(r["distance"]), 1) : 0.0,
						}
                        );

                }


                response.Data = amenities;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] Attraction model)
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

                    var query = string.Format(@"INSERT INTO nearest_attraction(attraction_name,image,location,distance,created_by,created_on, property_id) 
                                                VALUES('{0}','{1}','{2}','{3}','{4}','{5}', {6});
                                                SELECT LAST_INSERT_ID() AS id;",
                                     model.attractionName, model.image, model.attractLocation,model.distance, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.propertyid);

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
                    return BadRequest("Invalid Tag details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] Attraction model, int id)
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

                    var query = string.Format(@"UPDATE nearest_attraction SET attraction_name='{0}',image='{1}', location='{3}', distance='{4}', updated_by='{5}', updated_on='{6}'  WHERE id={2}",
                                     model.attractionName, model.image, id, model.attractLocation, model.distance, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Tag details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

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
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;
                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE nearest_attraction SET is_active='N', updated_by='{1}', updated_on='{2}'  WHERE id={0}",
                           id, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Tag details");
                }

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

                Attraction attraction = new Attraction();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,attraction_name AS attractionName,image, location, distance as attractLocation
                  FROM nearest_attraction WHERE is_active='Y' AND id={0} ORDER BY sort_order", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    attraction = new Attraction
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        attractionName = r["attractionName"].ToString(),
                        image = r["image"].ToString(),
                        attractLocation = r["attractLocation"].ToString(),
						distance = r["distance"] != DBNull.Value ? Math.Round(Convert.ToDouble(r["distance"]), 1) : 0.0,
					};

                }

                response.Data = attraction;
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
