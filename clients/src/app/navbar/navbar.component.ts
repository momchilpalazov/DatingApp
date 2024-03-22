import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  
  
}) 
export class NavbarComponent implements OnInit{


  model: any = {};
  loggedIn= false;

  constructor( private accountService: AccountService ) { 

  }
  ngOnInit(): void {
    this.getCurrentUser();
    
  }


  getCurrentUser(){
    this.accountService.curentUser$.subscribe({
        next:user=> this.loggedIn= !!user,
        error:error=> console.log(error)
        });
  }



  login() {

    this.accountService.login(this.model).subscribe( {

      next:responce=> {
        console.log(responce);
        this.loggedIn= true;
      } ,
      error: error=> {
        console.log(error);
      }
      
    });  
  
  }

  logout(){

    this.accountService.logout();
    this.loggedIn=false;
  }


  


}


