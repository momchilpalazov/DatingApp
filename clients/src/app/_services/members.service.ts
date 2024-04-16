import { HttpClient, HttpHeaders, HttpParams, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/members';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
 

  constructor(private http:HttpClient) { }

  getMembers(userParams:UserParams){
    let params = this.getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params= params.append('gender',userParams.gender);
    
    return this.getPaginationResults<Member[]>(this.baseUrl+'users',params)
  }

  private getPaginationResults<T>(usrl:string ,params: HttpParams) {
    const paginatedResult:PaginatedResult<T>=new PaginatedResult<T>;
    return this.http.get<T>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;

        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    
    return params;
  }

  getMember(username:string){
    const member=this.members.find(x=>x.userName==username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+'users/'+ username)
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl+'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        this.members[index]={...this.members[index],...member};
      })
    )
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  
}
