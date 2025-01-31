import { Injectable } from '@angular/core';
import { API } from './API';
import { OrderDto } from '../dto/OrderDto';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment.development';


@Injectable({
    providedIn: 'root'
})
export class OrderApi extends API {

  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = environment.ApiUrlOrder;
    this._endpoint = "/api/v1/order/";
  }
  
  async GetListAll() {
    return this._http.get<OrderDto[]>(`${this._baseurl + this._endpoint + 'getall'}`);
  }

  async Save(data: Partial<OrderDto>) {
    return this._http.post(`${this._baseurl + this._endpoint}`, data).subscribe();
    }

}
