import { Reference } from "./reference.interface";

export interface ReminderDto {
    id: number;
    plantedId: number;
    plant: string;
    reminderType?: string;
    nextDueDate: string;
    notes?: string;
    isLate: boolean;
}

export interface ReminderGetDto {
    id: number;
    plantedName?: string;
    plantedId: number;
    reminderType?: Reference;
    frequencyType?: Reference;
    nextDueDate: string;
    daysDelayed: number;
    frequencyNum: number;
    notes?: string;
    isLate: boolean;
}

export interface UpsertReminderDto {
    id?: number;
    plantedId: number;
    reminderTypeId: number;
    frequencyTypeId: number;
    frequencyNum: number;
    nextDueDate: string;
    notes?: string;
}