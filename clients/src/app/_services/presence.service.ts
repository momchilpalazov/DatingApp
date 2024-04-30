import { Injectable } from '@angular/core';
import { HubConnection } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/users';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection?:HubConnection;

  constructor(private toastr:ToastrService) { }


  createHubConnection(user:User){

}
