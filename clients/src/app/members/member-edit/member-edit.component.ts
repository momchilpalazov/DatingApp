import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/members';
import { User } from 'src/app/_models/users';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
  
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload',['$event']) unloadNotification($event:any){
    if(this.editForm?.dirty){
      $event.returnValue = true;
    }
  }

  member: Member | undefined;

  user:User | null = null;

  constructor(private accountSrevice:AccountService,private meberService:MembersService,private toastr: ToastrService) { 
    this.accountSrevice.curentUser$.pipe(take(1)).subscribe({next:user=> this.user = user});
  }
  ngOnInit(): void {
    this.loadMember();   
  }

  loadMember(){
    if(this.user == null) return;
    this.meberService.getMember(this.user?.username).subscribe({
      next:member=>this.member = member
    })
  }

  updateMember(){
  
    if(this.member == null) return;
    this.meberService.updateMember(this.editForm?.value).subscribe({
      next:()=>{
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member);
      }
    });
   
  }



}
