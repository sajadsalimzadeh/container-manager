
export interface ImageGetDto {
    id: number;
    name: string;
    buildPath: string;
    buildCron: string;
    lifeTime: number;
}

export interface ImageAddDto {
    name: string;
    buildPath: string;
    buildCron: string;
    lifeTime: number;
}

export interface ImageChangeDto {
    name: string;
    buildPath: string;
    buildCron: string;
    lifeTime: number;
}

export interface ImageBuildLogDto {
    buildName: string;
    scripts: string[];
}