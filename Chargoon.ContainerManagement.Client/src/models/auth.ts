export interface LoginRequestDto {
    username: string;
    password: string;
}

export interface LoginResponseDto {
    id: number;
    token: string;
    username: string;
    roles: string[]
}