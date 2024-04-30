import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/users';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{

  users: User[]= [];
  bsModalRef: BsModalRef<RolesModalComponent>= new BsModalRef<RolesModalComponent>();
  avilibleRoles= [
    'Admin',
    'Moderator',
    'Member'
  ]

  constructor(private adminService:AdminService,private modalService:BsModalService) { }
  ngOnInit(): void {
    this.getUsersWithRoles();
   
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    })
  }

  openRolesModal(user:User) {
   const config = {
      class:'modal-dialog-centered',
      initialState:{
        username: user.username,
        availibleRoles: this.avilibleRoles,
        selectedRoles: [...user.roles]
        
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent,config);
    this.bsModalRef.onHidden?.subscribe({ 
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if(selectedRoles && !this.arrayEquals(selectedRoles,user.roles)) {
          this.adminService.updateUserRoles(user.username,selectedRoles.join(',')).subscribe(() => {
            user.roles = [...selectedRoles];
          })  
        
        }
        
      }
    });   
  }

  arrayEquals(a: string[], b: string[]): boolean {
    return Array.isArray(a) && Array.isArray(b) && a.length === b.length && a.every((val, index) => val === b[index]);
  }



}
