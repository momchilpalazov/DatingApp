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

  member: Member[] = [];
  pagination: Pagination | undefined;
  userParams:UserParams|undefined;
  
  genderList=[{value:'male',display:'Males'},{value:'female',display:'Females'}]

  constructor(private memeberService:MembersService){

    this.userParams=this.memeberService.getUserParams();
   

    }

  ngOnInit(): void {
    // this.members$=this.memeberService.getMembers();
    this.loadMembers();
    
  }

  loadMembers(){
    if(this.userParams)
      {
        this.memeberService.setUserParams(this.userParams);
        this.memeberService.getMembers(this.userParams).subscribe({
          next: response=>{
            if(response.result&&response.pagination){
              this.member=response.result;
              this.pagination=response.pagination;
            }
             
          }         
        })


      }
    
  }

  resetFilters(){
    
      this.userParams= this.memeberService.resetUserParams();
      this.loadMembers();
    
    
  }

  pageChanged(event: any){
    if(this.userParams&& this.userParams?.pageNumber==event.page){
      this.userParams.pageNumber=event.page;
      this.memeberService.setUserParams(this.userParams);
      this.loadMembers();
    }    
  }



}

