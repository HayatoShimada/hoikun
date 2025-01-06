namespace hoikun.Enums
{
    public static class StatusCollection
    {
        public static List<StatusObject> GetStatuses()
        {
            DateTime date = DateTime.Today;
            List<StatusObject> dataSource = new() {
            new StatusObject() {
                Id = 1,
                StatusCaption = "Resolved",
                StatusColor = System.Drawing.Color.LightBlue,
                // Uncomment the line below and comment the line above to specify other style options.
                //CssClass = "status1-style",
                MyCustomField = "Custom text for the 'Resolved' status",
            },
            new StatusObject() {
                Id = 2,
                StatusCaption = "In process",
                StatusColor = System.Drawing.Color.LightGreen,
                // Uncomment the line below and comment the line above to specify other style options.
                //CssClass = "status2-style",
                MyCustomField = "Custom text for the 'In process' status",
            }
        };
            return dataSource;
        }
    }

    public class StatusObject
    {
        public int Id { get; set; }
        public string StatusCaption { get; set; } = string.Empty;
        public System.Drawing.Color StatusColor { get; set; }
        public string CssClass { get; set; } = string.Empty;
        public string MyCustomField { get; set; } = string.Empty; // A custom field
    }
}
