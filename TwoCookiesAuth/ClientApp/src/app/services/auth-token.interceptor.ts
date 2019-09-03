import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Observable } from "rxjs";
import { UserManagerService } from "./user-manager.service";

@Injectable()
export class AuthTokenInterceptor implements HttpInterceptor {
    constructor(private readonly userManager: UserManagerService) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!this.userManager.isAuthenticated) {
            return next.handle(req);
        }

        const token = this.userManager.getToken();

        return next.handle(req.clone({
            setHeaders: {
                'Authorization': `Bearer ${token}`
            }
        }));
    }
}