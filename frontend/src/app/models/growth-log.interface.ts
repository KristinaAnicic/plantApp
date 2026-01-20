import { ImageDto } from "./image.interface";
import { Reference } from "./reference.interface";

export interface GrowthLogDto {
    id: number;
    plantedId: number;
    plant?: string;
    note?: string;
    title: string;
    plantStatus?: string;
    createdAt?: string;
    images?: ImageDto[];
}

export interface GrowthLogGetDto {
    id: number;
    plantedId: number;
    title: string;
    plant?: string;
    note?: string;
    plantStatus?: Reference;
    createdAt?: string;
    images?: ImageDto[];
}

export interface UpsertGrowthLogDto {
    id?: number;  
    plantedId: number;
    title: string;
    note?: string;
    plantStatusId: number;
    images?: string[];
}