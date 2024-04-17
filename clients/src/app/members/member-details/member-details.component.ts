import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/members';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-details', 
  standalone:true, 
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule]
})
export class MemberDetailsComponent implements OnInit {

  member: Member|undefined;
  images:  GalleryItem[] = []; 

  constructor(private  memberSercice: MembersService, private route:ActivatedRoute){}

  ngOnInit(): void {

    this.loadMember();
    
  }

  loadMember(){
    var username= this.route.snapshot.paramMap.get('username');
    if(!username) return;
    this.memberSercice.getMember(username).subscribe({
      next: member =>{

       this.member=member,
        this.gerImages();

      }})
  }

  gerImages(): any[]{
    if(!this.member) return this.images;
    for (const photo of this.member?.photos) {
      this.images.push(
       new ImageItem({ src: photo?.url, thumb: photo?.url} )
      );
    }
    return this.images;
  }



}
