using System;

namespace Allied.RealTime.Example.Web.Models
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            success = true;
        }

        public BaseResponse(Exception ex)
        {
            success = false;
            errorMessage = ex.Message;
        }

        // TODO: place common response properties here
        public bool success { get; set; }

        public string errorMessage { get; set; }
    }
}
