using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Room> _roomRepository;

        public BookingsController(IRepository<Room> roomRepository, IRepository<Booking> bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
        {
            var bookings = await _bookingRepository.GetAllAsync();

            var bookingDTOs = bookings.Select(booking =>
            {
                var room = _roomRepository.GetByIdAsync(booking.RoomId).Result;
                var totalDays = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) - booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
                var totalPrice = room?.RoomPrice * totalDays ?? 0;

                return new BookingDTO
                {
                    Id = booking.Id,
                    RoomNumber = room?.RoomNumber,
                    CustomerName = booking.CustomerName,
                    CustomerEmail = booking.CustomerEmail,
                    CustomerPhone = booking.CustomerPhone,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalPrice = totalPrice
                };
            }).ToList();

            return Ok(bookingDTOs);
        }

        // GET: api/Bookings/room/{roomNumber}
        [HttpGet("room/{roomNumber}")]

        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingByRoomNumber(string roomNumber)
        {
            // 查找房间
            var room = (await _roomRepository.GetAllAsync())
                .FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            // 查找与该房间关联的所有预订记录
            var bookings = (await _bookingRepository.GetAllAsync())
                .Where(b => b.RoomId == room.Id)
                .ToList();

            if (!bookings.Any())
            {
                return NotFound("No bookings found for the specified room.");
            }

            // 创建 BookingDTO 列表
            var bookingDTOs = bookings.Select(booking =>
            {
                var totalDays = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) - booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
                var totalPrice = room.RoomPrice * totalDays;

                return new BookingDTO
                {
                    Id = booking.Id,
                    RoomNumber = room.RoomNumber,
                    CustomerName = booking.CustomerName,
                    CustomerEmail = booking.CustomerEmail,
                    CustomerPhone = booking.CustomerPhone,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalPrice = totalPrice
                };
            });

            return Ok(bookingDTOs);
        }

        // POST: api/Bookings
        [HttpPost]

        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] BookingDTO bookingDTO)
        {
            // 获取指定的房间
            var rooms = await _roomRepository.GetAllAsync();
            var selectedRoom = rooms.FirstOrDefault(r => r.RoomNumber == bookingDTO.RoomNumber);

            if (selectedRoom == null)
            {
                return NotFound("The specified Room does not exist.");
            }

            // 检查预订时间段是否冲突
            var existingBookings = (await _bookingRepository.GetAllAsync())
                .Where(b => b.RoomId == selectedRoom.Id)
                .ToList();

            bool hasConflict = existingBookings.Any(b =>
                bookingDTO.CheckInDate < b.CheckOutDate && bookingDTO.CheckOutDate > b.CheckInDate);

            if (hasConflict)
            {
                return BadRequest("The specified time period is already booked.");
            }

            // 创建新的预订记录
            var booking = new Booking
            {
                RoomId = selectedRoom.Id,
                CustomerName = bookingDTO.CustomerName,
                CustomerEmail = bookingDTO.CustomerEmail,
                CustomerPhone = bookingDTO.CustomerPhone,
                CheckInDate = bookingDTO.CheckInDate,
                CheckOutDate = bookingDTO.CheckOutDate,
            };

            var createdBooking = await _bookingRepository.AddAsync(booking);

            // 返回创建的预订信息
            var result = new BookingDTO
            {
                Id = createdBooking.Id,
                RoomNumber = selectedRoom.RoomNumber,
                CustomerName = createdBooking.CustomerName,
                CustomerEmail = createdBooking.CustomerEmail,
                CustomerPhone = createdBooking.CustomerPhone,
                CheckInDate = createdBooking.CheckInDate,
                CheckOutDate = createdBooking.CheckOutDate,
            };

            return CreatedAtAction(nameof(GetBookingByRoomNumber), new { roomNumber = result.RoomNumber }, result);
        }




        // PUT: api/Bookings/room/{roomNumber}
        // PUT: api/Bookings/room/{roomNumber}
        /*
        [HttpPut("room/{roomNumber}")]
        public async Task<IActionResult> UpdateBookingByRoomNumber(string roomNumber, [FromBody] BookingDTO bookingDTO)
        {
            // 根据房间号查找房间
            var room = (await _roomRepository.GetAllAsync())
                        .FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("The specified Room does not exist.");
            }

            // 根据房间 ID 查找关联的 Booking
            var booking = (await _bookingRepository.GetAllAsync())
                          .FirstOrDefault(b => b.RoomId == room.Id);

            if (booking == null)
            {
                return NotFound("No booking found for the specified Room.");
            }

            // 更新 Booking 实体（只更新提供的字段）
            if (!string.IsNullOrEmpty(bookingDTO.CustomerName))
            {
                booking.CustomerName = bookingDTO.CustomerName;
            }
            if (!string.IsNullOrEmpty(bookingDTO.CustomerEmail))
            {
                booking.CustomerEmail = bookingDTO.CustomerEmail;
            }
            if (!string.IsNullOrEmpty(bookingDTO.CustomerPhone))
            {
                booking.CustomerPhone = bookingDTO.CustomerPhone;
            }
            if (bookingDTO.CheckInDate != default(DateOnly))
            {
                booking.CheckInDate = bookingDTO.CheckInDate;
            }
            if (bookingDTO.CheckOutDate != default(DateOnly))
            {
                booking.CheckOutDate = bookingDTO.CheckOutDate;
            }

            // 更新 Booking
            var updatedBooking = await _bookingRepository.UpdateAsync(booking);

            // 同步更新 Room 信息
            if (!string.IsNullOrEmpty(bookingDTO.CustomerName))
            {
                room.OccupantName = bookingDTO.CustomerName;
            }
            if (!string.IsNullOrEmpty(bookingDTO.CustomerPhone))
            {
                room.OccupantPhone = bookingDTO.CustomerPhone;
            }
            if (bookingDTO.CheckInDate != default(DateOnly))
            {
                room.CheckInDate = bookingDTO.CheckInDate;
            }
            if (bookingDTO.CheckOutDate != default(DateOnly))
            {
                room.ExpectedCheckOutDate = bookingDTO.CheckOutDate;
            }

            // 保存 Room 更新
            await _roomRepository.UpdateAsync(room);

            return Ok(updatedBooking);
        }
        */
        // PATCH: api/Bookings/room/{roomNumber}

        [HttpPatch("room/{roomNumber}")]
        public async Task<IActionResult> UpdatePartialBooking(string roomNumber, [FromBody] JsonPatchDocument<BookingDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            // 查找房间
            var room = (await _roomRepository.GetAllAsync())
                        .FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("The specified Room does not exist.");
            }

            // 查找关联的 Booking
            var booking = (await _bookingRepository.GetAllAsync())
                          .FirstOrDefault(b => b.RoomId == room.Id);

            if (booking == null)
            {
                return NotFound("No booking found for the specified Room.");
            }

            // 创建一个 DTO 对象用于应用补丁
            var bookingDTO = new BookingDTO
            {
                Id = booking.Id,
                RoomNumber = room.RoomNumber,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate
            };

            // 应用补丁
            patchDoc.ApplyTo(bookingDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 更新 Booking 实体（仅更新被修改的字段）
            if (bookingDTO.CustomerName != booking.CustomerName)
            {
                booking.CustomerName = bookingDTO.CustomerName;
            }
            if (bookingDTO.CustomerEmail != booking.CustomerEmail)
            {
                booking.CustomerEmail = bookingDTO.CustomerEmail;
            }
            if (bookingDTO.CustomerPhone != booking.CustomerPhone)
            {
                booking.CustomerPhone = bookingDTO.CustomerPhone;
            }
            if (bookingDTO.CheckInDate != booking.CheckInDate)
            {
                booking.CheckInDate = bookingDTO.CheckInDate;
            }
            if (bookingDTO.CheckOutDate != booking.CheckOutDate)
            {
                booking.CheckOutDate = bookingDTO.CheckOutDate;
            }

            // 更新 Room 信息（同步 Booking 中更新的字段）
            room.OccupantName = booking.CustomerName;
            room.OccupantPhone = booking.CustomerPhone;
            room.CheckInDate = booking.CheckInDate;
            room.ExpectedCheckOutDate = booking.CheckOutDate;

            // 更新数据库
            await _bookingRepository.UpdateAsync(booking);
            await _roomRepository.UpdateAsync(room);

            // 返回更新后的 BookingDTO
            var updatedBookingDTO = new BookingDTO
            {
                Id = booking.Id,
                RoomNumber = room.RoomNumber,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate
            };

            return Ok(updatedBookingDTO);
        }





        // DELETE: api/Bookings/5
        // DELETE: api/Bookings/room/{roomNumber}
        [HttpDelete("room/{roomNumber}")]
        public async Task<IActionResult> CancelBookingByRoomNumber(string roomNumber)
        {
            // 根据房间号查找房间
            var room = (await _roomRepository.GetAllAsync())
                        .FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            // 查找与房间关联的预订记录
            var booking = (await _bookingRepository.GetAllAsync())
                          .FirstOrDefault(b => b.RoomId == room.Id);

            if (booking == null)
            {
                return NotFound("No booking found for the specified room.");
            }

            // 更新房间信息
            if (room.Status == "occupied")
            {
                room.Status = "Available";
                room.OccupantName = " ";
                room.OccupantPhone = " ";
                room.CheckInDate = null;
                room.ExpectedCheckOutDate = null;
                await _roomRepository.UpdateAsync(room);
            }

            // 删除预订记录
            await _bookingRepository.DeleteAsync(booking.Id);

            return NoContent();
        }




    }
}

