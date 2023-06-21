using JellyFish.Handler.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;

namespace JellyFish.Data.WebApi
{
    public class WebApiResponseModel
    {
        public class RootObject
        {
            public Meta meta { get; set; }
            public Jsonapi jsonapi { get; set; }
        }
        public class RootObject<T> : RootObject
            where T : class
        {
            public new Data<T>[] data { get; set; }
        }

        public class Meta
        {
            public int count { get; set; }
        }

        public class Jsonapi
        {
            public string version { get; set; }
            public string company { get; set; }
            public string author { get; set; }
            public string copyright { get; set; }
            public string use { get; set; }
            public string rfc { get; set; }
        }

        public class Data<T>
        {
            public string id { get; set; }
            public string type { get; set; }
            public T attributes { get; set; }
            public Links links { get; set; }
            public int depth { get; set; }
            public object relationships { get; set; }
            public Included<T>[] included { get; set; }
            public Meta meta { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string type { get; set; }
            public int depth { get; set; }
            public Meta meta { get; set; }
        }

        public class Included<T>
        {
            public string id { get; set; }
            public string type { get; set; }
            public T attributes { get; set; }
            public Links links { get; set; }
            public int depth { get; set; }
            public Meta meta { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

    }

}
