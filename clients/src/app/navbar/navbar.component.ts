import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],  
  
}) 
export class NavbarComponent implements OnInit{

  model: any = {};
  

  constructor( public accountService: AccountService, private router: Router) { 

  }
  ngOnInit(): void {    
    
  }

  login() {

    this.accountService.login(this.model).subscribe( {

      next:responce=> {
        this.router.navigateByUrl('/members');
        
      } ,
      error: error=> {
        console.log(error);
      }
      
    });  
  
  }

  logout(){

    this.accountService.logout(); 
    this.router.navigateByUrl('/');   
  
  }  


}


