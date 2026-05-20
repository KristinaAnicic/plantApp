export interface PlantNetResponse {
    results: PlantNetResult[];
    bestMatch: string;
}

export interface PlantNetResult {
    score: number;
    species?: PlantNetSpecies
}

export interface PlantNetSpecies {
    scientificNameWithoutAuthor: string;
    scientificNameAuthorship: string;
    genus: PlantNetScientificName;
    family: PlantNetScientificName;
    commonNames: string[];
    scientificName: string;
}

export interface PlantNetScientificName {
    scientificNameWithoutAuthor: string;
    scientificNameAuthorship: string;
    scientificName: string;
}