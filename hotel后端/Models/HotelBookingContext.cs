using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models
{
    public class HotelBookingContext : DbContext
    {
        public HotelBookingContext(DbContextOptions<HotelBookingContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            // 配置 Hotel 和 Room 的关系
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade); // 删除 Hotel 时删除相关 Rooms

            // 配置 Room 和 Booking 的关系
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade); // 删除 Room 时删除相关 Bookings

            // 配置 Room 的属性
            modelBuilder.Entity<Room>()
                .Property(r => r.RoomPrice)
                .HasPrecision(10, 2) // 配置价格的精度
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(r => r.RoomType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Room>()
                .Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id); // 设置主键

            modelBuilder.Entity<Booking>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd(); // 确保 ID 是数据库自动生成

            // 配置 Booking 的属性
            modelBuilder.Entity<Booking>()
                .Property(b => b.CheckInDate)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.CheckOutDate)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CustomerPhone)
                .IsRequired()
                .HasMaxLength(20);

            // 确保 RoomNumber 是唯一的
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();
        }
    }
}
