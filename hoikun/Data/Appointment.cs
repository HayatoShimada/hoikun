namespace hoikun.Data
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string Caption { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int Label { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
