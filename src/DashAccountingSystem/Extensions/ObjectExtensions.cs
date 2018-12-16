using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DashAccountingSystem.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeProps(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
