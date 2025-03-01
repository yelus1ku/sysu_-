using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelBookingSystem.Repositories
{
    public class RoomRepository : IRepository<Room>
    {
        private readonly HotelBookingContext _context;

        public RoomRepository(HotelBookingContext context)
        {
            _context = context;
        }

        // 添加房间
        public async Task<Room> AddAsync(Room entity)
        {
            _context.Rooms.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除房间
        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        // 获取所有房间
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Hotel) // 加载关联的 Hotel 数据
                .Include(r => r.Bookings) // 加载关联的 Bookings 数据
                .ToListAsync();
        }

        // 按 ID 获取单个房间
        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)  // 如果需要加载关联的 Hotel
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // 更新房间
        public async Task<Room> UpdateAsync(Room entity)
        {
            var room = await _context.Rooms.FindAsync(entity.Id);
            if (room == null)
                throw new KeyNotFoundException("Room not found.");

            // 更新房间的属性
            room.Status = entity.Status; // 更新房间状态
            room.RoomType = entity.RoomType; // 更新房间类型
            room.RoomPrice = entity.RoomPrice; // 更新房间价格
            room.OccupantName = entity.OccupantName; // 更新入住人姓名
            room.OccupantPhone = entity.OccupantPhone; // 更新入住人手机号
            //room.OccupantIdCard = entity.OccupantIdCard; // 更新入住人身份证号
            room.CheckInDate = entity.CheckInDate; // 更新入住时间
            room.ExpectedCheckOutDate = entity.ExpectedCheckOutDate; // 更新预计离开时间

            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        // 检查是否存在满足条件的房间
        public async Task<bool> ExistsAsync(Expression<Func<Room, bool>> predicate)
        {
            return await _context.Rooms.AnyAsync(predicate);
        }
    }
}
