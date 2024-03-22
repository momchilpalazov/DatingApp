import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/users';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl= 'https://localhost:5001/api/';
  private currentUsreSource= new BehaviorSubject<User | null>(null);
  curentUser$= this.currentUsreSource.asObservable();

  constructor(private http: HttpClient ) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUsreSource.next(user);
        }
      })
    );   
    
    
  }

  setCurrentUser(user: User){
    this.currentUsreSource.next(user);
  }

  register(model:any) {
    return this.http.post(this.baseUrl + 'account/register', model);
  }
  
  logout() {
    localStorage.removeItem('user');
  }

  getUserData(){
    return this.http.get(this.baseUrl + 'account');
  }
}
