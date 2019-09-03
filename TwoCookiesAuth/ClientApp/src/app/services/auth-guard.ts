import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { UserManagerService } from './user-manager.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(
        private readonly userManager: UserManagerService,
        private readonly router: Router) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this.userManager.isAuthenticated) {
            this.router.navigate(['/login']);
            return false;
        }

        return true;
    }
}
