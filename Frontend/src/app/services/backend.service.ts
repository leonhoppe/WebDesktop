import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment";

export interface BackendResponse<T> {
  content: T;
  success: boolean;
  code: number;
  message?: string;
}

export enum RequestTypes {
  GET = 0,
  PUT = 1,
  POST = 2,
  DELETE = 3
}

export interface RequestOptions {
  withCredentials?: boolean;
  authorized?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BackendService {
  public authKey: string;

  public headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': ''
  });

  constructor(private client: HttpClient) {}

  public setToken(token: string): void {
    this.authKey = token;
    this.headers = this.headers.set("Authorization", token);
  }

  public async sendRequest<T>(type: RequestTypes, endpoint: string, body?: any, options?: RequestOptions): Promise<BackendResponse<T>> {
    try {
      let response;

      switch (type) {
        default:
        case RequestTypes.GET:
          response = await firstValueFrom(this.client.get<T>(environment.backendUrl + endpoint, {withCredentials: options?.withCredentials, headers: this.headers}));
          break;

        case RequestTypes.DELETE:
          response = await firstValueFrom(this.client.delete<T>(environment.backendUrl + endpoint, {withCredentials: options?.withCredentials, headers: this.headers}));
          break;

        case RequestTypes.PUT:
          response = await firstValueFrom(this.client.put<T>(environment.backendUrl + endpoint, body, {withCredentials: options?.withCredentials, headers: this.headers}));
          break;

        case RequestTypes.POST:
          response = await firstValueFrom(this.client.post<T>(environment.backendUrl + endpoint, body, {withCredentials: options?.withCredentials, headers: this.headers}));
          break;
      }

      return {content: response, success: true, code: 200};
    } catch (e) {
      const error = e as HttpErrorResponse;

      if (error.status == 401 && options?.authorized) {
        if (await this.requestToken()) {
          options.authorized = false; // Prevent infinite resent loop
          return this.sendRequest<T>(type, endpoint, body, options);
        }
      }

      return {content: undefined, success: false, code: error.status, message: error.error};
    }
  }

  public async testConnection(): Promise<boolean> {
    try {
      await this.client.get(environment.backendUrl);
      return true;
    }catch {
      return false;
    }
  }

  public async requestToken(): Promise<boolean> {
    try {
      const token = await firstValueFrom(this.client.get<{id: string, refreshTokenId: string, expirationDate: string}>(environment.backendUrl + "users/token", {headers: this.headers, withCredentials: true}));
      this.setToken(token.id);
      return true;
    }catch {
      return false;
    }
  }
}
