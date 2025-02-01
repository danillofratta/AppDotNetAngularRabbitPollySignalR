import { Injectable } from '@angular/core';
import { API } from './API';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SaleDto } from '../dto/SaleDto';
import { environment } from '../../environments/environment.development';


@Injectable({
    providedIn: 'root'
})
export class SaleApi extends API {
  
 
  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = environment.ApiUrlSale;

    if (!environment.production)      
      this._endpoint = "/api/v1/sale/";
    else
      this._endpoint = "/sale";
  }
  
  async GetListAll() {
    return this._http.get<SaleDto[]>(`${this._baseurl + this._endpoint}`);
  }

  async Save(id: number) {
    return this._http.post(`${this._baseurl + this._endpoint }`, id).subscribe();
    }

}
