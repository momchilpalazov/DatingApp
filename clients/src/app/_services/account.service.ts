import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl= 'https://localhost:5001/api/';

  constructor(private http: HttpClient ) { }

  login(model: any){
    return this.http.post(this.baseUrl + 'account/login', model);
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
