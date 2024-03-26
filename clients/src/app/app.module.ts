import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './navbar/navbar.component';
import { FormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ScharedModule } from './_modules/schared/schared.module';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MessagesComponent } from './messages/messages.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_intercepters/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';



@NgModule({
    declarations: [
        AppComponent,
        NavbarComponent,
        HomeComponent,
        RegisterComponent,
        MemberListComponent,
        MemberDetailsComponent,
        MessagesComponent,
        TestErrorComponent,
        NotFoundComponent,
        ServerErrorComponent
    ],
    providers: [

       {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true } 

    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        FormsModule,
        ScharedModule
       
    ]
})
export class AppModule { }
