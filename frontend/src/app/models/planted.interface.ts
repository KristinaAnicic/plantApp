import { GrowthLogDto, GrowthLogGetDto } from "./growth-log.interface";
import { ImageDto } from "./image.interface";
import { PlaceDto } from "./place.interface";
import { PlantDto } from "./plant.interface";
import { Reference } from "./reference.interface";
import { ReminderDto, ReminderGetDto } from "./reminder.interface";

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
    datePlantedString: string;
    lastUpdate: string,
    source?: string;
    note?: string;
    isOutside: boolean;
    plantStatus?: Reference;
    nextReminders?: ReminderGetDto[];
    growthLogs?: GrowthLogGetDto[];
    images?: ImageDto[];
    image?: string;
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
    images: string[];
}

export interface PlantedReference {
    places: Reference[],
    plantStatuses: Reference[]
}
