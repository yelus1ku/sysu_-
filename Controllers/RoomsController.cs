using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Hotel> _hotelRepository;

        public RoomsController(IRepository<Room> roomRepository, IRepository<Hotel> hotelRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        // 获取所有房间信息
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var rooms = await _roomRepository.GetAllAsync();

            var roomDTOs = rooms.Select(room => new RoomDTO
            {
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                RoomPrice = room.RoomPrice,
                Status = room.Status,
                HotelName = room.Hotel?.Name,
                OccupantName = room.Status == "occupied" ? room.OccupantName : null,
                OccupantPhone = room.Status == "occupied" ? room.OccupantPhone : null,
                CheckInDate = room.Status == "occupied" ? room.CheckInDate : null,
                ExpectedCheckOutDate = room.Status == "occupied" ? room.ExpectedCheckOutDate : null
            }).ToList();

            return Ok(roomDTOs);
        }

        // 根据房间号获取房间信息
        [HttpGet("{roomNumber}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(string roomNumber)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var room = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var roomDTO = new RoomDTO
            {
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                RoomPrice = room.RoomPrice,
                Status = room.Status,
                HotelName = room.Hotel?.Name,
                OccupantName = room.Status == "occupied" ? room.OccupantName : null,
                OccupantPhone = room.Status == "occupied" ? room.OccupantPhone : null,
                CheckInDate = room.Status == "occupied" ? room.CheckInDate : null,
                ExpectedCheckOutDate = room.Status == "occupied" ? room.ExpectedCheckOutDate : null
            };

            return Ok(roomDTO);
        }

        // 修改房间信息，包括入住人信息
        [HttpPut("{roomNumber}")]
        public async Task<IActionResult> UpdateRoom(string roomNumber, [FromBody] Dictionary<string, object> updates)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var existingRoom = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (existingRoom == null)
            {
                return NotFound("Room not found.");
            }

            // 动态更新传递的字段
            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "occupantname":
                        existingRoom.OccupantName = update.Value?.ToString();
                        break;
                    case "occupantphone":
                        existingRoom.OccupantPhone = update.Value?.ToString();
                        break;
                    case "checkindate":
                        //existingRoom.CheckInDate = update.Value != null ? DateTime.Parse(update.Value.ToString()) : null;
                        existingRoom.CheckInDate = update.Value != null ? DateOnly.Parse(update.Value.ToString()) : (DateOnly?)null;
                        break;
                    case "expectedcheckoutdate":
                        //existingRoom.ExpectedCheckOutDate = update.Value != null ? DateTime.Parse(update.Value.ToString()) : null;
                        existingRoom.ExpectedCheckOutDate = update.Value != null ? DateOnly.Parse(update.Value.ToString()) : (DateOnly?)null;
                        break;
                    case "roomtype":
                        existingRoom.RoomType = update.Value?.ToString();
                        break;
                    case "roomprice":
                        existingRoom.RoomPrice = update.Value != null ? Convert.ToDecimal(update.Value) : existingRoom.RoomPrice;
                        break;
                    case "status":
                        existingRoom.Status = update.Value?.ToString();
                        break;
                    default:
                        return BadRequest($"Unknown field: {update.Key}");
                }
            }

            var updatedRoom = await _roomRepository.UpdateAsync(existingRoom);

            return Ok(new
            {
                RoomNumber = updatedRoom.RoomNumber,
                RoomType = updatedRoom.RoomType,
                RoomPrice = updatedRoom.RoomPrice,
                Status = updatedRoom.Status,
                OccupantName = updatedRoom.OccupantName,
                OccupantPhone = updatedRoom.OccupantPhone,
                CheckInDate = updatedRoom.CheckInDate,
                ExpectedCheckOutDate = updatedRoom.ExpectedCheckOutDate
            });
        }


        // 删除房间
        [HttpDelete("{roomNumber}")]
        public async Task<IActionResult> DeleteRoom(string roomNumber)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var room = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var result = await _roomRepository.DeleteAsync(room.Id);

            if (!result)
            {
                return StatusCode(500, "Failed to delete the room.");
            }

            return NoContent();
        }

        // 获取所有空房间
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms()
        {
            var rooms = await _roomRepository.GetAllAsync();

            var availableRooms = rooms.Where(r => r.Status == "Available").Select(room => new RoomDTO
            {
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                RoomPrice = room.RoomPrice,
                Status = room.Status,
                HotelName = room.Hotel?.Name
            }).ToList();

            return Ok(availableRooms);
        }
    }
}

