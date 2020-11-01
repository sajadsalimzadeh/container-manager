export * from './auth';
export * from './docker';
export * from './image';
export * from './instance';
export * from './template';
export * from './user';

export interface OperationResult<T = any> {
    data: T;
    code: number;
    message: string;
    success: boolean;
}