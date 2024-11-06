namespace VTravel.UAEAdmin.Models
{
    public class TripCompletion
    {
        public int id { get; set; }
        public string CustName { get; set; }
        public string CustEmail { get; set; }
        public string CustPhone { get; set; }
        public string Property { get; set; }
        public decimal FinalAmount { get; set; }
        public string Agent { get; set; }
        public string Permission { get; set; }
        public int Mode { get; set; }
    }
}
