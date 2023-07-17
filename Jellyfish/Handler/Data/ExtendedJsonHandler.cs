using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApiFunction.Data.Format.Json;

namespace JellyFish.Handler.Data
{
    public class ExtendedJsonHandler : JsonHandler
    {
        public static JsonSerializerOptions DefaultSerializerOption= new() { 
            
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented= true,
            Converters = {
                new JsonStringEnumConverter()//enum Values als String
            },
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode

        };
        public ExtendedJsonHandler() : this(true, DefaultSerializerOption)
        {

        }
        public ExtendedJsonHandler(bool tryToFillValues,JsonSerializerOptions jsonSerializerOptions) : base(tryToFillValues,jsonSerializerOptions)
        {

        }    
    }
}
