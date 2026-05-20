using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.DiseasePrediction;

public class DiseasePredictionDto
{
    [JsonPropertyName("disease")]
    public string? Disease {  get; set; }
    [JsonPropertyName("confidence")]
    public string? Confidence { get; set; }
}
