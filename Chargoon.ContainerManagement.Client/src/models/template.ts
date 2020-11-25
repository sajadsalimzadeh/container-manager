import { ContainerExecInspectResponse } from "./docker";


export enum TemplateCommandColor {
    None = 0,
    Red = 1,
    Blue = 2,
    Black = 3,
    Green = 4,
    Yellow = 5,
}

export interface TemplateCommandGetDto {
    id: number;
    templateId: number;
    name: string;
    serviceName: string;
    color: TemplateCommandColor;
    runOnStartup: boolean;

    isRunning?: boolean;
}

export interface TemplateGetDto {
    id: number;
    name: string;
    environments: any;
    isActive: boolean;
    insertCron: string;
    insertLifeTime: number;
    expireTime: string;
    description: string;
    dockerComposeObj: any;
    dockerCompose: string;
    beforeStartCommand: string;
    afterStartCommand: string;
    beforeStoptCommand: string;
    afterStopCommand: string;

    commands: TemplateCommandGetDto[];
}

export interface TemplateAddDto {
    name: string;
    environments: any;
    isActive: boolean;
    insertCron: string;
    insertLifeTime: number;
    expireTime: string;
    description: string;
    dockerCompose: string;
    beforeStartCommand: string;
    afterStartCommand: string;
    beforeStoptCommand: string;
    afterStopCommand: string;
}

export interface TemplateChangeDto {
    name: string;
    environments: any;
    isActive: boolean;
    insertCron: string;
    insertLifeTime: number;
    expireTime: string;
    description: string;
    dockerCompose: string;
    beforeStartCommand: string;
    afterStartCommand: string;
    beforeStoptCommand: string;
    afterStopCommand: string;
}

export interface TemplateCommandExecDto {
    commandId: string;
    templateCommandId: number;

    inspect?: ContainerExecInspectResponse;
    templateCommand?: TemplateCommandGetDto;

    isLoadingLogs?: boolean;
    logs?: string;
}