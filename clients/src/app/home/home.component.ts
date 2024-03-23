import { Component, OnInit } from '@angular/core';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    providers: [BsDropdownModule, FormsModule, BrowserAnimationsModule],
    exportAs: 'HomeComponent',
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HomeComponent,
        RegisterComponent
    ]
})
export class HomeComponent implements OnInit {

  registerMode = false;
  users: any;
  constructor(private http:HttpClient) { }

  ngOnInit(): void {

    this.getusers();
    
  }


  
  registerToggle(){
    this.registerMode = !this.registerMode;
  }

  getusers(){
    this.http.get('https://localhost:5001/api/users').subscribe( {
      next: response => {
        this.users = response;
      },
      error: error => {
        console.log(error);
      },
      complete: () => {
        console.log('Request has completed');
      }
    });
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }
}
