import Axios from "axios";
import {
  OperationResult,
  LoginRequestDto,
  LoginResponseDto,
  InstanceGetDto,
  SwarmService,
  InstanceChangeTemplateDto,
  TemplateGetDto,
  UserGetDto,
  InstanceRunCommandDto,
  TemplateCommandExecDto,
  InstanceSignalDto,
  UserAddDto,
  InstanceAddDto
} from "../models";
import Cookies from 'js-cookie';

var axios = Axios.create({
  baseURL: 'http://localhost:57019/api',
});

axios.interceptors.request.use(config => {
  if (!config.headers) config.headers = {};
  config.headers.Authorization = Cookies.get('token');
  return config;
});

axios.interceptors.response.use(response => {
  return {
    ...response,
    data: { ...response?.data }
  };
}, error => {
  if (error?.response?.status === 401) {
    window.location.hash = '/';
  }
  return {
    ...error?.response,
    data: {
      success: false,
      ...error?.response?.data
    }
  };
});


export function Auth_Login(dto: LoginRequestDto) { return axios.post<OperationResult<LoginResponseDto>>(`Auth/Login`, dto); }
export function Auth_ChangeUser(id: number) { return axios.post<OperationResult<LoginResponseDto>>(`Auth/ChangeUser/${id}`); }
export function Auth_SetLoginInfo(dto: LoginResponseDto) {
  Cookies.set('token', dto.token);
  localStorage.setItem('username', dto.username);
  localStorage.setItem('roles', JSON.stringify(dto.roles));
}
export function Auth_HasRole(value: string) {
  const rolesJson = localStorage.getItem('roles');
  if (rolesJson) {
    try {
      const roles = JSON.parse(rolesJson) as string[];
      value = value.toLowerCase();
      if (roles.findIndex(x => x.toLowerCase().indexOf('admin') > -1) > -1) return true;
      return roles.findIndex(x => x.toLowerCase().indexOf(value) > -1) > -1;
    } catch {

    }
  }
  return false;
}

export function Docker_GetCommandLog(id: string) { return axios.get<OperationResult<string>>(`Dockers/Commands/${id}/Log`); }

export function Instance_GetAllOwn() { return axios.get<OperationResult<InstanceGetDto[]>>(`Instances/Own`); }
export function Instance_GetAllService(id: number) { return axios.get<OperationResult<SwarmService[]>>(`Instances/Own/${id}/Services`); }
export function Instance_GetAllCommand(id: number) { return axios.get<OperationResult<TemplateCommandExecDto[]>>(`Instances/Own/${id}/Commands`); }
export function Instance_Signal() { return axios.get<OperationResult<InstanceSignalDto[]>>(`Instances/Own/Signal`); }
export function Instance_Add(dto: InstanceAddDto) { return axios.post<OperationResult<InstanceGetDto>>(`Instances`, dto); }
export function Instance_ChangeOwnTemplate(id: number, dto: InstanceChangeTemplateDto) { return axios.patch<OperationResult<InstanceGetDto>>(`Instances/Own/${id}/ChangeTemplate`, dto); }
export function Instance_StartOwn(id: number) { return axios.put<OperationResult<InstanceGetDto[]>>(`Instances/Own/${id}/Start`); }
export function Instance_StopOwn(id: number) { return axios.put<OperationResult<InstanceGetDto[]>>(`Instances/Own/${id}/Stop`); }
export function Instance_RunCommand(id: number, commandId: number, dto?: InstanceRunCommandDto) { return axios.put<OperationResult<InstanceGetDto>>(`Instances/Own/${id}/RunCommand/${commandId}`, dto); }
export function Instance_Remove(id: number) { return axios.delete<OperationResult<InstanceGetDto>>(`Instances/${id}`); }

export function Template_GetAll() { return axios.get<OperationResult<TemplateGetDto[]>>(`Templates`); }

export function User_GetAll() { return axios.get<OperationResult<UserGetDto[]>>(`Users`); }
export function User_Add(dto: UserAddDto) { return axios.post<OperationResult<UserGetDto>>(`Users`, dto); }
