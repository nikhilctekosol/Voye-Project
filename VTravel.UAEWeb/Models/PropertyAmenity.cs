﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.UAEWeb.Models
{
    public class PropertyAmenity
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public int status { get; set; }
        public string amenityName { get; set; }
        public string image1 { get; set; }

    }
  
}
