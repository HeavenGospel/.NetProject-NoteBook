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
builder.Services.AddDbContext<PrismProjectContenxt>(option =>  // ������ݿ�����
{
    var constr = builder.Configuration.GetConnectionString("PrismProjectConnection");
    option.UseSqlite(constr);
}).AddUnitOfWork<PrismProjectContenxt>()
.AddCustomRepository<ToDo, ToDoRepository>()
.AddCustomRepository<Memo, MemoRepository>()
.AddCustomRepository<User, UserRepository>();  // �������ע��ִ�

builder.Services.AddTransient<IToDoService, ToDoService>();  // ���ô����������ӿ�
builder.Services.AddTransient<IMemoService, MemoService>();  // ���ñ���¼����ӿ�
builder.Services.AddTransient<ILoginService, LoginService>();  // ���õ�¼����ӿ�
//builder.Services.AddTransient<IUploadService, UploadService>();  // �����ϴ�����ӿ�

var autoMapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProFile());
});
builder.Services.AddSingleton(autoMapperConfig.CreateMapper());  // ����AutoMapper

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
// �����Զ��徲̬�ļ�Ŀ¼�������ϴ���ͷ��
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars")),
    RequestPath = "/uploads/avatars"  // ָ��ͨ�� /avatars �����ʸ��ļ����е�����
});
// ===================================================================================================

app.MapControllers();

app.Run();
