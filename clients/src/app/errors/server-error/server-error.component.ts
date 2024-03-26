import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit{
refresh() {
throw new Error('Method not implemented.');
}
  error:any;

  constructor(private router:Router) { 

    const navigation = this.router.getCurrentNavigation();
    this.error  = navigation?.extras?.state;



  }
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

}
