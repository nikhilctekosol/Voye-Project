using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace VTravel.HostWeb.Controllers
{
    
    [Route("api/inventory"),
    Authorize//
        ]
    public class InventoryController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public InventoryController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        

        [HttpGet, Route("get-list")]//
        public IActionResult GetList(int propertyId,string dateFrom,string dateTo)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y' AND property_id={0} AND inv_date BETWEEN '{1}' AND '{2}'  ORDER BY inv_date"
                                  , propertyId, dateFrom, dateTo);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    inventory.Add(
                        new InvData
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            invDate =DateTime.Parse(r["inv_date"].ToString()),                           
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            totalQty = Convert.ToInt32(r["total_qty"].ToString()),
                            bookedQty = Convert.ToInt32(r["booked_qty"].ToString()),

                        }
                        );

                }


                response.Data = inventory;
                response.ActionStatus = "SUCCESS";
                   
                

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-list-room")]//
        public IActionResult GetListRoom(int propertyId, int roomId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
				//var userRole = "";
				//var identity = HttpContext.User.Identity as ClaimsIdentity;
				//if (identity != null)
				//{
				//	IEnumerable<Claim> claims = identity.Claims;

				//	userRole = identity.FindFirst(ClaimTypes.Role).Value;

				//}

				List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                //var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y'
                //        AND  property_id in(select property_id from partner_user_property where partner_user_id={0}) AND room_id={1} AND inv_date >= '{2}'  ORDER BY inv_date"
                //                  , userId, roomId, DateTime.Today.ToString("yyyy-MM-dd")); 
                //var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id,	IFNULL(r.noofrooms,0) total_qty,i.booked_qty,i.price price1,i.extra_bed_price,i.child_price, IFNULL(rb.rate, 0) price, rb.rateplan,
                //                            GROUP_CONCAT(concat(o.occupancy, ' - ', rb1.rate)) AS occ_rates FROM inventory i
                //                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id and rb.occupancy = 2
                //                            left join rateplan_breakup rb1 on rb1.rateplan =  i.rateplan and rb1.room_id = i.room_id 
                //                            left join occupancy o on o.id = rb1.occupancy
                //                            left join room r on i.room_id = r.id
                //                            WHERE i.is_active='Y' AND i.property_id in(select property_id from partner_user_property where partner_user_id={0}) AND i.room_id={1} AND i.inv_date >= '{2}'
                //                            group by i.id, i.inv_date, i.room_id, i.property_id, r.noofrooms, i.booked_qty, i.price, i.extra_bed_price, i.child_price, rb.rate, rb.rateplan 
                //                            ORDER BY i.inv_date;"
                //                  , userId, roomId, DateTime.Today.ToString("yyyy-MM-dd"));

                var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id,	IFNULL(r.noofrooms,0) total_qty,i.booked_qty,i.extra_bed_price,i.child_price, IFNULL(rb.rate, IFNULL(rb2.rate, 0)) price,
                                            GROUP_CONCAT(concat(o.occupancy, ' - ', m.mealplan, ' - ', rb1.rate)) AS occ_rates FROM inventory i
                                            left join room r on i.room_id = r.id
                                            left join room_meals rm1 on rm1.room_id = r.id
                                            left join room_meals rm on rm.room_id = r.id and rm.mealplan = (select MIN(mealplan) from room_meals where room_id = r.id)
                                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id and rb.occupancy = 2 and rb.mealplan = rm.mealplan
                                            left join rateplan_breakup rb1 on rb1.rateplan =  i.rateplan and rb1.room_id = i.room_id and rb1.mealplan = rm1.mealplan
                                            left join rateplan_breakup rb2 on rb2.rateplan =  i.rateplan and rb2.room_id = i.room_id and rb2.occupancy = 1 and rb2.mealplan = rm.mealplan
                                            left join occupancy o on o.id = rb1.occupancy
                                            left join mealplans m on m.id = rb1.mealplan
                                            WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date >= '{2}'
                                            group by i.id, i.inv_date, i.room_id, i.property_id, r.noofrooms, i.booked_qty, i.price, i.extra_bed_price, i.child_price, rb.rate, rb.rateplan 
                                            ORDER BY i.inv_date;"
								  , propertyId, roomId, DateTime.Today.ToString("yyyy-MM-dd"));


				DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    inventory.Add(
                        new InvData
                        {
							id = Convert.ToInt32(r["id"].ToString()),
							invDate = DateTime.Parse(r["inv_date"].ToString()),
							roomId = r["room_id"].ToString(),
							propertyId = r["property_id"].ToString(),
							totalQty = Convert.ToInt32(r["total_qty"].ToString()),
							bookedQty = Convert.ToInt32(r["booked_qty"].ToString()),
							price = double.Parse(r["price"].ToString()),
							extraBedPrice = double.Parse(r["extra_bed_price"].ToString()),
							childPrice = double.Parse(r["child_price"].ToString()),
							occrates = r["occ_rates"].ToString()

						}
                        );

                }


                response.Data = inventory;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


		}

		[HttpGet, Route("get-room-inventory")]//
		public IActionResult GetRoomInventory(int propertyId, int roomId, string from, string to)
		{
			ApiResponse response = new ApiResponse();
			response.ActionStatus = "FAILURE";
			response.Message = string.Empty;

			try
			{

				List<RoomInventory> inventory = new List<RoomInventory>();
				MySqlHelper sqlHelper = new MySqlHelper();

				//var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty,price,extra_bed_price,child_price  FROM inventory WHERE is_active='Y' AND property_id={0} AND room_id={1} AND inv_date >= '{2}'  ORDER BY inv_date"
				//                  , propertyId, roomId, DateTime.Today.ToString("yyyy-MM-dd")); 
				var query = string.Format(@"SELECT i.room_id, i.property_id, i.inv_date, r.normal_occupancy, r.max_adults, r.max_children, o.occupancy, o.occ_count, rb.rate FROM inventory i
                                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id 
                                            left join occupancy o on o.id = rb.occupancy
                                            left join room r on i.room_id = r.id
                                            WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date between '{2}' and '{3}'
                                            ORDER BY i.inv_date;"
								  , propertyId, roomId, from, DateTime.Parse(to).AddDays(-1).ToString("yyyy-MM-dd"));

				DataSet ds = sqlHelper.GetDatasetByMySql(query);


				foreach (DataRow r in ds.Tables[0].Rows)
				{

					inventory.Add(
						new RoomInventory
						{
							roomId = r["room_id"].ToString(),
							propertyId = r["property_id"].ToString(),
							invDate = DateTime.Parse(r["inv_date"].ToString()),
							normalocc = Convert.ToInt32(r["normal_occupancy"].ToString()),
							maxadults = Convert.ToInt32(r["max_adults"].ToString()),
							maxchildren = Convert.ToInt32(r["max_children"].ToString()),
							occupancy = r["occupancy"].ToString(),
							occcount = Convert.ToInt32(r["occ_count"].ToString()),
							rate = Convert.ToDecimal(r["rate"].ToString())
						}
						);

				}


				response.Data = inventory;
				response.ActionStatus = "SUCCESS";//



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


