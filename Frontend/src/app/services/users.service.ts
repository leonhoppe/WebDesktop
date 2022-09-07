import {Injectable} from '@angular/core';
import {BackendService, RequestTypes} from "./backend.service";
import {User, UserEditor, UserLogin} from "../entitys/user";
import {AccessToken} from "../entitys/accessToken";

@Injectable({
  providedIn: 'root'
})
export class UserApi {
  private user: User;

  constructor(private backend: BackendService) { }

  public async getUsers(): Promise<User[]> {
    const response = await this.backend.sendRequest<User[]>(RequestTypes.GET, "users", undefined, {authorized: true});
    if (!response.success) return [];
    return response.content;
  }

  public async getUser(id: string): Promise<User> {
    const response = await this.backend.sendRequest<User>(RequestTypes.GET, "users/" + id, undefined, {authorized: true});
    return response.content;
  }

  public async editUser(id: string, editor: UserEditor): Promise<boolean> {
    const response = await this.backend.sendRequest<any>(RequestTypes.PUT, "users/" + id, editor, {authorized: true});
    return response.success;
  }

  public async deleteUser(id: string): Promise<boolean> {
    const response = await this.backend.sendRequest(RequestTypes.DELETE, "users/" + id, undefined, {authorized: true});
    return response.success;
  }

  public async getUserPermissions(id: string, excludeGroupPermissions: boolean = false): Promise<string[]> {
    const response = await this.backend.sendRequest<string[]>(RequestTypes.GET, "users/" + id + "/permissions" + (excludeGroupPermissions ? "/raw" : ""), undefined, {authorized: true});
    if (!response.success) return [];
    return response.content;
  }

  public async addUserPermissions(id: string, permissions: string[]): Promise<boolean> {
    const response = await this.backend.sendRequest<any>(RequestTypes.POST, "users/" + id + "/permissions", permissions, {authorized: true});
    return response.success;
  }

  public async removeUserPermissions(id: string, permissions: string[]): Promise<boolean> {
    const response = await this.backend.sendRequest<any>(RequestTypes.PUT, "users/" + id + "/permissions", permissions, {authorized: true});
    return response.success;
  }

  public async login(login: UserLogin): Promise<{success: boolean, errorMessage: string, errorCode: number}> {
    const response = await this.backend.sendRequest<AccessToken>(RequestTypes.PUT, "users/login", login, {withCredentials: true});

    if (response.success) {
      this.backend.setToken(response.content.id);
      await this.getAuthorizedUser();
    }

    return {success: response.success, errorMessage: response.message, errorCode: response.code};
  }

  public async register(register: UserEditor): Promise<{success: boolean, errorMessage: string, errorCode: number}> {
    const response = await this.backend.sendRequest<AccessToken>(RequestTypes.POST, "users/register", register, {withCredentials: true});

    if (response.success) {
      this.backend.setToken(response.content.id);
      await this.getAuthorizedUser();
    }

    return {success: response.success, errorMessage: response.message, errorCode: response.code};
  }

  public async logout(id: string): Promise<boolean> {
    const response = await this.backend.sendRequest(RequestTypes.DELETE, "users/" + id + "/logout", undefined, {authorized: true, withCredentials: true});
    return response.success;
  }

  public async getAuthorizedUser(): Promise<User> {
    if (this.user != undefined) return this.user;

    const response = await this.backend.sendRequest<User>(RequestTypes.GET, "users/self", undefined, {authorized: true});

    if (response.success)
      this.user = response.content;

    return response.content;
  }
}
