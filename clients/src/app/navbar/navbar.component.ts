import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/users';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  
  
}) 
export class NavbarComponent implements OnInit{

  model: any = {};
  

  constructor( public accountService: AccountService ) { 

  }
  ngOnInit(): void {
    
    
  }


  login() {

    this.accountService.login(this.model).subscribe( {

      next:responce=> {
        console.log(responce);
        
      } ,
      error: error=> {
        console.log(error);
      }
      
    });  
  
  }

  logout(){

    this.accountService.logout();
  
  }


  


}


