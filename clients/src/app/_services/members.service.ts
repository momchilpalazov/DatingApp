import { HttpClient, HttpHeaders, HttpParams, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/members';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { User } from '../_models/users';
import { AccountService } from './account.service';
import { getPaginationHeaders, getPaginationResults } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
  memberCache=new Map();
  user:User|undefined;
  userParams:UserParams|undefined;

  

 

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.curentUser$.pipe(take(1)).subscribe(user => {
      if (user) {
        this.user = user;
        this.userParams = new UserParams(user);
      }
    });
  }
  
  

  getUserParams(){
    return this.userParams; 
  }

    setUserParams(params:UserParams){
      this.userParams=params;
    }

    resetUserParams(){
      if(this.user){
        this.userParams=new UserParams(this.user);
        return this.userParams;
      }
      ;

      return;
    }

  getMembers(userParams:UserParams){
    var response=this.memberCache.get(Object.values(userParams).join('-'));

    if(response){
      return of(response);
    }
    let params = getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params= params.append('gender',userParams.gender.toString());
    params= params.append('orderBy',userParams.orderBy.toString());
    
    return getPaginationResults<Member[]>(this.baseUrl+'users',params,this.http).pipe
    (map(response=>{
      this.memberCache.set(Object.values(userParams).join('-'),response);
      return response;
    }));
  }

 

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);

    if (member) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  addLike(username:string){
    return this.http.post(this.baseUrl+'likes/'+username,{});
  }

  getLikes(predicate:string,pageNumber:number,pageSize:number){
    let params = getPaginationHeaders(pageNumber,pageSize);
    params = params.append('predicate',predicate);
    return getPaginationResults<Partial<Member[]>>(this.baseUrl+'likes',params,this.http);
  }

  
}
