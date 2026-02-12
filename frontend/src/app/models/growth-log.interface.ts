import { ImageDto } from "./image.interface";
import { Reference } from "./reference.interface";

export interface GrowthLogDto {
    id: number;
    plantedId?: number;
    plantGroupId?: number;
    plant?: string;
    note?: string;
    title: string;
    plantStatus?: string;
    observationDate?: string;
    images?: ImageDto[];
}

export interface GrowthLogGetDto {
    id: number;
    plantedId?: number;
    plantGroupId?: number;
    title: string;
    plant?: string;
    note?: string;
    plantStatus?: Reference;
    observationDate?: string;
    images?: ImageDto[];
}

export interface UpsertGrowthLogDto {
    id?: number;  
    plantedId?: number;
    plantGroupId?: number;
    title: string;
    note?: string;
    plantStatusId: number;
    images: string[];
    observationDate?: string;
}