using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Data.WebApi
{
    public class WebApiHttpRequestResponseModel<T>
        where T:class
    {

        public WebApiResponseModel.RootObject<T> ApiResponseDeserialized { get; set; }
        public RestResponse DefaultResponse { get; set; } = new RestResponse() { StatusCode = System.Net.HttpStatusCode.NotFound };
        public bool IsSuccess
        {
            get
            {
                return DefaultResponse!=null&&DefaultResponse.IsSuccessStatusCode;
            }
        }
    }
}
