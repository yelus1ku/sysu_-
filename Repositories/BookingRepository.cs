using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelBookingSystem.Repositories
{
    public class BookingRepository : IRepository<Booking>
    {
        private readonly HotelBookingContext _context;

        public BookingRepository(HotelBookingContext context)
        {
            _context = context;
        }

        // 添加预订
        public async Task<Booking> AddAsync(Booking entity)
        {
            // 检查房间是否已在目标日期被预订
            var isConflicting = await _context.Bookings.AnyAsync(b =>
                b.RoomId == entity.RoomId &&
                (entity.CheckInDate < b.CheckOutDate && entity.CheckOutDate > b.CheckInDate));

            if (isConflicting)
                throw new InvalidOperationException("The room is already booked during the selected dates.");

            // 更新房间状态为 "Booked"
            var room = await _context.Rooms.FindAsync(entity.RoomId);
            if (room == null)
                throw new KeyNotFoundException("Room not found.");

            room.Status = "Booked";
            _context.Rooms.Update(room);

            _context.Bookings.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除预订
        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            // 更新房间状态为 "Available"
            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room != null)
            {
                room.Status = "Available";
                _context.Rooms.Update(room);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        // 获取所有预订
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel) // 加载相关的 Room 和 Hotel
                .ToListAsync();
        }

        // 根据 ID 获取单个预订
        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel) // 加载相关的 Room 和 Hotel
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // 根据日期范围获取预订
        public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.CheckInDate < endDate && b.CheckOutDate > startDate)
                .ToListAsync();
        }

        // 更新预订
        public async Task<Booking> UpdateAsync(Booking entity)
        {
            // 检查房间是否已在目标日期被预订
            var isConflicting = await _context.Bookings.AnyAsync(b =>
                b.Id != entity.Id && // 排除当前预订
                b.RoomId == entity.RoomId &&
                (entity.CheckInDate < b.CheckOutDate && entity.CheckOutDate > b.CheckInDate));

            if (isConflicting)
                throw new InvalidOperationException("The room is already booked during the selected dates.");

            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 检查是否存在满足条件的预订
        public async Task<bool> ExistsAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _context.Bookings.AnyAsync(predicate);
        }
    }
}
