import { Reference } from "./reference.interface";

export interface UserRatingDto {
    id: number;
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
    id?: number;
    rating: number;
    comment: string;
}