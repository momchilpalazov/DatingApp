import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/members';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { User } from 'src/app/_models/users';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list', 
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  // members$: Observable<Member[]>|undefined;

  members: Member[] = [];

  pagination: Pagination|undefined;
  userParams:UserParams|undefined;
  user: User|undefined;

  constructor(private memeberService:MembersService,private accountService:AccountService){
    this.accountService.curentUser$.pipe(take(1)).subscribe({
      next:user=>{
        if(user){
          this.userParams=new UserParams(user);
          this.user=user;
      }
    }
    })

    }

  ngOnInit(): void {
    // this.members$=this.memeberService.getMembers();
    this.loadMembers();
    
  }

  loadMembers(){
    if(!this.userParams)return;
     this.memeberService.getMembers(this.userParams).subscribe({
      next: response=>{
        if(response.result&&response.pagination){
          this.members=response.result;
          this.pagination=response.pagination;
        }
         
      }
     
    })
  }

  pageChanged(event: any){
    if(this.userParams&& this.userParams?.pageNumber==event.page){
      this.userParams.pageNumber=event.page;
      this.loadMembers();
    }    
  }



}

