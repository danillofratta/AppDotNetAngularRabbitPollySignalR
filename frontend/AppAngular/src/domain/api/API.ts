import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class API {

  protected _baseurl = "";

  protected _http: HttpClient;
  protected _router: Router

  constructor(
    protected http: HttpClient,
    protected router: Router
  ) {
    this._http = http;
    this._router = router;
   
    //this._baseurl = environment.ApiUrl;
    //console.log(environment.production);    
  }

  // public composeHeadersToken() {
  //   //const token = localStorage.getItem('petshop.token');
  //   const token = new SecurityService(this._cookieservice, this.router).GetToken();
  //   const headers = new HttpHeaders().set('Authorization', `bearer ${token}`);

  //   //const headers = new HttpHeaders()
  //   //  .set('Authorization', "bearer " + token);      
                      
  //   return headers;
  // }
}
