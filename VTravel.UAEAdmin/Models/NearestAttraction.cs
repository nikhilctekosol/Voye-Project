namespace VTravel.UAEAdmin.Models
{
    public class NearestAttraction
    {
        public class Attraction
        {
            public int id { get; set; }
            public int propertyid { get; set; }
            public string attractionName { get; set; }
            public string image { get; set; }
            public string attractLocation { get; set; }
            public double distance { get; set; }
        }
    }
}
