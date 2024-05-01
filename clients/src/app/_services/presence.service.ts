import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/users';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSoutce = new BehaviorSubject <string[]>([]);
  onlineUsers$ = this.onlineUsersSoutce.asObservable();

  constructor(private toastr: ToastrService,private router:Router) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build(); 


    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSoutce.next([...usernames, username]);
      })
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSoutce.next([...usernames.filter(x => x !== username)]);
      })
    });

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSoutce.next(usernames);
    });

    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a new message! Click to read.')
        .onTap
        .pipe(take(1))
        .subscribe({
          next: () => {
            this.router.navigateByUrl('/members/' + username + '?tab=Messages');
          }
        });
        
    });
     
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }   
  

}
