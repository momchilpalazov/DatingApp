import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/members';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/users';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-details', 
  standalone:true, 
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule,MemberMessagesComponent]
})
export class MemberDetailsComponent implements OnInit,OnDestroy {
  @ViewChild('memberTabs',{static:true}) memberTabs?: TabsetComponent;

  member: Member ={} as Member;
  images:  GalleryItem[] = []; 
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?:User;

  constructor(private  accountService: AccountService, private route:ActivatedRoute,
    private messageService:MessageService,public presenceService:PresenceService){
      this.accountService.curentUser$.pipe(take(1)).subscribe({
        next: user =>
          {
            if(user) this.user=user;
          }           
      
      })
    }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {   

    this.route.data.subscribe({
      next: data => this.member= data ['member'] 
      }
    )

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab']) 
      } 

    })

    this.getImages();  
    
  }

  selectTab(heading:string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(t=>t.heading===heading)!.active=true;
    }
   
  }


  onTabActivated(data: TabDirective){
    this.activeTab=data;
    if(this.activeTab.heading==='Messages' && this.user){
      this.messageService.createHubConnection(this.user,this.member.userName);
    }
    else{
      this.messageService.stopHubConnection();
    }
  }

  loadMessages(){
    if(this.member){
      this.messageService.getMessageThread(this.member.userName).subscribe( {
        next: messages => 
        this.messages = messages
      })
    }
   
  }


  getImages(): GalleryItem[] {
    if (!this.member || !this.member.photos) return []; // Проверка за null или undefined
    for (const photo of this.member.photos) {
        this.images.push(new ImageItem({ src: photo?.url, thumb: photo?.url }));
    }
    return this.images;
}




}
