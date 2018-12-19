namespace Domain.Models
{
    public class Airport
    {
        public string Code { get; set; }
        public string Aliases { get; set; }
        public string Name { get; set; }
        public string Description { get; }
        public string Timezone { get; set; }
        public string Country { get; set; }
        public bool UseAliases { get; set; }
        public bool ExcludeFromSearch { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool M2iorigin { get; set; }
    }
}
