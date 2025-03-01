using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelBookingSystem.Repositories
{
    public class HotelRepository : IRepository<Hotel>
    {
        private readonly HotelBookingContext _context;

        public HotelRepository(HotelBookingContext context)
        {
            _context = context;
        }

        // 添加酒店
        public async Task<Hotel> AddAsync(Hotel entity)
        {
            _context.Hotels.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除酒店
        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        // 获取所有酒店
        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _context.Hotels
                .Include(h => h.Rooms) // 加载房间数据
                .ToListAsync();
        }

        // 根据 ID 获取单个酒店
        public async Task<Hotel> GetByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Rooms) // 加载房间数据
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        // 获取酒店的空房间数量
        public async Task<int> GetAvailableRoomCountAsync(int hotelId)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Hotel not found.");

            return hotel.Rooms.Count(r => r.Status.ToLower() == "available");
        }

        // 更新酒店信息
        public async Task<Hotel> UpdateAsync(Hotel entity)
        {
            var existingHotel = await _context.Hotels.FindAsync(entity.Id);
            if (existingHotel == null)
                throw new KeyNotFoundException("Hotel not found.");

            // 更新酒店信息，但不影响房间信息
            existingHotel.Name = entity.Name;
            existingHotel.Address = entity.Address;
            existingHotel.City = entity.City;
            existingHotel.State = entity.State;
            existingHotel.ZipCode = entity.ZipCode;
            existingHotel.PhoneNumber = entity.PhoneNumber;
            existingHotel.Email = entity.Email;
            existingHotel.Website = entity.Website;

            _context.Hotels.Update(existingHotel);
            await _context.SaveChangesAsync();
            return existingHotel;
        }

        // 检查酒店是否存在
        public async Task<bool> ExistsAsync(Expression<Func<Hotel, bool>> predicate)
        {
            return await _context.Hotels.AnyAsync(predicate);
        }
    }
}
