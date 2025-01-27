namespace hoikun.Models
{
    public class FormFieldModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string? Options { get; set; } // カンマ区切りのオプション文字列
        public List<FormFieldOption> OptionList { get; set; } = [];
    }
}
