import { Component, OnInit } from "@angular/core";
import { ApiService } from "src/app/services/api.service";
import { UserManagerService } from "src/app/services/user-manager.service";
import { Router } from "@angular/router";

@Component({
    selector: 'profile',
    templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {

    message: string;

    constructor(
        private readonly api: ApiService,
        private readonly userManager: UserManagerService,
        private readonly router: Router) {
        this.message = null;
    }

    ngOnInit() {
        this.api.getMessage().subscribe(msg => this.message = msg);
    }

    async logout(): Promise<void> {
        await this.userManager.logout();
        this.router.navigate(['/login']);
    }
}