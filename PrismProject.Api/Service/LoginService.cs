using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using PrismProject.Api.Context;
using PrismProject.Api.Context.Dtos;
using PrismProject.Api.Extensions;

namespace PrismProject.Api.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork work;
        private readonly IMapper mapper;

        public LoginService(IUnitOfWork work, IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> LoginAsync(string Account, string Password)
        {
            try
            {
                Password = Password.GetMD5();
                var repo = work.GetRepository<User>();

                var model = await repo.GetFirstOrDefaultAsync(
                    predicate: x => (x.Account.Equals(Account)) && (x.Password.Equals(Password)));

                if(model == null)
                    return new ApiResponse(false, "登陆失败， 账号或密码错误。");
                else
                    return new ApiResponse(true, model);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> RegisterAsync(UserDto user)
        {
            try
            {
                var model = mapper.Map<User>(user);
                model.CreatedDate = DateTime.Now;
                model.Password = model.Password.GetMD5();
                
                var repo = work.GetRepository<User>();

                var usreModel = await repo.GetFirstOrDefaultAsync(
                    predicate: x => (x.Account.Equals(model.Account)));
                
                if(usreModel!= null)
                    return new ApiResponse(false, $"账号:{model.Account}已存在.");

                await repo.InsertAsync(model);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, model);
                return new ApiResponse(false, "注册失败，");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }
    }
}
