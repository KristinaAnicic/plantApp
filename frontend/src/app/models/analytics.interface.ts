import { PlantedDto } from "./planted.interface";

export interface AnalyticsDto
{
    summary: PlantSummary;
    reminderStats: PercentageStat[];
    healthStats: PercentageStat[];
    growthLogActivity: MonthlyActivityDto[];
    seasonalPlanting: MonthlyActivityDto[];
    actionStats: ActionFrequencyDto[];
    hallOfFame?: PlantHallOfFame;
    healthPrediction: HealthPrediction[];
    plantRecommendations: string[];
    groupPlantSuccess: PercentageStat[];
    familyPlantSuccess: PercentageStat[];
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

export interface PercentageStat
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

export interface PlantedAnalyticsDto
{
    monthlyHealthPrediction: number[];
    plantGrowthHeight: PlantGrowthHeight[];

}

export interface PlantGrowthHeight
{
    month: number;
    height: number;
    activeAttributes: string[];
}

export interface GroupedGrowthAnalytics
{
    planted: PlantedDto;
    plantGrowthHeight: PlantGrowthHeight[];
}

export interface PlantGroupLogAnalytics
{ 
    month: number;
    avgHealth: number;
}

export interface PlantGroupAnalytics
{
    groupLogAnalytics: PlantGroupLogAnalytics[];
    growthAnalytics: GroupedGrowthAnalytics[];
}
