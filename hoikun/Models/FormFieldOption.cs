namespace hoikun.Models
{
    public class FormFieldOption
    {
        public int Id { get; set; }
        public int FormFieldId { get; set; }
        public string Option { get; set; } = string.Empty;
    }
}
