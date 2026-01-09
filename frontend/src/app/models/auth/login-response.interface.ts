import { UserDto } from "../user.interface";

export interface LoginResponse {
    accessToken: string,
    user: UserDto
}