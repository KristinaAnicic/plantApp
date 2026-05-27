namespace PlantApp.Domain.Dtos.ML
{
    public class PlantedGrowthLogOverviewDto
    {
        public int PlantedId { get; set; }
        public int SunlightIntensity { get; set; }
        public int HumidityIntensity { get; set; }
        public bool IsOutside { get; set; }
        public string Family { get; set; } = null!;
        public double Hardiness { get; set; }
        public int PlantStatusId { get; set; }

        public List<int> SunlightList { get; set; } = null!;
        public List<int> MoistureList { get; set; } = null!;
        public List<int> Seasons { get; set; } = null!;

        public bool LowMaintenance { get; set; }
        public bool DroughtResistant { get; set; } 
        public int Month { get; set; }  
        public int DaysSincePlanted { get; set; }    
        public double ReminderDelay { get; set; }
    }
}
