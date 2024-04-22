import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/members';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'], // Fix: Change to an array of strings
  
})
export class ListsComponent implements OnInit{
  
  members:Member[] = [];
  
  predicate='liked';

  pageNumber=1;
  pageSize=5;
  pagination:Pagination|undefined;


  constructor(private memeberService:MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
    
  }


  loadLikes(){
   
    this.memeberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next: response=>{
        this.members=response.result as Member[];
        this.pagination=response.pagination;
      }
    })
  }

  pageChanged(event: any){
    if(this. pageNumber==event.page){
      this.pageNumber=event.page;      
      this.loadLikes();
    }    
  }

}