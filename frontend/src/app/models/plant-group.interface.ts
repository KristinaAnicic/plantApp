import { GrowthLogGetDto } from "./growth-log.interface";
import { PlantedDto } from "./planted.interface";
import { ReminderGetDto } from "./reminder.interface";

export interface PlantGroupDto {
    id: number;
    name: string;
    description?: string;
    numOfPlants: number;
}

export interface PlantGroupGetDto {
    id: number;
    name: string;
    description?: string;
    planted?: PlantedDto[];
    growthLogs?: GrowthLogGetDto[];
    reminders?: ReminderGetDto[];
}

export interface UpsertPlantGroupDto {
    id?: number;
    name: string;
    description?: string;
}