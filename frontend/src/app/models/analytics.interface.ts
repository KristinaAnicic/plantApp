import { PlantedDto } from "./planted.interface";

export interface AnalyticsDto
{
    summary: PlantSummary;
    reminderStats: ReminderStat[];
    healthStats: HealthOverview[];
    growthLogActivity: MonthlyActivityDto[];
    seasonalPlanting: MonthlyActivityDto[];
    actionStats: ActionFrequencyDto[];
    hallOfFame?: PlantHallOfFame;
    healthPrediction: HealthPrediction[];

}

export interface PlantSummary
{
    numOfPlants: number;
    numOfCurrentPlants: number;
    numOfDeadPlants: number;
    numOfLogsThisYear: number;
    numOfLogsOverAll: number;
    firstPlantedDate: string;
}

export interface HealthPrediction
{
    plantName: string;
    placeName: string;
    monthlyPrediction: number[];
    currentSuccessProbability: number;
}

export interface ReminderStat
{
    label: string;
    percentage: number;
}

export interface HealthOverview
{
    label: string;
    percentage: number;
}

export interface MonthlyActivityDto
{
    year: number;
    month: number;
    count: number;
}

export interface ActionFrequencyDto
{
    actionType: string;
    count: number
}

export interface PlantHallOfFame
{
    oldestPlant: PlantedDto;
    daysAlive: number;
    mostResilientPlant: PlantedDto;
    numOfLateReminder: number;
}
