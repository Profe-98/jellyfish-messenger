using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JellyFish.Attribute;
using JellyFish.Data.AppConfig.Abstraction;

namespace JellyFish.Data.AppConfig.ConcreteImplements
{
    public class NetworkConfig : AbstractApplicationConfig
    {
        public enum HTTP_TRANSPORT_PROTOCOLS : int
        {
            HTTPS,
            HTTP,

        }

        [PropertyUiDisplayText("WebApiBaseUrl",true)]
        public string WebApiBaseUrl { get; set; }
        [PropertyUiDisplayText("WebApiBaseUrlPort", true)]
        public uint WebApiBaseUrlPort { get; set; }
        [PropertyUiDisplayText("WebApiHttpClientTransportProtocol", true)]
        public HTTP_TRANSPORT_PROTOCOLS WebApiHttpClientTransportProtocol { get; set; }

        [PropertyUiDisplayText("SignalRHubBaseUrl", true)]
        public string SignalRHubBaseUrl { get;set; }
        [PropertyUiDisplayText("SignalRHubBaseUrlPort", true)]
        public uint SignalRHubBaseUrlPort { get; set; }

        [PropertyUiDisplayText("SignalRTransferFormat", true)]
        public Microsoft.AspNetCore.Connections.TransferFormat SignalRTransferFormat = Microsoft.AspNetCore.Connections.TransferFormat.Text;

        [PropertyUiDisplayText("SignalRTransportProtocol", true)]
        public Microsoft.AspNetCore.Http.Connections.HttpTransportType SignalRTransportProtocol = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;

        public NetworkConfig() {

            SetDefaults();
        }

        public override void SetDefaults()
        {
            WebApiHttpClientTransportProtocol = HTTP_TRANSPORT_PROTOCOLS.HTTP;
            base.SetDefaults();
        }
    }
}
