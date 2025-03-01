public class RoomDTO
{
    public string RoomNumber { get; set; }
    public string RoomType { get; set; }
    public decimal RoomPrice { get; set; }
    public string Status { get; set; }
    public string HotelName { get; set; }
    public string OccupantName { get; set; } // 入住人姓名
    public string OccupantPhone { get; set; } // 入住人电话
    public DateOnly? CheckInDate { get; set; } // 入住时间
    public DateOnly? ExpectedCheckOutDate { get; set; } // 预计离开时间
}

