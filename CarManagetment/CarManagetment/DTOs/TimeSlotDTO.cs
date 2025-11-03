namespace CarManagetment.DTOs
{
    public class TimeSlotDTO
    {
        public int TimeSlot_Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
