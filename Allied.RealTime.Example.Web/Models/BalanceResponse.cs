using System;

namespace Allied.RealTime.Example.Web.Models
{
    public class BalanceResponse : BaseResponse
    {
        public BalanceResponse()
        {
        }

        public BalanceResponse(Exception ex) :
            base(ex)
        {
        }

        // Available balance
        public decimal balance { get; set; }

        // Financial institution RTN
        public string routingTransitNumber { get; set; }

        // Customer account number
        public string accountNumber { get; set; }

        // TODO: generally the properties above are adequate
        // for determining the viability of a given transaction.
        // However, should there be other properties of
        // interest/use, place those below.
    }
}
