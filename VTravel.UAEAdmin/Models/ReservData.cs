﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEAdmin.Models
{
    public class ReservData
    {
        public int id { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string roomId { get; set; }
        public string propertyId { get; set; }
        public string property { get; set; }
        public string customerId { get; set; }
        public string custName { get; set; }       
        public string custEmail { get; set; }
        public string custPhone { get; set; }
        public string bookingChannelId { get; set; }
        public string details { get; set; }
        public string isHostBooking { get; set; }
        public string noOfRooms { get; set; }
        public int noOfGuests { get; set; }
        public float finalAmount { get; set; }
        public float advancepayment { get; set; }
        public float partpayment { get; set; }
        public float balancepayment { get; set; }
        public float discount { get; set; }
        public float commission { get; set; }
        public float tds { get; set; }
        public string country { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string enquiry_ref { get; set; }
        public string res_status { get; set; }
        public string user_permission { get; set; }
        public  int validator_id { get; set; }
        public string validation_date { get; set; }
        public string validation_status { get; set; }
        public int booking_agent { get; set; }
        public string validated_by { get; set; }
        public int validateButtonStatus { get; set; }

    }
}
