using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace TuyaDevelopment
{
    public class DeviceInfoProp
    {
        
        public const string device_id = "your device id";
        public static string client_id = "your client id";
        public static string secret = "your secret key";


        public static System.Windows.Forms.NotifyIcon m_notifyIcon;
        public static System.Windows.Forms.ContextMenu contextMenu1;
        public static System.Windows.Forms.MenuItem menuItem1, menuItem2, menuItem3;

        public class Result
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("value")]
            public object Value { get; set; }
        }

        public class Root
        {
            [JsonProperty("result")]
            public List<Result> Result { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("t")]
            public long T { get; set; }
        }

        public static string Login()
        {
            string t = MillisecondsTimestamp(DateTime.Now);
            string hash = Hash(DeviceInfoProp.client_id + t, DeviceInfoProp.secret);
            var client = new RestClient("https://openapi.tuyaeu.com/v1.0/token?grant_type=1");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("client_id", DeviceInfoProp.client_id);
            request.AddHeader("sign", hash);
            request.AddHeader("t", t);
            request.AddHeader("sign_method", "HMAC-SHA256");
            IRestResponse response = client.Execute(request);


            var jObject = JObject.Parse(response.Content);
            var sampledataJson = JObject.Parse(jObject["result"].ToString());
            var token = sampledataJson["access_token"].ToString();
            
            return token;
        }

        public static string Hash(string data, string secret)
        {
            var dataAsBytes = Encoding.UTF8.GetBytes(data);
            var secretDataAsByte = Encoding.UTF8.GetBytes(secret);
            using (var hasher = new HMACSHA256(secretDataAsByte))
            {
                return ToHexString(hasher.ComputeHash(dataAsBytes)).ToUpper();
            }
        }

        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static string MillisecondsTimestamp(DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long l = (long)(date.ToUniversalTime() - baseDate).TotalMilliseconds;
            return l.ToString();
        }

        public static bool CheckLastState(string deviceID)
        {
            string token = Login();
            string t = MillisecondsTimestamp(DateTime.Now);
            string hash = Hash(client_id + token + t, secret);
            var client = new RestClient("https://openapi.tuyaeu.com/v1.0/devices/"+ deviceID + "/status");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("client_id", client_id);
            request.AddHeader("access_token", token);
            request.AddHeader("sign", hash);
            request.AddHeader("t", t);
            request.AddHeader("sign_method", "HMAC-SHA256");
            IRestResponse response = client.Execute(request);


            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response.Content);
            bool lastState = (bool)myDeserializedClass.Result[0].Value;

            switch (deviceID)
            {
                case device_id:
                    if (lastState)
                        m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_on.ico");
                    else
                    {
                        m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_off.ico");
                    }
                    break;

            }

            return lastState;
        }
    }


}
