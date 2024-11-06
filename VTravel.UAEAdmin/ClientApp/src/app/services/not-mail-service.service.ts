import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { Router, ActivatedRoute } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NotMailServiceService {

  token: any;
  constructor(private http: HttpClient, private authService: NbAuthService, private router: Router) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();
    });
  }

  firstemail(): Observable<any> {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    return this.http.get('api/reservation/first-email'
      , { headers: headers });
  }

  secondemail(): Observable<any> {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    return this.http.get('api/reservation/second-email'
      , { headers: headers });
  }
}
