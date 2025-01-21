import { Injectable } from '@angular/core';
import { API } from './API';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SaleDto } from '../dto/SaleDto';


@Injectable({
    providedIn: 'root'
})
export class SaleApi extends API {
  
  private _urlpost: string = '/api/v1/sale/PaymentOK';
  private _urlget: string = '/api/v1/sale/GetAll';

  

  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = "https://localhost:7276";           
  }
  
  async GetListAll() {   
    return this._http.get<SaleDto[]>(`${this._baseurl + this._urlget}`);
  }

  async Save(id: number) {
    return this._http.post(`${this._baseurl + this._urlpost}`, id).subscribe();
    }

}
