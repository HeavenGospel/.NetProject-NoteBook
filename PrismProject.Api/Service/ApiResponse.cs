namespace PrismProject.Api.Service
{
    public partial class ApiResponse
    {
        public ApiResponse(bool status, object result)  // 成功
        {
            this.Status = status;
            this.Result = result;
        }
        public ApiResponse(bool status, string message)  // 失败
        {
            this.Status = status;
            this.Message = message;
        }

        public string Message { get; set; }
        public bool Status { get; set; }
        public object Result { get; set; }

    }
}
