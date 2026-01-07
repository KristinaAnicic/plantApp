import { ImageDto } from "./image.interface";
import { PlantedDto } from "./planted.interface";
import { Reference } from "./reference.interface";

export interface PlantExchangeResponse {
    total: number;
    items: PlantExchangeDto[];
}

export interface PlantExchangeDto {
    id: number;
    title: string;
    exchangeType?: Reference;
    place?: string;
    image: string;
    price?: number;
    createdAt: string;
}

export interface PlantExchangeGetDto {
    user: Reference;
    planted?: PlantedDto;
    title: string;
    content: string;
    plantStatus: string;
    exchangeFor?: string;
    shipping: string;
    userRating?: number;
    images?: ImageDto[];
}

export interface UpsertPlantExchangeDto {
    plantedId?: number;
    title: string;
    content: string;
    plantStatus: string;
    contact: string;
    mainImage: string;
    isActive?: boolean;
    exchangeTypeId: number;
    city: string;
    countryId: number;
    exchangeFor?: string;
    price?: number;
    shipping: string;
    images?: string[];
}