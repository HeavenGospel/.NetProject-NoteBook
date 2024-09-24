using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using PrismProject.Api.Context;
using PrismProject.Api.Context.Dtos;
using PrismProject.Shared.Parameters;

namespace PrismProject.Api.Service
{
    public class MemoService : IMemoService  // 备忘录服务接口
    {
        private readonly IUnitOfWork work;
        private readonly IMapper mapper;

        public MemoService(IUnitOfWork work, IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> AddAsync(MemoDto model)
        {
            try
            {
                if (model?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var dbMemo = mapper.Map<Memo>(model);

                var repo = work.GetRepository<Memo>();

                dbMemo.CreatedDate = DateTime.Now;
                dbMemo.ModifiedDate = DateTime.Now;
                await repo.InsertAsync(dbMemo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, dbMemo);
                return new ApiResponse(false, "Failed to add Memo");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(GetAndDelParameter parameter)
        {
            try
            {
                if (parameter?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var repo = work.GetRepository<Memo>();
                var memo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(parameter.Id) && x.UserId.Equals(parameter.UserId));
                repo.Delete(memo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, "");
                return new ApiResponse(false, "Failed to delete Memo");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter parameter)
        {
            try
            {
                if (parameter?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var repo = work.GetRepository<Memo>();
                var memos = await repo.GetPagedListAsync(
                    predicate: x =>
                    x.UserId.Equals(parameter.UserId) && (
                    string.IsNullOrWhiteSpace(parameter.Search) ||
                    x.Title.Contains(parameter.Search) ||
                    x.Content.Contains(parameter.Search)),
                    pageIndex: parameter.PageIndex,
                    pageSize: parameter.PageSize,
                    orderBy: source =>
                    {
                        // 修正了这里的逻辑
                        if (parameter.SortOrder == 0)
                            return source.OrderByDescending(t => t.CreatedDate);
                        else
                            return source.OrderByDescending(t => t.ModifiedDate);
                    }
                );
                return new ApiResponse(true, memos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> GetSingleAsync(GetAndDelParameter parameter)
        {
            try
            {
                if (parameter?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var repo = work.GetRepository<Memo>();
                var memo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(parameter.Id) && x.UserId.Equals(parameter.UserId));
                return new ApiResponse(true, memo);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(MemoDto model)
        {
            try
            {
                if (model?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var dbMemo = mapper.Map<Memo>(model);

                var repo = work.GetRepository<Memo>();
                var memo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(model.Id) && x.UserId.Equals(model.UserId));
                memo.Title = dbMemo.Title;
                memo.Content = dbMemo.Content;
                memo.ModifiedDate = DateTime.Now;
                repo.Update(memo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, dbMemo);
                return new ApiResponse(false, "Failed to update Memo");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }
    }
}
