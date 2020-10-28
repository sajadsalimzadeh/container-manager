export interface OperationResult<T = any> {
    data: T;
    code: number;
    message: string;
    success: boolean;
}

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

export interface UserGetDto {
    id: number;
    username: string;
    roles: string[];
    instances: InstanceGetDto[];
}

export interface UserAddDto {
    username: string;
    password: string;
    roles: string[];
    instances: string;
}
export interface UserChangePasswordDto {
    currentPassword: string;
    newPassword: string;
}

export enum TemplateCommandColor {
    None = 0,
    Red = 1,
    Blue = 2,
    Black = 3,
    Green = 4,
    Yellow = 5,
}

export interface InstanceSignalDto {
    instanceId: number;
    services: SwarmService[];
    templateCommandExecs: TemplateCommandExecDto[];
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
    dockerCompose: any;
    environments: any;
    isActive: boolean;
    commands: TemplateCommandGetDto[];
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

export interface TemplateCommandExecDto {
    commandId: string;
    templateCommandId: number;

    inspect?: ContainerExecInspectResponse;
    templateCommand?: TemplateCommandGetDto;

    isLoadingLogs?: boolean;
    logs?: string;
}

export interface ContainerExecInspectResponse {
    execID: string;
    containerID: string;
    running: boolean;
    exitCode: number;
    pid: number;
}

export interface TaskSpec {

}

export interface ServiceSpec {
    name: string;
    labels: any;
    taskTemplate: any;
    mode: {
        replicated: {
            replicas: number
        }
        global: any;
    };
    updateConfig: any;
    rollbackConfig: any;
    networks: any[];
    endpointSpec: any;
}

export interface PortConfig {
    name?: string;
    protocol?: string;
    targetPort: number;
    publishedPort: number;
    publishMode?: string;
}

export interface EndpointVirtualIIP {
    networkID?: string;
    addr?: string;
}

export interface EndpointSpec {
    mode?: string;
    ports?: PortConfig[];
}

export interface Endpoint {
    spec: EndpointSpec;
    ports?: PortConfig[];
    virtualIPs?: EndpointVirtualIIP[];
}

export interface UpdateStatus {
    state: string;
    startedAt: string;
    completedAt: string;
    message: string;
}


export interface ServiceStatus {
    runningTasks: number;
    desiredTasks: number;
}

export interface SwarmService {
    id: string;
    version: { index: number },
    createdAt: string;
    updatedAt: string;
    spec: ServiceSpec;
    endpoint: Endpoint;
    updateStatus: UpdateStatus;
    serviceStatus: ServiceStatus;
}

export interface BranchGetDto {
    id: number;
    name: string;
    dockerCompose: any;
    isBranchEnable: boolean;
}

export interface BranchAddDto {
    name: string;
    dockerCompose: any;
    isBranchEnable: boolean;
}

export interface BranchChangeDto {
    name: string;
    dockerCompose: any;
    isBranchEnable: boolean;
}