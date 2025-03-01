using HotelBookingSystem.Models; // 引入您的数据模型命名空间
using HotelBookingSystem.Repositories;
using Microsoft.EntityFrameworkCore; // 引入 EF Core

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器中

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 禁用 ReferenceHandler.Preserve
        options.JsonSerializerOptions.ReferenceHandler = null;
    }

    );

// 配置数据库上下文
builder.Services.AddDbContext<HotelBookingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 配置 CORS 策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()        // 允许任何来源
              .AllowAnyMethod()        // 允许任何 HTTP 方法（如 GET、POST）
              .AllowAnyHeader();       // 允许任何头部
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 在添加数据库上下文之后，添加以下代码
builder.Services.AddScoped<IRepository<Hotel>, HotelRepository>();
builder.Services.AddScoped<IRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IRepository<Booking>, BookingRepository>();
builder.Services.AddHttpClient();
// 添加 JSON Patch 支持
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // 启用 CORS 策略

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HotelView}/{action=Index}/{id?}"
);

// 应用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelBookingContext>();
    db.Database.Migrate();
}

app.Run();
