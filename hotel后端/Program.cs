using HotelBookingSystem.Models; // ������������ģ�������ռ�
using HotelBookingSystem.Repositories;
using Microsoft.EntityFrameworkCore; // ���� EF Core

var builder = WebApplication.CreateBuilder(args);

// ��ӷ���������

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ���� ReferenceHandler.Preserve
        options.JsonSerializerOptions.ReferenceHandler = null;
    }

    );

// �������ݿ�������
builder.Services.AddDbContext<HotelBookingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ���� CORS ����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()        // �����κ���Դ
              .AllowAnyMethod()        // �����κ� HTTP �������� GET��POST��
              .AllowAnyHeader();       // �����κ�ͷ��
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������ݿ�������֮��������´���
builder.Services.AddScoped<IRepository<Hotel>, HotelRepository>();
builder.Services.AddScoped<IRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IRepository<Booking>, BookingRepository>();
builder.Services.AddHttpClient();
// ��� JSON Patch ֧��
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// ���� HTTP ����ܵ�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // ���� CORS ����

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HotelView}/{action=Index}/{id?}"
);

// Ӧ�����ݿ�Ǩ��
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelBookingContext>();
    db.Database.Migrate();
}

app.Run();
