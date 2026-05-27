namespace PlantApp.Domain.Dtos.Plant;

public class ReferenceDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class SunlightDto : ReferenceDto { }
public class PhDto : ReferenceDto { }
public class MoistureDto : ReferenceDto { }
public class AspectDto : ReferenceDto { }
public class SoilTypeDto : ReferenceDto { }
public class ExposureDto : ReferenceDto { }
public class HabitDto : ReferenceDto { }
public class SpreadTypeDto : ReferenceDto { }
public class HeightTypeDto : ReferenceDto { }
public class TimeToFullHeightDto : ReferenceDto { }
public class HardinessLevelDto : ReferenceDto { }
public class FragranceDto : ReferenceDto { }
public class PlantFamilyDto : ReferenceDto { }
