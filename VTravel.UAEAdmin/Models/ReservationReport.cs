using System;
using System.Collections.Generic;

namespace VTravel.UAEAdmin.Models
{

    public class ReservationReportResposne
    {
        public string reservation_id { get; set; }
        public string property { get; set; }
        public string created_by { get; set; }
        public string created_on { get; set; }
        public string cust_name { get; set; }
        public string cust_email { get; set; }
        public string cust_phone { get; set; }
        public string channel_name { get; set; }
        public int noOfRooms { get; set; }
        public int no_of_guests { get; set; }
        public string final_amount { get; set; }
        public string advancepayment { get; set; }
        public string partpayment { get; set; }
        public string balancepayment { get; set; }
        public string Less_Than_6_Years { get; set; }
        public string childeren_6_to_11_years { get; set; }
        public string tds { get; set; }
        public string ot_commission { get; set; }
        public string details { get; set; }
        public string calc_amount { get; set; }
        public string discount { get; set; }
        public string booking_amount { get; set; }
        public string comments { get; set; }
        public string booking_agent { get; set; }
    }
}

