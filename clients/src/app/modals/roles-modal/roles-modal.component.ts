import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit{
  username='';
  availibleRoles:any[]=[];
  selectedRoles:any[]=[];

  constructor(public bsModalRef:BsModalRef) { }
  ngOnInit(): void {
   
  }

  updateCheckedRoles(checkedValue:string) {
   const index = this.selectedRoles.indexOf(checkedValue);
    if(index === -1) {
      this.selectedRoles.push(checkedValue);
    } else {
      this.selectedRoles.splice(index,1);
    }

  }

}
