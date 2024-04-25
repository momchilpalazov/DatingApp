import { ResolveFn } from '@angular/router';
import { Member } from '../_models/members';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';



export const memberDetailedResolver: ResolveFn<Member> = (route) => {
 const memberService=inject(MembersService);
 
  return memberService.getMember(route.params['username']);

};
