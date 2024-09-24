using RestSharp;

namespace PrismProject.Service
{
    public class BaseRequest  // 请求基类
    {
        public Method Method { get; set; }
        public string Route { get; set; }
        public string ContentType { get; set; } = "application/json";
        public object Parameter { get; set; }
    }
}
