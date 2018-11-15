namespace Allied.RealTime.Example.Web.Models
{
    public class BalanceRequest
    {
        // Financial institution RTN
        public string routingTransitNumber { get; set; }

        // Customer account number
        public string accountNumber { get; set; }

        // TODO: place any additional properties necessary to 
        // identify an individual account below.
    }
}
