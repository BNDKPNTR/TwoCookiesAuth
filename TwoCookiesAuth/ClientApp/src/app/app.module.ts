import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { UserManagerService, BrowserUserManagerService } from './services/user-manager.service';
import { AppRootComponent, ProfileComponent, LoginComponent } from './components';
import { AuthGuard } from './services/auth-guard';
import { ApiService } from './services/api.service';

@NgModule({
  declarations: [
    AppRootComponent,
    ProfileComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: ProfileComponent, pathMatch: 'full', canActivate: [AuthGuard] },
      { path: 'login', component: LoginComponent }
    ])
  ],
  providers: [
    AuthGuard,
    ApiService,
    { provide: UserManagerService, useClass: BrowserUserManagerService }
  ],
  bootstrap: [AppRootComponent]
})
export class AppModule { }
