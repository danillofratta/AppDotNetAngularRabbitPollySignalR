import { Injectable } from '@angular/core';
import { API } from './API';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ProductDto } from '../dto/ProductDto';
import { environment } from '../../environments/environment.development';


@Injectable({
    providedIn: 'root'
})
export class ProductApi extends API {
   
  constructor(
    protected override http: HttpClient,    
    protected override router: Router
  ) {
    super(http, router);

    this._baseurl = environment.ApiUrlProduct;

    if (!environment.production)
      this._endpoint = '/api/v1/product/';
    else
      this._endpoint = '/product';
  }
  
  async GetListAll() {   
   return this._http.get<ProductDto[]>(`${this._baseurl + this._endpoint}`);
  }

  async GetById(id: number) {   
    return this._http.get<ProductDto[]>(`${this._baseurl + this._endpoint + '/getbyid/' + id}`);
  }

  async GetByName(name: string) {
    return this._http.get<ProductDto[]>(`${this._baseurl + this._endpoint + '/getbyname/' + name}`);
  }

  async Save(dto: ProductDto) {
   return this._http.post(`${this._baseurl + this._endpoint}`, dto).subscribe();
   }

   async Update(dto: ProductDto) {
    return this._http.put(`${this._baseurl + this._endpoint}`, dto).subscribe();
    }

  async Delete(id: number) {
    return this._http.delete(`${this._baseurl + this._endpoint +'/'+ id}`).subscribe();
  }

}
