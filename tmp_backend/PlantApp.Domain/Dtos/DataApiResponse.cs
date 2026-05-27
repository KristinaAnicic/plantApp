namespace PlantApp.Domain.Dtos;

public class DataApiResponse
{
    public int id { get; set; }
    public bool? isSynonym { get; set; }
    public int? synonymParentPlantId { get; set; }
    public string? botanicalNameUnFormatted { get; set; }
    public bool? notedForFragrance { get; set; }
    public string? fragrance { get; set; } // (Deciduous, evergreen, foliage, semi-evergreen)
    // if not in list add to list
    public Image[]? images { get; set; } // međutablica
    public string? commonName { get; set; }
    public int? hardinessLevel { get; set; }
    public bool? isGenus { get; set; }
    public bool? isSpecie { get; set; }
    public bool? isPlantsForPollinators { get; set; }
    public bool? isLowMaintenance { get; set; }
    public bool? isDroughtResistance { get; set; }
    public int[]? sunlight { get; set; } // međutablica
    public int[]? soilType { get; set; } // međutablica
    public int[]? spreadType { get; set; } // SAMO JEDAN!!
    public int[]? heightType { get; set; } // SAMO JEDAN!!
    public int[]? timeToFullHeight { get; set; } // SAMO JEDAN!!
    public int[]? aspect { get; set; } // međutablica
    public int[]? moisture { get; set; } // međutablica
    public int[]? ph { get; set; } // međutablica
    public int[]? exposure { get; set; } // međutablica
    public int[]? plantType { get; set; } // međutablica
    public int[]? foliage { get; set; } // SAMO JEDAN!! (tekst, ne misli se na sliku!)
    public int[]? habit { get; set; } // međutablica! (provjereno s id 137374)
    public string[]? toxicity { get; set; } // leave as string
    public int[]? seasonOfInterest { get; set; } // međutablica
    public Colourwithattribute[]? colourWithAttributes { get; set; }
    public string? cultivation { get; set; } // leave as string
    public string? pestResistance { get; set; } // leave as string
    public string? diseaseResistance { get; set; } // leave as string
    public string? pruning { get; set; } // leave as string
    public string? propagation { get; set; } // leave as string
    public string? family { get; set; } // kao int (if not in list, add to list)
    public string? entityDescription { get; set; } // leave as string
    public string? genusDescription { get; set; } // leave as string
}

public class Image
{
    public string? image { get; set; }
    public string? copyRight { get; set; }
}

public class Colourwithattribute
{
    public int? season { get; set; }
    public int? colour { get; set; }
    public int? attributeType { get; set; }
}
