using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MySql.Data.MySqlClient;
using Dapper;
using System.Transactions;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace VTravel.Admin.Controllers
{

    [Route("api/reservation")
        , Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")

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
        public IActionResult GetList(int roomId, int propertyId)
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

                var query = string.Format(@"select t1.id,t1.from_date,t1.to_date,t1.customer_id,t1.room_id,t1.property_id,t1.cust_name,t1.cust_email,t1.cust_phone,t1.booking_channel_id,t1.details
                ,t1.noOfRooms,t1.no_of_guests,t1.final_amount, t1.is_host_booking,t1.created_on,t1.updated_on,t1.enquiry_ref,t1.res_status,t2.user_name,t2.name_of_user ,t3.user_name AS updated_user_name,t3.name_of_user  AS updated_name_of_user
                , t1.advancepayment, t1.partpayment, t1.balancepayment, (case when curdate() = date_add(t1.to_date, interval 1 day) then (case when t1.completion_enabled = 'Y' then 1 else 0 end)
                when curdate() >= date_add(t1.to_date, interval 2 day) then 0 else 1 end) user_permission, t1.validator_id, t1.validation_date, t1.validation_status, t1.booking_agent
                , IFNULL(IFNULL(t4.name_of_user, t4.user_name), '') validated_by
                FROM reservation t1 LEFT JOIN admin_user t2 ON t1.created_by=t2.id
                LEFT JOIN admin_user t3 ON t1.updated_by=t3.id 
                LEFT JOIN admin_user t4 ON t1.validator_id=t4.id 
                WHERE t1.is_active='Y' AND t1.room_id={0} AND t1.property_id={1} AND t1.from_date > '{2}'  ORDER BY from_date"
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
                            created_on = String.IsNullOrEmpty(r["created_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["created_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            updated_on = String.IsNullOrEmpty(r["updated_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["updated_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            created_by = r["is_host_booking"].ToString() == "Y" ? "Host" : r["user_name"].ToString() + "/" + r["name_of_user"].ToString(),
                            updated_by = r["is_host_booking"].ToString() == "Y" ? "NA" : r["updated_user_name"].ToString() + "/" + r["updated_name_of_user"].ToString(),
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
                            amount = Convert.ToInt32(r["amount"].ToString()),
                            discount = Convert.ToInt32(r["discount"].ToString()),
                            newbamt = Convert.ToInt32(r["new_ba"].ToString()),
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

        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] ReservData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    if (checkIfInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                        , int.Parse(model.noOfRooms)))
                    {
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                        //reduce inventory

                        if (updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                        , int.Parse(model.noOfRooms)))
                        {
                            MySqlHelper sqlHelper = new MySqlHelper();

                            var query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id
                                        ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment, booking_agent)
                                        VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11},{12},'{13}','{14}','{15}','{16}','{17}');
                                        SELECT LAST_INSERT_ID() AS id;",
                                             model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                             model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                                             , model.advancepayment, model.partpayment, model.balancepayment, model.booking_agent);

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

        [HttpPost, Route("create-new")]
        public IActionResult CreateNew([FromBody] ReservDataNew model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            using (var scope = new TransactionScope())
            {
                try
                {
                    if (model != null)
                    {
                        //for (int i = 0; i < model.rooms.Count; i++)
                        //{
                        //    if (!checkIfInventory(model.rooms[i].fromDate, model.rooms[i].toDate, 0, int.Parse(model.rooms[i].roomId), int.Parse(model.propertyId), model.rooms.Count(r => r.roomId == model.rooms[i].roomId)))
                        //    {
                        //        response.ActionStatus = "EXCEPTION";
                        //        response.Message = "Inventory doesn't matches the reservation!";
                        //        throw new Exception();
                        //    }
                        //}
                        if (!checkIfInventorynew(model))
                        {
                            response.ActionStatus = "EXCEPTION";
                            response.Message = "Inventory doesn't matches the reservation!";
                            throw new Exception();
                        }

                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        for (int i = 0; i < model.rooms.Count; i++)
                        {
                            if (!updateInventory(model.rooms[i].fromDate, model.rooms[i].toDate, 0, int.Parse(model.rooms[i].roomId), int.Parse(model.propertyId), 1))
                            {
                                response.ActionStatus = "EXCEPTION";
                                response.Message = "Something went wrong in inventory update!";
                                throw new Exception();
                            }
                        }

                        MySqlHelper sqlHelper = new MySqlHelper();

                        var query = string.Format(@"INSERT INTO reservation(property_id,cust_name,cust_email,cust_phone,booking_channel_id
                                                ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment, discount, commission, country, tds, booking_agent)
                                                VALUES({0},'{1}','{2}','{3}',{4},'{5}',{6},{7},{8},{9},'{10}',{11},{12},{13},{14},{15}, '{16}', {17}, {18});
                                                SELECT LAST_INSERT_ID() AS id;",
                                         model.propertyId,
                                         model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                                         , model.advancepayment, model.partpayment, model.balancepayment, 0, model.commission, model.country, model.tds, model.booking_agent);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    model.id = Convert.ToInt32(r["id"].ToString());

                                    for (int i = 0; i < model.rooms.Count; i++)
                                    {
                                        query = string.Format(@"INSERT INTO reserve_rooms(reservation_id,from_date,to_date,room_id,years06, years612, years12, amount, discount, new_ba, comments)
                                                VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7}, {8}, {9}, '{10}');",
                                         model.id, model.rooms[i].fromDate.ToString("yyyy-MM-dd"), model.rooms[i].toDate.ToString("yyyy-MM-dd"), model.rooms[i].roomId, model.rooms[i].years06, model.rooms[i].years612
                                         , model.rooms[i].years12, model.rooms[i].amount, model.rooms[i].discount, model.rooms[i].newbamt, model.rooms[i].comments);

                                        DataSet ds1 = sqlHelper.GetDatasetByMySql(query);
                                    }



                                    response.Data = model;
                                    response.ActionStatus = "SUCCESS";

                                }
                            }
                        }
                    }


                    //if (model != null)
                    //{
                    //    if (checkIfInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                    //        , int.Parse(model.noOfRooms)))
                    //    {
                    //        IEnumerable<Claim> claims = User.Claims;
                    //        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                    //        //reduce inventory

                    //        if (updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                    //        , int.Parse(model.noOfRooms)))
                    //        {
                    //            MySqlHelper sqlHelper = new MySqlHelper();

                    //            var query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id
                    //                        ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment)
                    //                        VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11},{12},'{13}','{14}','{15}','{16}');
                    //                        SELECT LAST_INSERT_ID() AS id;",
                    //                             model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                    //                             model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                    //                             , model.advancepayment, model.partpayment, model.balancepayment);

                    //            DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    //            if (ds != null)
                    //            {
                    //                if (ds.Tables.Count > 0)
                    //                {
                    //                    if (ds.Tables[0].Rows.Count > 0)
                    //                    {
                    //                        DataRow r = ds.Tables[0].Rows[0];
                    //                        model.id = Convert.ToInt32(r["id"].ToString());
                    //                        response.Data = model;
                    //                        response.ActionStatus = "SUCCESS";

                    //                    }
                    //                }
                    //            }

                    //        }

                    //    }
                    //    else
                    //    {
                    //        response.Message = "Inventory not available";
                    //    }


                    //}
                    //else
                    //{
                    //    return BadRequest("Invalid details");
                    //}

                    scope.Complete();
                }
                catch (Exception ex)//
                {
                    response.ActionStatus = "EXCEPTION";
                    response.Message = "Something went wrong";
                }

            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] ReservData model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();//
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;



                    var query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',booking_channel_id={3},details='{4}'
                                    ,updated_by={5}, updated_on='{6}',no_of_guests={7},final_amount={8},enquiry_ref='{9}',advancepayment='{11}'
                                    ,partpayment='{12}',balancepayment='{13}', booking_agent = {14} WHERE id={10}",
                                    model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId
                                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.noOfGuests, model.finalAmount, model.enquiry_ref, id
                                    , model.advancepayment, model.partpayment, model.balancepayment, model.booking_agent);

                    var ds = sqlHelper.GetDatasetByMySql(query);

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

        [HttpPut, Route("update-new")]
        public IActionResult UpdateNew([FromBody] ReservDataNew model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();//
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    var roomquery = string.Format(@"select t1.id, t1.from_date, t1.to_date, t1.room_id, t2.title, t1.years06, t1.years612, t1.years12, t1.amount, IFNULL(t1.discount, 0) discount, IFNULL(t1.new_ba, 0) new_ba, IFNULL(t1.comments, '') comments, r.property_id
                                            from reserve_rooms t1
                                            left join room t2 on t2.id = t1.room_id
                                            inner join reservation r on t1.reservation_id = r.id
                                            where t1.reservation_id = '{0}'  ORDER BY t1.id"
                                  , id);

                    DataSet roomds = sqlHelper.GetDatasetByMySql(roomquery);


                    var query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',booking_channel_id={3},details='{4}'
                                        ,updated_by={5}, updated_on='{6}',no_of_guests={7},final_amount={8},enquiry_ref='{9}',advancepayment='{11}'
                                        ,partpayment='{12}',balancepayment='{13}',discount={14},commission={15}, country = '{16}', tds = '{17}', booking_agent = {18} WHERE id={10}",
                                        model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId
                                        , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.noOfGuests, model.finalAmount, model.enquiry_ref, id
                                        , model.advancepayment, model.partpayment, model.balancepayment, model.discount, model.commission, model.country, model.tds, model.booking_agent);

                    var ds = sqlHelper.GetDatasetByMySql(query);

                    if ((roomds != null) && (roomds.Tables[0].Rows.Count != model.rooms.Count))
                    {
                        string deleteId = roomds.Tables[0].Rows
                            .Cast<DataRow>()
                            .Select(row => row["id"])
                            .FirstOrDefault(id => !model.rooms.Any(room => room.id == Convert.ToInt32(id))).ToString();

                        if (deleteId != null)
                        {
                            var detailsToDelete = roomds.Tables[0].AsEnumerable()
                                .Where(row => row["ID"].ToString() == deleteId)
                                .Select(row => new
                                {
                                    room_id = row["room_id"].ToString(),
                                    property_id = row["property_id"].ToString(),
                                    from_date = row["from_date"].ToString()
                                })
                                .FirstOrDefault();


                            query = string.Format(@"DELETE FROM reserve_rooms where reservation_id = {0} and id = {1}",
                                                  id, deleteId);

                            var ds1 = sqlHelper.GetDatasetByMySql(query);

                            DateTime parsedDate = DateTime.Parse(detailsToDelete.from_date);
                            string from_date = parsedDate.ToString("yyyy-MM-dd");

                            query = string.Format(@"update inventory set booked_qty=booked_qty-{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                            , 1, detailsToDelete.property_id, detailsToDelete.room_id, from_date.ToString());

                            var ds2 = sqlHelper.GetDatasetByMySql(query);
                        }
                    }
                    else
                    {

                        for (int i = 0; i < model.rooms.Count; i++)
                        {
                            query = string.Format(@"update reserve_rooms set years06 = {0}, years612 = {1}, years12 = {2}, amount = {3}, discount = {4}, new_ba = {5}, comments = '{6}'
                                                    where id = {7};",
                             model.rooms[i].years06, model.rooms[i].years612, model.rooms[i].years12, model.rooms[i].amount, model.rooms[i].discount, model.rooms[i].newbamt, model.rooms[i].comments, model.rooms[i].id);

                            DataSet ds1 = sqlHelper.GetDatasetByMySql(query);
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

                    var query = string.Format(@"UPDATE reservation SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

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
                                    var propertyId = Convert.ToInt32(r["property_id"].ToString());
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


        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-new")]
        public IActionResult DeleteNew(int id)
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

                    //increase inventory
                    query = string.Format(@"select t1.id, t2.from_date, t2.to_date, 1 noOfRooms, t1.property_id, t2.room_id 
                     FROM reservation t1
                     LEFT JOIN reserve_rooms t2 on t2.reservation_id = t1.id WHERE t1.id={0}", id);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        DataRow r = ds.Tables[0].Rows[i];
                                        var noOfRooms = Convert.ToInt32(r["noOfRooms"].ToString());
                                        var propertyId = Convert.ToInt32(r["property_id"].ToString());
                                        var roomId = Convert.ToInt32(r["room_id"].ToString());
                                        var startDate = DateTime.Parse(r["from_date"].ToString());
                                        var endDate = DateTime.Parse(r["to_date"].ToString());
                                        while (startDate < endDate)
                                        {
                                            query = string.Format(@"update inventory set booked_qty=booked_qty-{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                            , noOfRooms, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                            DataSet ds1 = sqlHelper.GetDatasetByMySql(query);
                                            startDate = startDate.AddDays(1);
                                        }
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

        bool checkIfReservation(DateTime startDate, DateTime endDate, int thisId, int roomId, int propertyId)
        {

            try
            {


                List<ReservData> reservations = (List<ReservData>)(((ApiResponse)((OkObjectResult)this.GetList(roomId, propertyId)).Value).Data);
                foreach (var reservData in reservations)
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
            catch (Exception ex)
            {
                return true;
            }

            return false;
        }

        bool checkIfInventory(DateTime startDate, DateTime endDate, int thisQty, int roomId, int propertyId, int noOfRooms)
        {

            try
            {
                if (startDate < DateTime.Today)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userRole = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                    if (userRole != "ADMIN")
                    {
                        return false;
                    }

                }
                bool hasInventory = true;


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
                                        , noOfRooms - thisQty, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

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



        [HttpPut, Route("add-doc"), DisableRequestSizeLimit]
        public async Task<IActionResult> AddDoc(int resid, int doctype)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;
            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                var files = Request.Form.Files;


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
                            Folder = "reservation/" + resid + "/docs",
                            Overwrite = true,
                            PublicId = guid + file.FileName,
                            Invalidate = true
                        };


                        var uploadResult = cloudinary.Upload(uploadParams);
                        System.IO.File.Delete(filePath);

                        if (uploadResult.Url != null)
                        {
                            var query = string.Format(@"INSERT INTO res_document(res_id,url,public_id,doc_type_id,created_by,file_name) VALUES({0},'{1}','{2}',{3},{4},'{5}')",
                                   resid, uploadResult.SecureUrl, guid, doctype, userId, file.FileName);

                            using (var connection = new MySqlConnection(Startup.conStr))
                            {//

                                var results = connection.Query(query);
                                //response.Data = (List<DocType>)results;
                                response.ActionStatus = "SUCCESS";

                            }


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

        [HttpPut, Route("create-add-doc"), DisableRequestSizeLimit]
        public async Task<IActionResult> CreateAddDoc(int resid, int doctype)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;
            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                var files = Request.Form.Files;


                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var doc_details = file.FileName.Split('|');
						var filename = "";
						if (doc_details.Length == 3)
						{
							filename = resid + doc_details[1] + doc_details[2];
						}
						else
						{
							filename = resid + doc_details[1];
						}
						var doc_id = doc_details[0];
                        //var filename = resid + doc_details[1] + doc_details[2];
                        var guid = Guid.NewGuid().ToString();
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, guid + filename);
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
                            File = new FileDescription(filename, filePath),
                            Folder = "reservation/" + resid + "/docs",
                            Overwrite = true,
                            PublicId = guid + filename,
                            Invalidate = true
                        };


                        var uploadResult = cloudinary.Upload(uploadParams);
                        System.IO.File.Delete(filePath);

                        if (uploadResult.Url != null)
                        {
                            var query = string.Format(@"INSERT INTO res_document(res_id,url,public_id,doc_type_id,created_by,file_name) VALUES({0},'{1}','{2}',{3},{4},'{5}')",
                                   resid, uploadResult.SecureUrl, guid, doc_id, userId, filename);

                            using (var connection = new MySqlConnection(Startup.conStr))
                            {//

                                var results = connection.Query(query);
                                //response.Data = (List<DocType>)results;
                                response.ActionStatus = "SUCCESS";

                            }


                        }
                        response.ActionStatus = "SUCCESS";
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


        [HttpGet, Route("get-doc-list")]
        public IActionResult GetList(int resid)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {



                var query = string.Format(@"select t1.id,t1.doc_type_id,t1.url,t1.file_name,t2.doc_type_name 
                 FROM res_document t1 LEFT JOIN doc_type t2 ON t1.doc_type_id=t2.id 
                WHERE t1.is_active='Y' AND t1.res_id={0} ORDER BY t1.sort_order"
                                   , resid);

                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var results = connection.Query<ResDoc>(query);
                    response.Data = (List<ResDoc>)results;
                    response.ActionStatus = "SUCCESS";

                }




            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpDelete, Route("delete-doc")]
        public IActionResult DeleteDoc(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {



                var query = string.Format(@"update  res_document SET is_active='N' WHERE id={0}"
                                   , id);

                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var results = connection.Query(query);

                    response.ActionStatus = "SUCCESS";

                }




            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpPut, Route("update-status")]
        public IActionResult UpdateStatus([FromBody] ReservData model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {


                if (model != null)
                {
                    MySqlHelper sqlHelper = new MySqlHelper();//
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    //verify docs
                    if (checkIfDocument(id))
                    {
                        var query = string.Format(@"UPDATE reservation SET res_status='{0}',updated_by={1}, updated_on='{2}' WHERE id={3}",
                                  model.res_status, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);


                        using (var connection = new MySqlConnection(Startup.conStr))
                        {

                            var results = connection.Query(query);

                            response.ActionStatus = "SUCCESS";

                        }

                    }
                    else
                    {
                        response.Message = "Please update all required documents";
                    }





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

        bool checkIfDocument(int id)
        {

            try
            {

                var idProofCountRequired = 1;
                var advancePaymentCountRequired = 1;
                var finalPaymentCountRequired = 1;

                var idProofCountAvailable = 0;
                var advancePaymentCountAvailable = 0;
                var finalPaymentCountAvailable = 0;




                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var query = string.Format(@"SELECT doc_type_id,count(1) AS doc_count FROM res_document where res_id={0} AND is_active='Y' group by doc_type_id"
                                , id);
                    //idProofCountRequired = connection.ExecuteScalar<int>
                    //   (string.Format("SELECT no_of_guests FROM reservation WHERE id={0}",id));
                    var results = connection.Query<DocCount>(query);
                    var countList = (List<DocCount>)results;
                    foreach (var docCount in countList)
                    {
                        if (docCount.doc_type_id == 1)
                        {
                            idProofCountAvailable += docCount.doc_count;
                        }
                        else if (docCount.doc_type_id == 4)
                        {
                            advancePaymentCountAvailable += docCount.doc_count;
                        }
                        else if (docCount.doc_type_id == 5)
                        {
                            finalPaymentCountAvailable += docCount.doc_count;
                        }
                    }

                    if ((idProofCountRequired <= idProofCountAvailable)
                       && (advancePaymentCountRequired <= advancePaymentCountAvailable)
                        && (finalPaymentCountRequired <= finalPaymentCountAvailable))
                    {
                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        [AllowAnonymous]
        [HttpGet, Route("first-email")]
        public IActionResult FirstEmail()
        {
            try
            {
                TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime utcDateTime = DateTime.UtcNow;
                var now = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, indianTimeZone);

                MySqlHelper MySqlHelper = new MySqlHelper();

                var tokenKey = Guid.NewGuid().ToString();

                string emailSubject = "";
                emailSubject = "First Email Reminder for Reservation Completion";

                string query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='complete_res_first_email'";
                DataSet ds = MySqlHelper.GetDatasetByMySql(query);

                var emailBody = ds.Tables[0].Rows[0]["content"].ToString();

                query = string.Format(@"select r.id reservation_id, r.cust_name, r.cust_email, r.cust_phone, p.title property, r.final_amount, r.created_by, IFNULL(ba.user_name, u.user_name) mail, IFNULL(ba.name_of_user, u.name_of_user) name_of_user, r.from_date checkin from reservation r
                    left join property p on p.id = r.property_id
                    left join admin_user u on u.id = r.created_by
                    left join admin_user ba on ba.id = r.booking_agent
                    where r.is_active = 'Y' and r.res_status != 'COMPLETED' and r.from_date is not null
                    group by r.id, r.cust_name, r.cust_email, r.cust_phone, p.title, r.final_amount, r.created_by, u.user_name, ba.user_name
                    having  r.from_date = DATE_ADD('{0}', interval -1 DAY)
                    union
                    select r.id reservation_id, r.cust_name, r.cust_email, r.cust_phone, p.title property, r.final_amount, r.created_by, IFNULL(ba.user_name, u.user_name) mail, IFNULL(ba.name_of_user, u.name_of_user) name_of_user, MIN(rr.from_date) checkin from reservation r
                    left join reserve_rooms rr on rr.reservation_id = r.id
                    left join property p on p.id = r.property_id
                    left join admin_user u on u.id = r.created_by
                    left join admin_user ba on ba.id = r.booking_agent
                    where r.is_active = 'Y' and r.res_status != 'COMPLETED'
                    group by r.id, r.cust_name, r.cust_email, r.cust_phone, p.title, r.final_amount, r.created_by, u.user_name, ba.user_name
                    having  MIN(rr.from_date) = DATE_ADD('{0}', interval -1 DAY);", now.ToString("yyyy-MM-dd"));

                ds = MySqlHelper.GetDatasetByMySql(query);
                string tomail = string.Empty;
                string content = string.Empty;
                Boolean result = false;

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var distinctValues = ds.Tables[0].AsEnumerable()
                                         .Select(row => row.Field<int>("created_by"))
                                         .Distinct();

                        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                        foreach (var value in distinctValues)
                        {
                            string body = string.Empty;
                            int i = 0;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (Convert.ToInt32(dr["created_by"]) == value)
                                {
                                    body = body + "<tr>"
                                        + "<td style=\"border:1px solid black;\">" + (i + 1).ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["reservation_id"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["name_of_user"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + (String.IsNullOrEmpty(dr["checkin"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(dr["checkin"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy")) + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_name"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_email"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_phone"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["property"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["final_amount"].ToString() + "</td>"
                                        + "</tr>";
                                    tomail = dr["mail"].ToString();
                                    i += 1;
                                }

                            }

                            content = emailBody.Replace("#BODY#", body);

                            var emailResponse = General.SendMailWithCC(emailSubject,
                                content, tomail,
                               General.GetSettingsValue("reservation_notification_from"),
                                General.GetSettingsValue("reservation_notofication_display_name"), "");

                            if (emailResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                result = true;
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                    }
                    else
                    {
                        return NoContent();
                    }
                }

                if (result)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet, Route("second-email")]
        public IActionResult SecondEmail()
        {
            try
            {
                TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime utcDateTime = DateTime.UtcNow;
                var now = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, indianTimeZone);

                MySqlHelper MySqlHelper = new MySqlHelper();

                var tokenKey = Guid.NewGuid().ToString();

                string emailSubject = "";
                emailSubject = "Second Email Reminder for Reservation Completion";

                string query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='complete_res_second_email'";
                DataSet ds = MySqlHelper.GetDatasetByMySql(query);

                var emailBody = ds.Tables[0].Rows[0]["content"].ToString();

                query = string.Format(@"select r.id reservation_id, r.cust_name, r.cust_email, r.cust_phone, p.title property, r.final_amount, r.created_by, IFNULL(ba.user_name, u.user_name) mail, IFNULL(ba.name_of_user, u.name_of_user) name_of_user, r.from_date checkin from reservation r
                    left join property p on p.id = r.property_id
                    left join admin_user u on u.id = r.created_by
                    left join admin_user ba on ba.id = r.booking_agent
                    where r.is_active = 'Y' and r.res_status != 'COMPLETED'
                    group by r.id, r.cust_name, r.cust_email, r.cust_phone, p.title, r.final_amount, r.created_by, u.user_name, r.to_date, ba.user_name
                    having r.to_date = '{0}' and datediff(r.to_date, r.from_date) > 1
					union
					select r.id reservation_id, r.cust_name, r.cust_email, r.cust_phone, p.title property, r.final_amount, r.created_by, IFNULL(ba.user_name, u.user_name) mail, IFNULL(ba.name_of_user, u.name_of_user) name_of_user, MIN(rr.from_date) checkin from reservation r
                    left join reserve_rooms rr on rr.reservation_id = r.id
                    left join property p on p.id = r.property_id
                    left join admin_user u on u.id = r.created_by
                    left join admin_user ba on ba.id = r.booking_agent
                    where r.is_active = 'Y' and r.res_status != 'COMPLETED'
                    group by r.id, r.cust_name, r.cust_email, r.cust_phone, p.title, r.final_amount, r.created_by, u.user_name, ba.user_name
                    having MIN(rr.to_date) = '{0}' and datediff(MIN(rr.to_date), MIN(rr.from_date)) > 1;", now.ToString("yyyy-MM-dd"));

                ds = MySqlHelper.GetDatasetByMySql(query);
                string tomail = string.Empty;
                string content = string.Empty;
                Boolean result = false;

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var distinctValues = ds.Tables[0].AsEnumerable()
                                         .Select(row => row.Field<int>("created_by"))
                                         .Distinct();

                        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        foreach (var value in distinctValues)
                        {
                            string body = string.Empty;
                            int i = 0;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (Convert.ToInt32(dr["created_by"]) == value)
                                {
                                    body = body + "<tr>"
                                        + "<td style=\"border:1px solid black;\">" + (i + 1).ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["reservation_id"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["name_of_user"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + (String.IsNullOrEmpty(dr["checkin"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(dr["checkin"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy")) + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_name"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_email"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["cust_phone"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["property"].ToString() + "</td>"
                                        + "<td style=\"border:1px solid black;\">" + dr["final_amount"].ToString() + "</td>"
                                        + "</tr>";
                                    tomail = dr["mail"].ToString();
                                    i += 1;
                                }
                            }

                            content = emailBody.Replace("#BODY#", body);

                            var emailResponse = General.SendMailWithCC(emailSubject,
                                content, tomail,
                               General.GetSettingsValue("reservation_notification_from"),
                                General.GetSettingsValue("reservation_notofication_display_name"), "");

                            if (emailResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                result = true;
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                    }
                    else
                    {
                        return NoContent();
                    }
                }

                if (result)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }


        [HttpGet, Route("get-reservation")]//
        public IActionResult GetReservation(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                ReservData reservation = new ReservData();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select t1.id,t1.room_id,t1.property_id, t2.title property
                FROM reservation t1 LEFT JOIN property t2 ON t1.property_id=t2.id
                WHERE t1.id = {0};", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                reservation =
                    new ReservData
                    {
                        id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString()),
                        roomId = ds.Tables[0].Rows[0]["room_id"].ToString(),
                        propertyId = ds.Tables[0].Rows[0]["property_id"].ToString(),
                        property = ds.Tables[0].Rows[0]["property"].ToString()
                    };


                response.Data = reservation;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [AllowAnonymous]
        [HttpGet, Route("wake-up")]
        public IActionResult WakeupCall()
        {
            return Ok();
        }

        bool checkIfInventorynew(ReservDataNew model)
        {

            try
            {
                bool hasInventory = true;

                MySqlHelper sqlHelper = new MySqlHelper();
                for (int i = 0; i < model.rooms.Count; i++)
                {
                    DateTime fromDate = model.rooms[i].fromDate;
                    DateTime toDate = model.rooms[i].toDate;
                    string roomId = model.rooms[i].roomId;

                    if(fromDate < DateTime.Today)
                    {
                        IEnumerable<Claim> claims = User.Claims;
                        var userRole = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                        if (userRole != "ADMIN")
                        {
                            return false;
                        }


                        while (hasInventory && fromDate < toDate)
                        {
                            hasInventory = false;
                            int qty = model.rooms.Count(c => c.roomId == roomId && c.fromDate <= fromDate && fromDate < c.toDate);

                            var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
                                                left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
                                  , model.propertyId, roomId, model.rooms[i].fromDate.ToString("yyyy-MM-dd"));

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
                                            var availableQty = totalQty - bookedQty + 0;
                                            if (availableQty >= qty)
                                            {
                                                hasInventory = true;

                                            }
                                        }
                                    }
                                }
                            }
                            fromDate = fromDate.AddDays(1);
                        }
                    }
                }
                return hasInventory;
            }
            catch(Exception ex)
            {

            }
            return false;





            //try
            //{
            //    if (startDate < DateTime.Today)
            //    {
            //        IEnumerable<Claim> claims = User.Claims;
            //        var userRole = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

            //        if (userRole != "ADMIN")
            //        {
            //            return false;
            //        }

            //    }
            //    //bool hasInventory = true;


            //    MySqlHelper sqlHelper = new MySqlHelper();

            //    while (hasInventory && (startDate < endDate))
            //    {
            //        //check inventory for the day
            //        hasInventory = false;
            //        var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
            //                                    left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
            //                      , propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

            //        DataSet ds = sqlHelper.GetDatasetByMySql(query);
            //        if (ds != null)
            //        {
            //            if (ds.Tables.Count > 0)
            //            {
            //                if (ds.Tables[0].Rows != null)
            //                {
            //                    if (ds.Tables[0].Rows.Count > 0)
            //                    {
            //                        DataRow r = ds.Tables[0].Rows[0];
            //                        var totalQty = Convert.ToInt32(r["total_qty"].ToString());
            //                        var bookedQty = Convert.ToInt32(r["booked_qty"].ToString());
            //                        var availableQty = totalQty - bookedQty + thisQty;
            //                        if (availableQty >= noOfRooms)
            //                        {
            //                            hasInventory = true;

            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        startDate = startDate.AddDays(1);
            //    }
            //    return hasInventory;
            //}
            //catch (Exception ex)
            //{

            //}

            //return false;
        }
        [HttpPut, Route("validate-reservation")]
        public IActionResult ValidateReservation(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();//
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;
                var datetime = DateTime.Now;

                var query = string.Format(@"UPDATE reservation SET validator_id='{0}',validation_date='{1}',validation_status='{2}' WHERE id={3}",
                                    userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "YES", id);

                var ds = sqlHelper.GetDatasetByMySql(query);

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


