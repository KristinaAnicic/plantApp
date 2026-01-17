import { PlantedDto } from "./planted.interface";
import { Reference } from "./reference.interface";

export interface PlaceDto {
    id: number;
    name: string;
    address?: string;
    numOfPlants: number;
    note?: string;
}

export interface PlaceGetDto {
    id: number;
    name: string;
    address?: string;
    city?: string;
    country: Reference;
    note?: string;
    planted?: PlantedDto[];
}

export interface UpsertPlaceDto {
    id?: number;
    name: string;
    address?: string;
    city: string;
    note?: string;
    countryId: number;
}