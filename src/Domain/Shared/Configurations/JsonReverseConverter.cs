using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Shared.Configurations
{
    public static class JsonReverseConverter
    {
        public static string JsonData<T>(IEnumerable<T> obj)
        {
            JsonSerializerOptions json = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // to make property in camel case
            };
            json.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;  // To prevent encoding of bangla font/text
            var jsonData = JsonSerializer.Serialize(obj, json);
            return jsonData;
        }
        public static string JsonData<T>(T obj)
        {
            JsonSerializerOptions json = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // to make property in camel case
            };
            json.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;  // To prevent encoding of bangla font/text
            var jsonData = JsonSerializer.Serialize(obj, json);
            return jsonData;
        }
        public static T JsonToObject<T>(string json)
        {
            try
            {
                if (json != null)
                {
                    var options = new JsonSerializerOptions()
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString
                    };
                    var data = JsonSerializer.Deserialize<T>(json, options);
                    return data;
                }
            }
            catch (Exception)
            {
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static T JsonToObject2<T>(string json)
        {
            try
            {
                if (json != null)
                {
                    //var options = new JsonSerializerOptions() {
                    //    NumberHandling = JsonNumberHandling.AllowReadingFromString |
                    //JsonNumberHandling.WriteAsString
                    //};
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    return data;
                }
            }
            catch (Exception)
            {
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
        public static IEnumerable<T> JsonToObject<T>(string json, string text)
        {
            if (json != null)
            {
                var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                var data = JsonSerializer.Deserialize<IEnumerable<T>>(json, options);
                return data;
            }
            return new List<T>();
        }

        public static string DataTableToJson(this DataTable dataTable)
        {
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
            return JSONString;
        }
    }
}
