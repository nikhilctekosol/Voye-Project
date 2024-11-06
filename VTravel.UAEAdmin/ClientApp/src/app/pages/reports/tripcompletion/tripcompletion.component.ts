import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NbToastrService, NbComponentStatus } from '@nebular/theme';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NbAuthService, NbAuthToken } from '@nebular/auth';

@Component({
  selector: 'ngx-tripcompletion',
  templateUrl: './tripcompletion.component.html',
  styleUrls: ['./tripcompletion.component.scss']
})
export class TripcompletionComponent implements OnInit {

  token: any;
  completionlist: any[];
  searchdata = new Searchdata();
  constructor(private http: HttpClient, private router: Router, private authService: NbAuthService
    , private toastrService: NbToastrService) {
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();      

    });
  }

  ngOnInit(): void {

    let fromdate = new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
    this.searchdata.fromDate = fromdate;
    this.searchdata.toDate = new Date();

    this.loadtripcompletion();
  }


  formatDate(date) {
    var d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
  }

  loadtripcompletion() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.get('api/reports/tripcompletion?from=' + this.formatDate(this.searchdata.fromDate) + '&&to=' + this.formatDate(this.searchdata.toDate)
      , { headers: headers }).subscribe((res: any) => {        

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.completionlist = res.data;

          }
        }
      },
      error => {
        console.log('api/reports/tripcompletion', error)
        if (error.status === 401) {
          this.router.navigate(['auth/login']);
        }
      });
  }

  enablepermission(id) {
    alert(id);
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/enable-complete-permission?id=' + id 
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.toast('Success', 'Permission enabled successfully!', 'success');
          this.loadtripcompletion();
        }
        else {

          this.toast('Error', 'Something went wrong!', 'danger');
        }
      },
      error => {
        console.log('api/reports/enable-complete-permission', error)
        if (error.status === 401) {
          this.router.navigate(['auth/login']);
        }
      });

  }

  completereservation(id, mode) {
    if (mode == 1) {
      this.router.navigate(['/pages/operations/reservation'], { queryParams: { id: id } });
    }
    else {
      this.router.navigate(['/pages/operations/newreservation'], { queryParams: { id: id } });
    }
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}
class Searchdata {
  fromDate: Date | undefined;
  toDate: Date | undefined;
}
