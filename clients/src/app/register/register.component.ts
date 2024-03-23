import { Component, EventEmitter, Input, OnInit, Output, input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AccountService } from '../_services/account.service';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [

    BrowserAnimationsModule,
    FormsModule,    
    RegisterComponent,

  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{

  
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private accountService: AccountService    ) { }

  ngOnInit(): void {
    
  }


  register(){    

    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.cancel();
    }, error => {
      console.log(error);
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
