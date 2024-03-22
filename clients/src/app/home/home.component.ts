import { Component, OnInit } from '@angular/core';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@Component({
  selector: 'app-home', 
  standalone: true,
  imports: [
  
    BrowserAnimationsModule,
    FormsModule,    
    HomeComponent,
    



  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  providers: [BsDropdownModule, FormsModule, BrowserAnimationsModule],
  exportAs: 'HomeComponent',

})
export class HomeComponent implements OnInit {

  registerMode = false;
  constructor() { }

  ngOnInit(): void {
    
  }
  
  registerToggle(){
    this.registerMode = !this.registerMode;
  }
}
