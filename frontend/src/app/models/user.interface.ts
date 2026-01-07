import { PlaceGetDto } from "./place.interface";
import { PlantExchangeDto } from "./plant-exchange.interface";

export interface UserDto{
    id: number;
    email: string;
    username: string;
    displayName: string;
    role?: string;
    roleId: number;
    gender: string;
    dateOfBirth: string;
    rating?: number;
    numOfRatings?: number;
}

export interface UserGetDto {
    id: number;
    email: string;
    username: string;
    displayName: string;
    role?: string;
    roleId: number;
    gender: string;
    dateOfBirth: string;
    rating?: number;
    numOfRatings?: number;
    places: PlaceGetDto[];
    plantExchanges: PlantExchangeDto[];
}

export interface AddUserDto {
    email: string;
    username: string;
    password: string;
    displayName: string;
    contact?: string;
    gender: string;
    dateOfBirth: string;
    roleId?: string;
}

export interface UpdateUserDto {
    id: number;
    displayName: string;
    contact?: string;
    gender: string;
    dateOfBirth: string;
    roleId?: string;
}

