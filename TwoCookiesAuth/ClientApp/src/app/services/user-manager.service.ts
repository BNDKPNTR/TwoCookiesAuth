import { Injectable, Inject } from "@angular/core";
import * as jwt_decode from 'jwt-decode';
import { ApiService } from "./api.service";

export const BearerToken = 'BEARER_TOKEN';

export abstract class UserManagerService {
    private _isAuthenticated: boolean;

    get isAuthenticated(): boolean {
        if (this._isAuthenticated === undefined) {
            this._isAuthenticated = this.authenticate();
        }

        return this._isAuthenticated;
    }

    constructor(protected readonly api: ApiService) {
        
    }

    async login(email: string, password: string): Promise<boolean> {
        try {
            await this.api.login(email, password);
            this._isAuthenticated = this.authenticate();
            return true;
        } catch (e) {
            this._isAuthenticated = false;
            return false;
        }
    }

    async logout(): Promise<void> {
        try {
            await this.api.logout();
        } catch (e) {

        } finally {
            this._isAuthenticated = false;
        }
    }

    abstract getToken(): string;

    private authenticate(): boolean {
        const token = this.getToken();

        if (token) {
            const jwt = jwt_decode<JwtPayload>(token);

            return new Date(jwt.exp * 1000) > new Date();
        }

        return false;
    }
}

@Injectable()
export class ServerUserManagerService extends UserManagerService {

    constructor(@Inject(BearerToken) private readonly token: string, api: ApiService) {
        super(api);
    }

    getToken(): string {
        return this.token;
    }
}

@Injectable()
export class BrowserUserManagerService extends UserManagerService {

    private static readonly cookieName = 'Identity.Payload';

    constructor(api: ApiService) {
        super(api);
    }

    getToken(): string {
        return this.getCookie(BrowserUserManagerService.cookieName);
    }

    private getCookie(name: string): string {
        const cookies = `; ${document.cookie}`.split(';');
        
        for (const cookie of cookies) {
            const splitCookie = cookie.split('=');

            if (splitCookie[0] === ` ${name}`) {
                return splitCookie[1];
            }
        }
    
        return null;
    }
}

interface JwtPayload {
    exp: number;
}