import { Reference } from "./reference.interface";

export interface UserRatingDto {
    rater: Reference; 
    rated: Reference; 
    rating: number; 
    comment: string;  
    createdAt: string;   
    updatedAt?: string;  
}

export interface AddUserRatingDto {
    ratedUserId: number; 
    rating: number;   
    comment: string; 
}

export interface UpdateUserRatingDto {
    rating: number;    
    comment: string;   
}