import { GrowthLogDto } from "./growth-log.interface";
import { ImageDto } from "./image.interface";
import { PlaceDto } from "./place.interface";
import { PlantDto } from "./plant.interface";
import { ReminderDto } from "./reminder.interface";

export interface PlantedDto {
    id: number;
    plantName: string;
    //name?: string;
    place: string;
    datePlanted: string;
    plantStatus: string;
    image: string;
}

export interface PlantedGetDto {
    id: number
    name: string
    place: PlaceDto
    plant: PlantDto
    datePlanted: string;
    source?: string;
    note?: string;
    isOutside: boolean;
    plantStatus: string;
    nextReminders?: ReminderDto[];
    growthLogs?: GrowthLogDto[];
    images?: ImageDto[]
}

export interface GroupedPlantedDto {
    place: PlaceDto;
    planted: PlantedDto[];
}

export interface UpsertPlantedDto {
    id?: number;
    name?: string;
    plantId: number;
    placeId: number;
    datePlanted?: string; 
    source?: string;
    note?: string;
    isOutside?: boolean;   
    image?: string;
    plantStatusId: number;
    images?: string[];
}