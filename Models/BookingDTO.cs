public class BookingDTO
{
    public int?Id { get; set; }
    //public int RoomId { get; set; }
    public string RoomNumber { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
  
    // 添加 TotalPrice 属性（只读）
    public decimal TotalPrice { get; set; }
}

