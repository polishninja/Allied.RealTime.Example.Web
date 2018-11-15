using System;

namespace Allied.RealTime.Example.Web.Models
{
    public class TransactionResponse : BaseResponse
    {
        public TransactionResponse()
        {
        }

        public TransactionResponse(Exception ex) :
            base(ex)
        {
        }

        public string transactionKey { get; set; }

        public string description { get; set; }
    }
}
