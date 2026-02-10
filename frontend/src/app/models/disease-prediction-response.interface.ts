export interface DiseasePredictionResponse {
    results: DiseasePrediction[],
    main_prediction: string
}

export interface DiseasePrediction{
    disease: string,
    confidence: string,
}