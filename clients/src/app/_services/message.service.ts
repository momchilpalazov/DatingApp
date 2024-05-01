import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginationHeaders, getPaginationResults } from './paginationHelper';
import { Message } from '../_models/message';
import { BehaviorSubject, take } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/users';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?:HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();


  constructor(private http:HttpClient) { }

  createHubConnection(user: User, otherUserName: string){
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'message?user=' + otherUserName, {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start().catch(error => console.log(error)); 
    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    });  

    this.hubConnection.on('NewMessage', message => {
      this.messageThreadSource.pipe(take(1)).subscribe( {
        next:messages => {
          this.messageThreadSource.next([...messages, message]);
        }
    });

  }
)}





  stopHubConnection(){
    this.hubConnection?.stop().catch(error => console.log(error));
  }

  

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginationResults<Message[]>(this.baseUrl + 'messages', params, this.http);
    
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

   async sendMessage(username: string, content: string){
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
    .catch(error => console.log(error));
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
