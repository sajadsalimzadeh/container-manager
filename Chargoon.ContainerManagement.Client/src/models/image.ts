
export interface ImageGetDto {
    id: number;
    name: string;
    buildPath: string;
    buildCron: string;
}

export interface ImageAddDto {
    name: string;
    buildPath: string;
    buildCron: string;
}

export interface ImageChangeDto {
    name: string;
    buildPath: string;
    buildCron: string;
}

export interface ImageBuildLogDto {
    buildName: string;
    scripts: string[];
}