using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; } // 单人/双人
        public decimal RoomPrice { get; set; } // 每晚价格
        public string Status { get; set; } = "Available"; // Available, Booked, Occupied
        public int HotelId { get; set; }
        [JsonIgnore] // 忽略对 Hotel 的序列化
        public Hotel Hotel { get; set; }
        public string OccupantName { get; set; } // 当前入住人姓名
        public string OccupantPhone { get; set; } // 当前入住人电话
        public DateOnly? CheckInDate { get; set; } // 入住时间
        public DateOnly? ExpectedCheckOutDate { get; set; } // 预计离开时间

        public ICollection<Booking> Bookings { get; set; }
    }
}
