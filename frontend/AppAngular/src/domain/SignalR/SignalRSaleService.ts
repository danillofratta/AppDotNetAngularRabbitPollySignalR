import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRSaleService {
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7276/notificationHub', {
        withCredentials: false, // Ensure credentials are included
      })
      .withAutomaticReconnect()      
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('Error connecting to SignalR: ', err));
  }

  onGetListSaleUpdated(callback: (dataList: any[]) => void): void {
    this.hubConnection.on('GetListSale', callback);
  }

}
