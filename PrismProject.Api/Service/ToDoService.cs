using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using PrismProject.Api.Context;
using PrismProject.Api.Context.Dtos;
using PrismProject.Shared.Parameters;

namespace PrismProject.Api.Service
{
    public class ToDoService : IToDoService  // 待办事项服务接口
    {
        private readonly IUnitOfWork work;
        private readonly IMapper mapper;

        public ToDoService(IUnitOfWork work, IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> AddAsync(ToDoDto model)
        {
            try
            {
                if (model?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var dbToDo = mapper.Map<ToDo>(model);
                var repo = work.GetRepository<ToDo>();

                dbToDo.CreatedDate = DateTime.Now;
                dbToDo.ModifiedDate = DateTime.Now;
                await repo.InsertAsync(dbToDo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, dbToDo);
                return new ApiResponse(false, "Failed to add ToDo");
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
                var repo = work.GetRepository<ToDo>();
                var todo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(parameter.Id) && x.UserId.Equals(parameter.UserId));
                repo.Delete(todo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, "");
                return new ApiResponse(false, "Failed to delete ToDo");
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

                var repo = work.GetRepository<ToDo>();
                var todos = await repo.GetPagedListAsync(
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

                return new ApiResponse(true, todos);
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
                var repo = work.GetRepository<ToDo>();
                var todo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(parameter.Id) && x.UserId.Equals(parameter.UserId));
                return new ApiResponse(true, todo);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(ToDoDto model)
        {
            try
            {
                if(model?.UserId == 0)
                    return new ApiResponse(false, "Invalid UserId");
                var dbToDo = mapper.Map<ToDo>(model);

                var repo = work.GetRepository<ToDo>();
                var todo = await repo.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(model.Id) && x.UserId.Equals(model.UserId));
                todo.Title = dbToDo.Title;
                todo.Content = dbToDo.Content;
                todo.Status = dbToDo.Status;
                todo.ModifiedDate = DateTime.Now;
                repo.Update(todo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, todo);
                return new ApiResponse(false, "Failed to update ToDo");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }
    }
}
