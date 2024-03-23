import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
    selector: 'app-home',
    
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
    
})
export  class HomeComponent implements OnInit {

  registerMode = false;
  users: any;
  constructor(public http:HttpClient) { }

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
