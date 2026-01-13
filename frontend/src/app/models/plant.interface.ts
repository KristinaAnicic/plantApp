import { ImageDto } from "./image.interface";
import { Reference } from "./reference.interface";

export interface PlantListResponse {
    total: number;
    items: PlantDto[];
}

export interface PlantDto {
    plantId: number;
    botanicalName: string;
    commonName: string;
    entityDescription?: string;
    image?: string;
}

export interface PlantGetDto extends PlantDto {
    fragrance?: string;
    hardinessLevel?: string;
    isSpecie?: boolean;
    isGenus?: boolean;
    isPlantForPollinators?: boolean;
    isLowMaintenance?: boolean;
    isDroughtResistant?: boolean;
    spreadType?: string;
    heightType?: string;
    timeToFullHeight?: string;
    toxicity?: string;
    cultivation?: string;
    pestResistance?: string;
    diseaseResistance?: string;
    pruning?: string;
    propagation?: string;
    family?: string;
    genusDescription?: string;
    soilTypes?: string;
    images?: ImageDto[];
    sunlights?: string;
    aspects?: string;
    moistures?: string;
    phs?: string;
    exposures?: string;
    habits?: string[];
    seasons?: string[];
    synonyms?: Reference[];
    parentPlant?: Reference;
}

export interface UpsertPlantDto {
    id?: number;
    botanicalName: string; // required
    commonName: string;    // required
    synonymParentPlantId?: number;
    fragranceId?: number;
    hardinessLevelId?: number;
    isSpecie?: boolean;
    isGenus?: boolean;
    isPlantForPollinators?: boolean;
    isLowMaintenance?: boolean;
    isDroughtResistant?: boolean;
    spreadTypeId?: number;
    heightTypeId?: number;
    timeToFullHeightId: number; // required
    toxicity?: string;
    cultivation?: string;
    pestResistance?: string;
    diseaseResistance?: string;
    pruning?: string;
    propagation?: string;
    familyId?: number;
    entityDescription?: string;
    genusDescription?: string;

    soilTypes: number[];
    images: string[];
    sunlights: number[];
    aspects: number[];
    moistures: number[];
    phs: number[];
    exposures: number[];
    habits: number[];
    seasons: number[];
}