export interface PlantFilterDto {
    name?: string;      
    isLowMaintenance?: boolean;
    isDroughtResistant?: boolean;
    habits?: number[];
    soilType?: number[];
    spread?: number;
    height?: number;
    timeToFullHeight?: number;
    exposure?: number;
}

export interface PlantExchangeFilterDto {
    name?: string;
    exchangeType?: number;
    priceFrom?: number;
    priceTo?: number;
    city?: string;
}