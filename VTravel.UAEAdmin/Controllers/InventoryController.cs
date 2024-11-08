using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.UAEAdmin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace VTravel.UAEAdmin.Controllers
{
    
    [Route("api/inventory")
        , Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")
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

                var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty,price,extra_bed_price,child_price  FROM inventory WHERE is_active='Y' AND property_id={0} AND inv_date BETWEEN '{1}' AND '{2}'  ORDER BY inv_date"
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
                            price=double.Parse(r["price"].ToString()),                           
                            extraBedPrice = double.Parse(r["extra_bed_price"].ToString()),
                            childPrice = double.Parse(r["child_price"].ToString()),
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
                var userRole = "";
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;

                    userRole = identity.FindFirst(ClaimTypes.Role).Value;

                }

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                //var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty,price,extra_bed_price,child_price  FROM inventory WHERE is_active='Y' AND property_id={0} AND room_id={1} AND inv_date >= '{2}'  ORDER BY inv_date"
                //                  , propertyId, roomId, DateTime.Today.ToString("yyyy-MM-dd")); 
                //var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id,	IFNULL(r.noofrooms,0) total_qty,i.booked_qty,i.price price1,i.extra_bed_price,i.child_price, IFNULL(rb.rate, 0) price, rb.rateplan,
                //                            GROUP_CONCAT(concat(o.occupancy, ' - ', m.mealplan, ' - ', rb1.rate)) AS occ_rates, r.normal_occupancy, r.max_adults, r.max_children FROM inventory i
                //                            left join room r on i.room_id = r.id
                //                            left join room_meals rm1 on rm1.room_id = r.id
                //                            left join room_meals rm on rm.room_id = r.id and rm.mealplan = (select MIN(mealplan) from room_meals where room_id = r.id)
                //                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id and rb.occupancy = 2 and rb.mealplan = rm.mealplan
                //                            left join rateplan_breakup rb1 on rb1.rateplan =  i.rateplan and rb1.room_id = i.room_id and rb1.mealplan = rm1.mealplan
                //                            left join occupancy o on o.id = rb1.occupancy
                //                            left join mealplans m on m.id = rb1.mealplan
                //                            WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date >= '{2}'
                //                            group by i.id, i.inv_date, i.room_id, i.property_id, r.noofrooms, i.booked_qty, i.price, i.extra_bed_price, i.child_price, rb.rate, rb.rateplan 
                //                            ORDER BY i.inv_date;"
                //                  , propertyId, roomId, DateTime.Today.ToString("yyyy-MM-dd"));

                var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id,	IFNULL(r.noofrooms,0) total_qty,i.booked_qty,i.extra_bed_price,i.child_price, IFNULL(rb.rate, 0) price,
                                            GROUP_CONCAT(concat(o.occupancy, ' - ', m.mealplan, ' - ', rb1.rate)) AS occ_rates FROM inventory i
                                            left join room r on i.room_id = r.id
                                            left join room_meals rm1 on rm1.room_id = r.id
                                            left join room_meals rm on rm.room_id = r.id and rm.mealplan = (select MIN(mealplan) from room_meals where room_id = r.id)
                                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id and rb.occupancy = 2 and rb.mealplan = rm.mealplan
                                            left join rateplan_breakup rb1 on rb1.rateplan =  i.rateplan and rb1.room_id = i.room_id and rb1.mealplan = rm1.mealplan
                                            left join occupancy o on o.id = rb1.occupancy
                                            left join mealplans m on m.id = rb1.mealplan
                                            WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date >= '{2}'
                                            group by i.id, i.inv_date, i.room_id, i.property_id, r.noofrooms, i.booked_qty, i.price, i.extra_bed_price, i.child_price, rb.rate, rb.rateplan 
                                            ORDER BY i.inv_date;"
                                  , propertyId, roomId, userRole == "ADMIN" ? DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd"));

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
                            //normalocc = Convert.ToInt32(r["normal_occupancy"].ToString()),
                            //maxadults = Convert.ToInt32(r["max_adults"].ToString()),
                            //maxchildren = Convert.ToInt32(r["max_children"].ToString())
                            //years06 = double.Parse(r["years06"].ToString()),
                            //years612 = double.Parse(r["years612"].ToString()),
                            //years12 = double.Parse(r["years12"].ToString())
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


        [HttpPost, Route("create"), Authorize(Roles = "ADMIN")]
        public IActionResult Create([FromBody] InvData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId =   claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var isFound = false;
                    var query = string.Format(@"SELECT 1 FROM inventory WHERE inv_date='{0}' AND room_id={1} AND property_id={2} AND is_active='Y'",
                                     model.invDate.ToString("yyyy-MM-dd"),model.roomId, model.propertyId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                isFound = true;

                            }
                        }
                    }

                    if (isFound)
                    {
                        //update 
                        if (model.mode == "QTY") {
                            query = string.Format(@"UPDATE inventory SET total_qty={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                       model.totalQty, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                         );

                        }
                        else if (model.mode == "PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.price, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "EXTRA_BED_PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET extra_bed_price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.extraBedPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "CHILD_PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET child_price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.childPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "ALL")
                        {
                            query = string.Format(@"UPDATE inventory SET total_qty={0},price={1},extra_bed_price={2},child_price={3},updated_by={4},updated_on='{5}' 
                              WHERE inv_date='{6}' AND room_id={7} AND property_id={8}",
                                     model.totalQty, model.price,model.extraBedPrice,model.childPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }


                    }
                    else
                    {
                        //create
                        query = string.Format(@"INSERT INTO inventory(inv_date,room_id,property_id,total_qty,created_by,price,extra_bed_price,child_price)
                              VALUES('{0}',{1},{2},{3},{4},{5},{6},{7});
                                         SELECT LAST_INSERT_ID() AS id;",
                                    model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                    model.totalQty, userId,model.price,model.extraBedPrice,model.childPrice);
                    }

                    ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";
                }
                else
                {
                    return BadRequest("Invalid details");
                }

            }
            catch (Exception ex)//
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("create-bulk")]
        public IActionResult CreateBulk([FromBody] InvData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    while (model.fromDate <= model.toDate)
                    {
                        model.invDate = model.fromDate;
                        model.mode = "ALL";
                        this.Create(model);
                        model.fromDate=model.fromDate.AddDays(1);
                    }
                    
                    response.ActionStatus = "SUCCESS";
                }
                else
                {
                    return BadRequest("Invalid details");
                }

            }
            catch (Exception ex)//
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

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE reservation SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid details");
                }

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


