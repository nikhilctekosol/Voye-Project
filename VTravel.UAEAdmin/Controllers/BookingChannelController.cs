using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.UAEAdmin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace VTravel.UAEAdmin.Controllers
{
    
    [Route("api/bookingchannel"), Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")]
    public class BookingChannelController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BookingChannelController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;//
        }



        [Authorize(Roles = "ADMIN")]
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

                    var query = string.Format(@"UPDATE room SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE room SET sort_order={1} WHERE id={2}", model.pushDownValue,
                                     model.sortOrder,model.itemId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);


                    
                    response.ActionStatus = "SUCCESS";
                    response.Message ="products sorted";
                }
                else
                {
                    return BadRequest("Invalid sort product");
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

                List<BookingChannel> bookingChannels = new List<BookingChannel>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,channel_name,sort_order, IFNULL(tds, 0) tds
                 FROM booking_channel WHERE is_active='Y' ORDER BY sort_order"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    bookingChannels.Add(
                        new BookingChannel
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            channelName = r["channel_name"].ToString(),
                            tds = Convert.ToDecimal(r["tds"])
                        }
                        );

                }


                response.Data = bookingChannels;
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

                BookingChannel channel = new BookingChannel();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,channel_name,sort_order 
                 FROM booking_channel WHERE id={0} is_active='Y'", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    channel = new BookingChannel
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        channelName = r["channel_name"].ToString()
                    };

                }

                response.Data = channel;
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
        public IActionResult Create([FromBody] BookingChannel model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO booking_channel(channel_name,description, tds) VALUES('{0}','{1}', {2});
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.channelName, model.description, model.tds);

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
        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] BookingChannel model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE booking_channel SET channel_name='{0}',description='{1}', tds={3} WHERE id={2}",
                                     model.channelName, model.description, id, model.tds);

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

                    var query = string.Format(@"UPDATE booking_channel SET is_active='N' WHERE id={0}",
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


