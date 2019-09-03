import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { UserManagerService } from "src/app/services/user-manager.service";

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['login.component.css']
})
export class LoginComponent {
    email: string;
    password: string;

    constructor(private readonly userManager: UserManagerService, private readonly router: Router) {
        this.email = 'user@mail.com';
        this.password = '123_Asdf';
    }

    async login() {
        if (await this.userManager.login(this.email, this.password)) {
            this.router.navigate(['/']);
        }
    }
}