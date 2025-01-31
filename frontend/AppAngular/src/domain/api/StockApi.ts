import { Injectable } from '@angular/core';
import { API } from './API';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { StockDto } from '../dto/StockDto';
import { environment } from '../../environments/environment.development';


@Injectable({
    providedIn: 'root'
})
export class StockApi extends API {
  
  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = environment.ApiUrlStock;
    this._endpoint = "/api/v1/stock/";
  }
  
  async GetListAll() {   
   return this._http.get<StockDto[]>(`${this._baseurl + this._endpoint}`);
  }

  async Save(dto: StockDto) {
    return this._http.post(`${this._baseurl + this._endpoint }`, dto).subscribe();
  }  
}
