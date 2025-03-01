using System;
using System.Collections.Generic;

namespace HotelBookingSystem.Models
{
    public class HotelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 添加额外字段
        public int TotalRooms { get; set; } // 总房间数
        public int AvailableRooms { get; set; } // 空房间数
        public List<string> RoomNumbers { get; set; } // 房间号列表
    }
}


