import { InstanceGetDto } from "./instance";

export interface UserGetDto {
    id: number;
    username: string;
    host: string;
    roles: string[];
    instances: InstanceGetDto[];
}

export interface UserAddDto {
    username: string;
    password: string;
    roles: string[];
    instances: string;
}

export interface UserChangeProfileDto {
    host: string;
}

export interface UserChangePasswordDto {
    currentPassword: string;
    newPassword: string;
}

export interface UserResetPasswordDto {
    newPassword: string;
}