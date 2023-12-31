﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Mobile.Jellyfish.Handler.Backend.Communication.WebApi.Abstraction
{
    public interface IRestClient
    {
        public List<KeyValuePair<string, string>> PermanentHeaders { get; }  
        public RestRequest CreateRequest(string urlOrEndpoint, RestSharp.Method httpMethod, ContentType contentType, string body = null, List<KeyValuePair<string, string>> query = null, List<KeyValuePair<string, string>> headerValues = null);
        public Task<RestResponse> Send(RestRequest restRequest,CancellationToken cancellationToken);
        public void AddPermanentHeaderValue(string header, string value);
        public void DeletePermantentHeaderValue(string header);
    }
}
