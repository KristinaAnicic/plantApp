import { PlantedDto } from "./planted.interface";

export interface PlaceDto {
    id: number;
    name: string;
    address?: string;
}

export interface PlaceGetDto {
    id: number;
    name: string;
    address?: string;
    city?: string;
    note?: string;
    planted: PlantedDto[];
}

export interface UpsertPlaceDto {
    id?: number;
    name: string;
    address?: string;
    city: string;
    note?: string;
    countryId: number;
}