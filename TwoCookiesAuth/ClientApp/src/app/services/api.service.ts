import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';

@Injectable()
export class ApiService {

    private readonly apiEndpoint: string;

    constructor(private readonly http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.apiEndpoint = baseUrl.endsWith('/') ? `${baseUrl}api` : `${baseUrl}/api`;
    }

    login(email: string, password: string): Promise<void> {
        return this.http.post<void>(`${this.apiEndpoint}/User/Login`, { email: email, password: password }).toPromise();
    }

    logout(): Promise<void> {
        return this.http.post<void>(`${this.apiEndpoint}/User/Logout`, null).toPromise();
    }

    getMessage(): Observable<string> {
        return this.http.get<{ message: string }>(`${this.apiEndpoint}/User`).pipe(
            map(x => x.message)
        );
    }
}