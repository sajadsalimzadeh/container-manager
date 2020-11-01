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

    isRunning?: boolean;
}

export interface TemplateGetDto {
    id: number;
    name: string;
    dockerCompose: string;
    dockerComposeObj: any;
    environments: any;
    insertCron: string;
    isActive: boolean;
    commands: TemplateCommandGetDto[];
}

export interface TemplateAddDto {
    name: string;
    dockerCompose: string;
    environments: any;
    insertCron: string;
    isActive: boolean;
}

export interface TemplateChangeDto {
    name: string;
    dockerCompose: string;
    environments: any;
    insertCron: string;
    isActive: boolean;
}

export interface TemplateCommandExecDto {
    commandId: string;
    templateCommandId: number;

    inspect?: ContainerExecInspectResponse;
    templateCommand?: TemplateCommandGetDto;

    isLoadingLogs?: boolean;
    logs?: string;
}