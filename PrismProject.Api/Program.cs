using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PrismProject.Api.Context;
using PrismProject.Api.Context.Repository;
using PrismProject.Api.Extensions;
using PrismProject.Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================================================================================================
builder.Services.AddDbContext<PrismProjectContenxt>(option =>  // 添加数据库连接
{
    var constr = builder.Configuration.GetConnectionString("PrismProjectConnection");
    option.UseSqlite(constr);
}).AddUnitOfWork<PrismProjectContenxt>()
.AddCustomRepository<ToDo, ToDoRepository>()
.AddCustomRepository<Memo, MemoRepository>()
.AddCustomRepository<User, UserRepository>();  // 添加依赖注入仓储

builder.Services.AddTransient<IToDoService, ToDoService>();  // 配置待办事项服务接口
builder.Services.AddTransient<IMemoService, MemoService>();  // 配置备忘录服务接口
builder.Services.AddTransient<ILoginService, LoginService>();  // 配置登录服务接口
//builder.Services.AddTransient<IUploadService, UploadService>();  // 配置上传服务接口

var autoMapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProFile());
});
builder.Services.AddSingleton(autoMapperConfig.CreateMapper());  // 配置AutoMapper

// ===================================================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===================================================================================================
app.UseAuthorization();
// 配置自定义静态文件目录，用于上传的头像
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars")),
    RequestPath = "/uploads/avatars"  // 指定通过 /avatars 来访问该文件夹中的内容
});
// ===================================================================================================

app.MapControllers();

app.Run();
