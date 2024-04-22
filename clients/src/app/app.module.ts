import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './navbar/navbar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ScharedModule } from './_modules/schared/schared.module';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_intercepters/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { JwtInterceptor } from './_intercepters/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { LoadingInterceptor } from './_intercepters/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { ListsComponent } from './lists/lists.component';



@NgModule({
    declarations: [
        AppComponent,
        NavbarComponent,
        HomeComponent,
        RegisterComponent,
        MemberListComponent,       
        MessagesComponent,
        TestErrorComponent,
        NotFoundComponent,
        ServerErrorComponent,        
        MemberEditComponent,
        PhotoEditorComponent,
        TextInputComponent,
        DatePickerComponent,
        MemberCardComponent,
        ListsComponent,
        
       
    ],
    providers: [

       {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
       {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true } ,
       {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true } ,


    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        FormsModule,
        ReactiveFormsModule,
        ScharedModule
        
        
       
    ]
})
export class AppModule { }
