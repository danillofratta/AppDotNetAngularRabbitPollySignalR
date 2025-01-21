import { Injectable } from '@angular/core';
import { API } from './API';
import { OrderDto } from '../dto/OrderDto';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';


@Injectable({
    providedIn: 'root'
})
export class OrderApi extends API {
  
  private _url: string = '/api/v1/order/getall';
  private _urlpost: string = '/api/v1/order/';

  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = "https://localhost:7091";           
  }
  
  async GetListAll() {   
    return this._http.get<OrderDto[]>(`${this._baseurl + this._url}`);
  }

  async Save(data: Partial<OrderDto>) {
    return this._http.post(`${this._baseurl + this._urlpost}`, data).subscribe();
    }

}
