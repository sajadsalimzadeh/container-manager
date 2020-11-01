
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

export interface ContainerExecInspectResponse {
    execID: string;
    containerID: string;
    running: boolean;
    exitCode: number;
    pid: number;
}

export interface ContainerListResponse {
    iD: string;
    names: string[];
    image: string;
    imageID: string;
    command: string;
    created: string;
    Ports: PortConfig[];
    sizeRw: number;
    sizeRootFs: number;
    labels: any;
    state: string;
    status: string;
    networkSettings: any;
    mounts: any[];
}