using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using ApplicationSharedKernel.Converter;
using ApplicationSharedKernel.Data.Web.MIME;
using ApplicationSharedKernel.Application.Model.Internal;


using ApplicationSharedKernel.Cache.Distributed.RedisCache;
using ApplicationSharedKernel.Ampq.Rabbitmq.Data;
using ApplicationSharedKernel.Ampq.Rabbitmq;
using ApplicationSharedKernel.Antivirus;
using ApplicationSharedKernel.Antivirus.nClam;
using ApplicationSharedKernel.Application.Model.DataTransferObject.Frontend.Transfer;
using ApplicationSharedKernel.Application.Model.DataTransferObject;
using ApplicationSharedKernel.Application.Model;
using ApplicationSharedKernel.Configuration;
using ApplicationSharedKernel.Collections;
using ApplicationSharedKernel.Web.AspNet.Controller;
using ApplicationSharedKernel.Data;
using ApplicationSharedKernel.Data.Web;
using ApplicationSharedKernel.Data.Format.Json;
using ApplicationSharedKernel.Data.Web.Api.Abstractions.JsonApiV1;
using ApplicationSharedKernel.Database;
using ApplicationSharedKernel.Application.Model.Database.MySQL;
using ApplicationSharedKernel.Application.Model.Database.MySQL.Data;
using ApplicationSharedKernel.Web.AspNet.Filter;
using ApplicationSharedKernel.Formatter;
using ApplicationSharedKernel.LocalSystem.IO.File;
using ApplicationSharedKernel.Log;
using ApplicationSharedKernel.Metric;
using ApplicationSharedKernel.Metric.Influxdb;
using ApplicationSharedKernel.MicroService;
using ApplicationSharedKernel.Network;
using ApplicationSharedKernel.Security;
using ApplicationSharedKernel.Security.Encryption;
using ApplicationSharedKernel.Threading;
using ApplicationSharedKernel.Threading.Service;
using ApplicationSharedKernel.Threading.Task;
using ApplicationSharedKernel.Utility;
using ApplicationSharedKernel.Web;
using ApplicationSharedKernel.Web.AspNet;
using ApplicationSharedKernel.Web.Authentification;
using ApplicationSharedKernel.Web.Http.Api.Abstractions.JsonApiV1;
using ApplicationSharedKernel.Web.Http;

namespace ApplicationSharedKernel.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class AuthModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv4_remote", MySqlDbType.String)]
        public string Ipv4 { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv6_remote", MySqlDbType.String)]
        public string Ipv6 { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("remote_port", MySqlDbType.Int32)]
        public int RemotePort { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("local_port", MySqlDbType.Int32)]
        public int LocalPort { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv4_local", MySqlDbType.String)]
        public string Ipv4Local { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv6_local", MySqlDbType.String)]
        public string Ipv6Local { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("token")]
        [DatabaseColumnPropertyAttribute("token", MySqlDbType.String)]
        public string Token { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("token_expires")]
        [DatabaseColumnPropertyAttribute("token_expires_in", MySqlDbType.DateTime)]
        public DateTime TokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("refresh_token")]
        [DatabaseColumnPropertyAttribute("refresh_token", MySqlDbType.String)]
        public string RefreshToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("refresh_token_expires")]
        [DatabaseColumnPropertyAttribute("refresh_token_expires_in", MySqlDbType.DateTime)]
        public DateTime RefreshTokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(1024, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("user_agent", MySqlDbType.String)]
        public string UserAgent { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("logout_datetime")]
        [DatabaseColumnPropertyAttribute("logout_datetime", MySqlDbType.DateTime)]
        public DateTime LogoutTime { get; set; }

        [JsonConverter(typeof(ApplicationSharedKernel.Converter.JsonConverter.JsonBoolConverter))]
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonIgnore]
        public IPAddress IPv4RemoteObject
        {
            get
            {
                return ConvertStringToIp(Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv4LocalObject
        {
            get
            {
                return ConvertStringToIp(Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6RemoteObject
        {
            get
            {
                return ConvertStringToIp(Ipv6);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6LocalObject
        {
            get
            {
                return ConvertStringToIp(Ipv6Local);
            }
        }

        [JsonIgnore]
        public bool IsTokenExpired
        {
            get
            {
                return DateTime.Now >= TokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsRefreshTokenExpired
        {
            get
            {
                return DateTime.Now >= RefreshTokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsLoggedIn
        {
            get
            {
                return Token == null ? false : true;
            }
        }
        [JsonIgnore]
        public UserModel UserModel { get; set; }

        #region Ctor & Dtor
        public AuthModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        private IPAddress ConvertStringToIp(string ipStr)
        {
            if (IPAddress.TryParse(Ipv4Local, out IPAddress address))
            {
                return address;
            }
            return null;
        }
        #endregion Methods
    }
}
