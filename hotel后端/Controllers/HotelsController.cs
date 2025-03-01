using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IRepository<Hotel> _hotelRepository;

        public HotelsController(IRepository<Hotel> hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            var hotels = await _hotelRepository.GetAllAsync();

            var hotelDTOs = hotels.Select(hotel => new HotelDTO
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                State = hotel.State,
                ZipCode = hotel.ZipCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Website = hotel.Website,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt,
                TotalRooms = hotel.Rooms.Count,
                AvailableRooms = hotel.Rooms.Count(r => r.Status == "Available"), // 空房间数
                RoomNumbers = hotel.Rooms.Select(r => r.RoomNumber).ToList()
            }).ToList();

            return Ok(hotelDTOs);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDTO>> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            var hotelDTO = new HotelDTO
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                State = hotel.State,
                ZipCode = hotel.ZipCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Website = hotel.Website,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt,
                TotalRooms = hotel.Rooms.Count,
                AvailableRooms = hotel.Rooms.Count(r => r.Status == "Available"),
                RoomNumbers = hotel.Rooms.Select(r => r.RoomNumber).ToList()
            };

            return Ok(hotelDTO);
        }

        // POST: api/Hotels
        [HttpPost]
        public async Task<ActionResult<Hotel>> CreateHotel([FromBody] HotelDTO hotelDTO)
        {
            var hotel = new Hotel
            {
                Name = hotelDTO.Name,
                Address = hotelDTO.Address,
                City = hotelDTO.City,
                State = hotelDTO.State,
                ZipCode = hotelDTO.ZipCode,
                PhoneNumber = hotelDTO.PhoneNumber,
                Email = hotelDTO.Email,
                Website = hotelDTO.Website,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdHotel = await _hotelRepository.AddAsync(hotel);
            return CreatedAtAction(nameof(GetHotel), new { id = createdHotel.Id }, createdHotel);
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDTO updateHotelDTO)
        {
            

            // 从数据库获取当前酒店信息
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
            {
                return NotFound("Hotel not found.");
            }

            // 更新酒店信息的字段
            hotel.Name = updateHotelDTO.Name;
            hotel.Address = updateHotelDTO.Address;
            hotel.City = updateHotelDTO.City;
            hotel.State = updateHotelDTO.State;
            hotel.ZipCode = updateHotelDTO.ZipCode;
            hotel.PhoneNumber = updateHotelDTO.PhoneNumber;
            hotel.Email = updateHotelDTO.Email;
            hotel.Website = updateHotelDTO.Website;

            // 更新修改时间
            hotel.UpdatedAt = DateTime.UtcNow;

            // 将更新后的酒店信息保存到数据库
            var updatedHotel = await _hotelRepository.UpdateAsync(hotel);

            // 返回更新后的酒店信息
            return Ok(updatedHotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
