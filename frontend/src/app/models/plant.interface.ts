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
    fragrance?: Reference;
    hardinessLevel?: Reference;
    isSpecie?: boolean;
    isGenus?: boolean;
    isPlantForPollinators?: boolean;
    isLowMaintenance?: boolean;
    isDroughtResistant?: boolean;
    spreadType?: Reference;
    heightType?: Reference;
    timeToFullHeight?: Reference;
    toxicity?: string;
    cultivation?: string;
    pestResistance?: string;
    diseaseResistance?: string;
    pruning?: string;
    propagation?: string;
    family?: Reference;
    genusDescription?: string;
    soilTypes?: Reference[];
    images?: ImageDto[];
    sunlights?: Reference[];
    aspects?: Reference[];
    moistures?: Reference[];
    phs?: Reference[];
    exposures?: Reference[];
    habits?: Reference[];
    seasons?: Reference[];
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