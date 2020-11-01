import { ContainerListResponse, SwarmService } from "./docker";
import { TemplateCommandExecDto, TemplateGetDto } from "./template";
import { UserGetDto } from "./user";

export interface InstanceSignalDto {
    instanceId: number;
    services: SwarmService[];
    containers: ContainerListResponse[];
    templateCommandExecs: TemplateCommandExecDto[];
}



export interface InstanceGetDto {

    id: number;
    code: number;
    userId: number;
    templateId: number;
    name: string;
    environments: any;
    user: UserGetDto;
    template: TemplateGetDto;

    services?: SwarmService[];
    containers?: ContainerListResponse[];
    commadns?: TemplateCommandExecDto[];

    isStarting?: boolean;
    isStopping?: boolean;
    isRemoving?: boolean;
}

export interface InstanceAddDto {
    userId: number;
    name: string;
}

export interface InstanceChangeTemplateDto {
    templateId: number;
}

export interface InstanceRunCommandDto {

}