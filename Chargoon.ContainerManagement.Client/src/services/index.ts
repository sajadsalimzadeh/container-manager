import Axios, { AxiosInstance } from "axios";
import {
  OperationResult,
  LoginRequestDto,
  LoginResponseDto,
  InstanceGetDto,
  SwarmService,
  InstanceChangeTemplateDto,
  TemplateGetDto,
  TemplateAddDto,
  TemplateChangeDto,
  UserGetDto,
  UserAddDto,
  UserChangePasswordDto,
  UserResetPasswordDto,
  InstanceRunCommandDto,
  TemplateCommandExecDto,
  InstanceSignalDto,
  InstanceAddDto,
  ImageGetDto,
  ImageAddDto,
  ImageChangeDto,
  ImageBuildLogDto,
  CustomeNightlyBuildLog,
  UserChangeProfileDto
} from "../models";
import Cookies from 'js-cookie';

var config: any;
var axios: AxiosInstance;

export function Init(configUrl: string) {
  return new Promise((resolve, reject) => {
    Axios.get(configUrl).then(res => {
      config = res.data;
      axios = Axios.create({
        baseURL: config.baseURL,
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
      resolve(config);
    }).catch(err => reject(err));
  });
}

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

export function User_GetAll() { return axios.get<OperationResult<UserGetDto[]>>(`Users`); }
export function User_GetOwn() { return axios.get<OperationResult<UserGetDto>>(`Users/Own`); }
export function User_ChangePassword(id: number, dto: UserChangePasswordDto) { return axios.patch<OperationResult<UserGetDto>>(`Users/${id}/ChangePassword`, dto); }
export function User_ChangeOwnProfile(dto: UserChangeProfileDto) { return axios.patch<OperationResult<UserGetDto>>(`Users/Own/ChangeProfile`, dto); }
export function User_ChangeOwnPassword(dto: UserChangePasswordDto) { return axios.patch<OperationResult<UserGetDto>>(`Users/Own/ChangePassword`, dto); }
export function User_Add(dto: UserAddDto) { return axios.post<OperationResult<UserGetDto>>(`Users`, dto); }
export function User_ResetPassword(id: number, dto: UserResetPasswordDto) { return axios.post<OperationResult<UserGetDto>>(`Users/${id}`, dto); }

export function Image_GetAll() { return axios.get<OperationResult<ImageGetDto[]>>(`Images`); }
export function Image_Get(id: number) { return axios.get<OperationResult<ImageGetDto>>(`Images/${id}`); }
export function Image_Add(dto: ImageAddDto) { return axios.post<OperationResult<ImageGetDto>>(`Images`, dto); }
export function Image_Change(id: number, dto: ImageChangeDto) { return axios.patch<OperationResult<ImageGetDto>>(`Images/${id}`, dto); }
export function Image_Remove(id: number) { return axios.delete<OperationResult<ImageGetDto>>(`Images/${id}`); }
export function Image_GetAllBuildLogs(id: number) { return axios.get<OperationResult<ImageBuildLogDto[]>>(`Images/${id}/BuildLogs`); }
export function Image_GetBuildLogLink(id: number, buildname: string, filename: string) { return `${config.baseURL}/Images/${id}/BuildLogs/${buildname}/${filename}`; }

export function Template_GetAll() { return axios.get<OperationResult<TemplateGetDto[]>>(`Templates`); }
export function Template_Get(id: number) { return axios.get<OperationResult<TemplateGetDto>>(`Templates/${id}`); }
export function Template_Add(dto: TemplateAddDto) { return axios.post<OperationResult<TemplateGetDto>>(`Templates`, dto); }
export function Template_Dupplicate(id: number) { return axios.put<OperationResult<TemplateGetDto>>(`Templates/${id}/Dupplicate`, {}); }
export function Template_Change(id: number, dto: TemplateChangeDto) { return axios.patch<OperationResult<TemplateGetDto>>(`Templates/${id}`, dto); }
export function Template_Remove(id: number) { return axios.delete<OperationResult<TemplateGetDto>>(`Templates/${id}`); }


export function Custome_GetNightlyBuildLogs(branch: string, date: string) { return axios.get<OperationResult<CustomeNightlyBuildLog[]>>(`Custom/NightlyBuildLogs/${branch}/${date}`); }
export function Custome_GetNightlyBuildLogDownloadPath(branch: string, date: string, filename: string) { return config.baseURL + `/Custom/NightlyBuildLogs/${branch}/${date.substr(0, 10)}/${filename}`; }
