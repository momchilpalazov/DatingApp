import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/users';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];

  user:User= {} as User;

  constructor(private viewContainre:ViewContainerRef,private tempalteRef:TemplateRef<any>,
    private accountService:AccountService) {
      this.accountService.curentUser$.pipe(take(1)).subscribe( {
        next:user=>{
          if(user) this.user=user;
          
        }
        
     }


    )
} 
 ngOnInit(): void {
    if(this.user.roles.some(r=>this.appHasRole.includes(r))){
      this.viewContainre.createEmbeddedView(this.tempalteRef);
      
    }
    else{
      this.viewContainre.clear();
    }
  }


}