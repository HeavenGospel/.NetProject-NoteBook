using PrismProject.Common.Models;

namespace PrismProject.Service
{
    public class MemoService : BaseService<MemoDto>, IMemoService  // 备忘录服务
    {
        public MemoService(HttpRestClient client, string serviceName = "Memo") : base(client, serviceName)
        {

        }
    }
}
