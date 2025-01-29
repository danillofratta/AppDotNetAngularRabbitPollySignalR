import { Injectable } from '@angular/core';
import { API } from './API';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ProductDto } from '../dto/ProductDto';


@Injectable({
    providedIn: 'root'
})
export class ProductApi extends API {
  
  private _endpoint: string = '/api/v1/product/';
   
  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = "http://localhost:5285";           
  }
  
  async GetListAll() {   
   return this._http.get<ProductDto[]>(`${this._baseurl + this._endpoint}`);
  }

  async GetById(id: number) {   
    return this._http.get<ProductDto[]>(`${this._baseurl + this._endpoint + id}`);
   }

  async Save(dto: ProductDto) {
   return this._http.post(`${this._baseurl + this._endpoint}`, dto).subscribe();
   }

   async Update(dto: ProductDto) {
    return this._http.put(`${this._baseurl + this._endpoint}`, dto).subscribe();
    }

  async Delete(id: number) {
    return this._http.delete(`${this._baseurl + this._endpoint + id}`).subscribe();
  }

}
