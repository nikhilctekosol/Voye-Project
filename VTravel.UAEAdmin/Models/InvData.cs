﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEAdmin.Models
{
    public class InvData
    {
        public int id { get; set; }
        public DateTime invDate { get; set; }       
        public string roomId { get; set; }
        public string propertyId { get; set; }
        public int totalQty { get; set; }
        public int bookedQty { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public double price { get; set; }
        public double extraBedPrice { get; set; }
        public double childPrice { get; set; }
        public string mode { get; set; }
        public string occrates { get; set; }
        //public int normalocc { get; set; }
        //public int maxadults { get; set; }
        //public int maxchildren { get; set; }
        //public double years06 { get; set; }
        //public double years612 { get; set; }
        //public double years12 { get; set; }

    }
}
