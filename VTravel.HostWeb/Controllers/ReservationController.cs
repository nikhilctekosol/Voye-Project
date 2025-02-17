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
    
    [Route("api/reservation"),
     Authorize
        ]
    public class ReservationController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReservationController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        

        [HttpGet, Route("get-list")]//
        public IActionResult GetList(int roomId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                List<ReservData> reservations = new List<ReservData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,from_date,to_date,customer_id,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id,details,noOfRooms,no_of_guests,is_host_booking  
                 FROM reservation WHERE is_active='Y' AND room_id={0} 
               AND  property_id in(select property_id from partner_user_property where partner_user_id={1}) AND from_date > '{2}'  ORDER BY from_date"
                                  , roomId, userId, DateTime.Today.AddDays(-60).ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    reservations.Add(
                        new ReservData
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            fromDate =DateTime.Parse(r["from_date"].ToString()),
                            toDate = DateTime.Parse(r["to_date"].ToString()),
                            customerId = r["customer_id"].ToString(),
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            custName = r["cust_name"].ToString(),
                            custEmail = r["cust_email"].ToString(),
                            custPhone = r["cust_phone"].ToString(),
                            bookingChannelId= r["booking_channel_id"].ToString(),
                            details = r["details"].ToString(),
                            noOfRooms = r["noOfRooms"].ToString(),
                            isHostBooking = r["is_host_booking"].ToString(),
                            noOfGuests = String.IsNullOrEmpty(r["no_of_guests"].ToString()) ? 0 : int.Parse(r["no_of_guests"].ToString()),
                        }
                        );

                }


                response.Data = reservations;
                response.ActionStatus = "SUCCESS";
                   
                

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


		}

		[HttpGet, Route("get-list-new")]//
		public IActionResult GetListNew(int roomId, int propertyId)
		{
			ApiResponse response = new ApiResponse();
			response.ActionStatus = "FAILURE";
			response.Message = string.Empty;

			try
			{

				IEnumerable<Claim> claims = User.Claims;
				var userrole = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

				List<ReservData> reservations = new List<ReservData>();
				MySqlHelper sqlHelper = new MySqlHelper();

				var query = string.Format(@"select t1.id,t4.from_date,t4.to_date,t1.customer_id,t4.room_id,t1.property_id,t1.cust_name,t1.cust_email,t1.cust_phone,t1.booking_channel_id,t1.details
                ,t1.noOfRooms,t1.no_of_guests,t1.final_amount, t1.is_host_booking,t1.created_on,t1.updated_on,t1.enquiry_ref,t1.res_status,t2.user_name,t2.name_of_user ,t3.user_name AS updated_user_name,t3.name_of_user  AS updated_name_of_user
                , t1.advancepayment, t1.partpayment, t1.balancepayment, t1.discount, t1.commission, t1.country, t5.country_name, t1.tds
                , (case when curdate() = date_add(MIN(t4.to_date), interval 1 day) then (case when t1.completion_enabled = 'Y' then 1 else 0 end)
                when curdate() >= date_add(MIN(t4.to_date), interval 2 day) then 0 else 1 end) user_permission, t1.validator_id, t1.validation_date, t1.validation_status, t1.booking_agent
                , IFNULL(IFNULL(t6.name_of_user, t6.user_name), '') validated_by
                FROM reservation t1 LEFT JOIN admin_user t2 ON t1.created_by=t2.id
                LEFT JOIN admin_user t3 ON t1.updated_by=t3.id 
                LEFT JOIN reserve_rooms t4 on t4.reservation_id = t1.id
                LEFT JOIN country t5 on t5.id = t1.country
                LEFT JOIN admin_user t6 ON t1.validator_id=t6.id 
                WHERE t1.is_active='Y' AND t4.room_id={0} AND t1.property_id={1} AND t4.from_date > '{2}' group by t4.id ORDER BY t1.id;"
								  , roomId, propertyId, DateTime.Today.AddDays(-60).ToString("yyyy-MM-dd"));

				DataSet ds = sqlHelper.GetDatasetByMySql(query);


				foreach (DataRow r in ds.Tables[0].Rows)
				{
					TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

					//DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);

					reservations.Add(
						new ReservData
						{
							id = Convert.ToInt32(r["id"].ToString()),
							fromDate = DateTime.Parse(r["from_date"].ToString()),
							toDate = DateTime.Parse(r["to_date"].ToString()),
							customerId = r["customer_id"].ToString(),
							roomId = r["room_id"].ToString(),
							propertyId = r["property_id"].ToString(),
							custName = r["cust_name"].ToString(),
							custEmail = r["cust_email"].ToString(),
							custPhone = r["cust_phone"].ToString(),
							bookingChannelId = r["booking_channel_id"].ToString(),
							details = r["details"].ToString(),
							noOfRooms = r["noOfRooms"].ToString(),
							isHostBooking = r["is_host_booking"].ToString(),
							noOfGuests = String.IsNullOrEmpty(r["no_of_guests"].ToString()) ? 0 : int.Parse(r["no_of_guests"].ToString()),
							finalAmount = String.IsNullOrEmpty(r["final_amount"].ToString()) ? 0 : float.Parse(r["final_amount"].ToString()),
							advancepayment = String.IsNullOrEmpty(r["advancepayment"].ToString()) ? 0 : float.Parse(r["advancepayment"].ToString()),
							partpayment = String.IsNullOrEmpty(r["partpayment"].ToString()) ? 0 : float.Parse(r["partpayment"].ToString()),
							balancepayment = String.IsNullOrEmpty(r["balancepayment"].ToString()) ? 0 : float.Parse(r["balancepayment"].ToString()),
							discount = String.IsNullOrEmpty(r["discount"].ToString()) ? 0 : float.Parse(r["discount"].ToString()),
							commission = String.IsNullOrEmpty(r["commission"].ToString()) ? 0 : float.Parse(r["commission"].ToString()),
							tds = String.IsNullOrEmpty(r["tds"].ToString()) ? 0 : float.Parse(r["tds"].ToString()),
							country = r["country"].ToString(),
							created_on = String.IsNullOrEmpty(r["created_on"].ToString()) ? ""
							: TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["created_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
							updated_on = String.IsNullOrEmpty(r["updated_on"].ToString()) ? ""
							: TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["updated_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
							created_by = r["is_host_booking"].ToString() == "Y" ? "Host" : r["user_name"].ToString() + "/" + r["name_of_user"].ToString(),
							updated_by = r["is_host_booking"].ToString() == "Y" ? "NA" : (r["updated_user_name"].ToString() == "" ? r["updated_user_name"].ToString() : r["updated_user_name"].ToString() + "/") + r["updated_name_of_user"].ToString(),
							enquiry_ref = r["enquiry_ref"].ToString(),
							res_status = r["res_status"].ToString(),
							user_permission = userrole.ToString() == "ADMIN" ? "1" : r["user_permission"].ToString(),
							validator_id = r["validator_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["validator_id"]),
							validation_date = r["validation_date"].ToString(),
							validation_status = r["validation_status"].ToString(),
							booking_agent = r["booking_agent"] == DBNull.Value ? 0 : Convert.ToInt32(r["booking_agent"]),
							validated_by = userrole.ToString() == "ADMIN" && r["validation_status"].ToString() == "YES" ? r["validated_by"].ToString() : "",
							validateButtonStatus = userrole.ToString() == "ADMIN" && r["res_status"].ToString() == "COMPLETED" && r["validation_status"].ToString() != "YES" ? 1 : 0
						}
						);

				}


				response.Data = reservations;
				response.ActionStatus = "SUCCESS";



			}
			catch (Exception ex)
			{
				response.ActionStatus = "EXCEPTION";
				response.Message = "Something went wrong";
			}
			return new OkObjectResult(response);


		}

		[HttpGet, Route("get-reserve-rooms")]//
		public IActionResult GetReserveRooms(int Id)
		{
			ApiResponse response = new ApiResponse();
			response.ActionStatus = "FAILURE";
			response.Message = string.Empty;

			try
			{

				List<ReservedRoomData> reservedrooms = new List<ReservedRoomData>();
				MySqlHelper sqlHelper = new MySqlHelper();

				var query = string.Format(@"select t1.id, t1.from_date, t1.to_date, t1.room_id, t2.title, t1.years06, t1.years612, t1.years12, t1.amount, IFNULL(t1.discount, 0) discount, IFNULL(t1.new_ba, 0) new_ba, IFNULL(t1.comments, '') comments
                                            from reserve_rooms t1
                                            left join room t2 on t2.id = t1.room_id
                                            where t1.reservation_id = '{0}'  ORDER BY t1.id"
								  , Id);

				DataSet ds = sqlHelper.GetDatasetByMySql(query);


				foreach (DataRow r in ds.Tables[0].Rows)
				{
					TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

					//DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);

					reservedrooms.Add(
						new ReservedRoomData
						{
							id = Convert.ToInt32(r["id"].ToString()),
							fromDate = DateTime.Parse(r["from_date"].ToString()),
							toDate = DateTime.Parse(r["to_date"].ToString()),
							roomId = r["room_id"].ToString(),
							room = r["title"].ToString(),
							years06 = Convert.ToInt32(r["years06"].ToString()),
							years612 = Convert.ToInt32(r["years612"].ToString()),
							years12 = Convert.ToInt32(r["years12"].ToString()),
							noOfGuests = Convert.ToInt32(r["years06"].ToString()) + Convert.ToInt32(r["years612"].ToString()) + Convert.ToInt32(r["years12"].ToString()),
							amount = Convert.ToDecimal(r["amount"].ToString()),
							discount = Convert.ToDecimal(r["discount"].ToString()),
							newbamt = Convert.ToDecimal(r["new_ba"].ToString()),
							comments = r["comments"].ToString()
						}
						);

				}


				response.Data = reservedrooms;
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
        public IActionResult Create([FromBody] ReservData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //check if user has permmision to this property
                var propertyId = 0;
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}",model.propertyId, userId);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());
                           

                        }
                    }
                }

                if (model != null && propertyId==int.Parse(model.propertyId))
                {
                    if(checkIfInventory(model.fromDate,model.toDate,0,int.Parse(model.roomId),int.Parse(model.propertyId)
                        ,int.Parse(model.noOfRooms)))
                    {

                        if (updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                       , int.Parse(model.noOfRooms)))
                        {

                            sqlHelper = new MySqlHelper();

                            query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id,details,created_by,noOfRooms,is_host_booking,no_of_guests)
                              VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},'{11}',{12});
                                         SELECT LAST_INSERT_ID() AS id;",
                                            model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                            model.custName, model.custEmail, model.custPhone, 45, model.details, userId, model.noOfRooms, 'Y', model.noOfGuests);

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
                       

                        
                        //reduce inventory

                       

                    }
                    else
                    {
                        response.Message = "Inventory not available";
                    }
                    

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

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] ReservData model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //get model.propertyId
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from reservation where id={0}", id);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            model.propertyId = r["property_id"].ToString();


                        }
                    }
                }

                //check if user has permmision to this property
                var propertyId = 0;
                sqlHelper = new MySqlHelper();
                query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}", model.propertyId, userId);
                ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());


                        }
                    }
                }

                if (model != null && propertyId == int.Parse(model.propertyId))
                {

                    sqlHelper = new MySqlHelper();
                    

                     query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',details='{3}',updated_by={4}, updated_on='{5}', no_of_guests={6} WHERE id={7} AND is_host_booking='Y' AND property_id={8}",
                                    model.custName, model.custEmail, model.custPhone,
                                    model.details, userId, DateTime.Now.ToString("yyyy-MM-dd"),model.noOfGuests, id,propertyId);

                     ds = sqlHelper.GetDatasetByMySql(query);

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

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete")]
        public IActionResult Delete(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                ReservData model = new ReservData();

                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //get model.propertyId
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from reservation where id={0}", id);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            model.propertyId = r["property_id"].ToString();


                        }
                    }
                }
                var propertyId = 0;
                sqlHelper = new MySqlHelper();
                query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}", model.propertyId, userId);
                ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());


                        }
                    }
                }

                if (id > 0 && propertyId == int.Parse(model.propertyId))
                {

                    sqlHelper = new MySqlHelper();

                    query = string.Format(@"UPDATE reservation SET is_active='N' WHERE is_host_booking='Y' AND id={0} AND property_id={1}",
                           id, propertyId);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    //increase inventory
                    query = string.Format(@"select * 
                     FROM reservation WHERE id={0}", id);

                     ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var noOfRooms = Convert.ToInt32(r["noOfRooms"].ToString());                                    
                                    var roomId = Convert.ToInt32(r["room_id"].ToString());
                                    var startDate = DateTime.Parse(r["from_date"].ToString());
                                    var endDate = DateTime.Parse(r["to_date"].ToString());
                                    while (startDate < endDate)
                                    {
                                        query = string.Format(@"update inventory set booked_qty=booked_qty-{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                        , noOfRooms, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                        ds = sqlHelper.GetDatasetByMySql(query);
                                        startDate = startDate.AddDays(1);
                                    }
                                    

                                }
                            }
                        }
                    }

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

        bool checkIfReservation(DateTime startDate, DateTime endDate, int thisId,int roomId,int propertyId)
        {

            try
            {
               

                List<ReservData> reservations =(List<ReservData>)(((ApiResponse)((OkObjectResult)this.GetList(roomId)).Value).Data);
                foreach (var  reservData in reservations)
                {
                    if (thisId != reservData.id)
                    {
                        var resFromDate = reservData.fromDate;
                        var resToDate = reservData.toDate;

                        if (((startDate >= resFromDate) && (startDate < resToDate))
                          || ((endDate > resFromDate) && (endDate <= resToDate)))
                        {
                            return true;
                        }
                        else if (((resFromDate >= startDate) && (resFromDate < endDate))
                          || ((resToDate > startDate) && (resToDate <= endDate)))
                        {
                            return true;
                        }
                    }
                   
                }
            }
            catch(Exception ex)
            {
                return true;
            }

            return false;
        }

        bool checkIfInventory(DateTime startDate, DateTime endDate, int thisQty, int roomId, int propertyId, int noOfRooms)
        {

            try
            {
                bool hasInventory = true;

                
                MySqlHelper sqlHelper = new MySqlHelper();

                while(hasInventory&&(startDate < endDate))
                {
                    //check inventory for the day
                    hasInventory = false;
                    var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
                                                left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
                                  , propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null){
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var totalQty = Convert.ToInt32(r["total_qty"].ToString());
                                    var bookedQty = Convert.ToInt32(r["booked_qty"].ToString());
                                    var availableQty = totalQty - bookedQty+thisQty;
                                    if (availableQty >= noOfRooms)
                                    {
                                        hasInventory = true;
                                        
                                    }
                                }
                            }
                        }
                    }
                    startDate = startDate.AddDays(1);
                }
                return hasInventory;
            }
            catch (Exception ex)
            {
                
            }

            return false;
        }
        bool updateInventory(DateTime startDate, DateTime endDate, int thisQty, int roomId, int propertyId, int noOfRooms)
        {

            try
            {
                bool hasInventory = true;

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                while (hasInventory && (startDate < endDate))
                {
                    //check inventory for the day
                    hasInventory = false;
                    var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
                                                left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
                                  , propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var totalQty = Convert.ToInt32(r["total_qty"].ToString());
                                    var bookedQty = Convert.ToInt32(r["booked_qty"].ToString());
                                    var availableQty = totalQty - bookedQty + thisQty;
                                    if (availableQty >= noOfRooms)
                                    {
                                        

                                        query = string.Format(@"update inventory set booked_qty=booked_qty+{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                        ,noOfRooms-thisQty, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                        ds = sqlHelper.GetDatasetByMySql(query);
                                        hasInventory = true;

                                    }
                                }
                            }
                        }
                    }
                    startDate = startDate.AddDays(1);
                }
                return hasInventory;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
    }
}


