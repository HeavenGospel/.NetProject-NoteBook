
namespace PrismProject.Service
{
    public class BaseResponse  // 响应基类
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }
    }

    public class BaseResponse<T>  // 响应基类(泛型)
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public T Result { get; set; }
    }
}