import { ImageDto } from "./image.interface";
import { PlantedDto } from "./planted.interface";
import { Reference } from "./reference.interface";
import { UserRatingDto } from "./user-rating.interface";

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
    id: number;
    title: string;
    exchangeType?: Reference;
    country?: Reference;
    city: string;
    image: string;
    contact: string;
    price?: number;
    createdAt: string;
    user: Reference;
    planted?: PlantedDto;
    content: string;
    plantStatus: string;
    exchangeFor?: string;
    shipping: string;
    userRating?: number;
    images?: ImageDto[];
    userRatings?: UserRatingDto[];
}

export interface UpsertPlantExchangeDto {
    id?: number;
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
    images: string[];
}

export interface PlantExchangeReference {
    planted: Reference[];
    exchangeTypes: Reference[]
}