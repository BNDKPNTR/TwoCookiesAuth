import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { ModuleMapLoaderModule } from '@nguniversal/module-map-ngfactory-loader';
import { AppModule } from './app.module';
import { UserManagerService, ServerUserManagerService } from './services/user-manager.service';
import { AppRootComponent } from './components';
import { AuthTokenInterceptor } from './services/auth-token.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
    imports: [AppModule, ServerModule, ModuleMapLoaderModule],
    providers: [
        { provide: UserManagerService, useClass: ServerUserManagerService },
        { provide: HTTP_INTERCEPTORS, useClass: AuthTokenInterceptor, multi: true }
    ],
    bootstrap: [AppRootComponent]
})
export class AppServerModule { }
