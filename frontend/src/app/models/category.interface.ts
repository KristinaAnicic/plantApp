import { Reference } from "./reference.interface";

export interface OnePlantAttributesDto {
    spreadTypes: Reference[]; 
    heightTypes: Reference[];
    timeToFullHeights: Reference[];
    hardinessLevels: Reference[];
    fragrances: Reference[];
    families: Reference[];
}

export interface ManyPlantAttributesDto {
    sunlights: Reference[];
    phs: Reference[];
    moistures: Reference[];
    aspects: Reference[];
    soilTypes: Reference[];
    exposures: Reference[];
    habits: Reference[];
    seasons: Reference[];
}