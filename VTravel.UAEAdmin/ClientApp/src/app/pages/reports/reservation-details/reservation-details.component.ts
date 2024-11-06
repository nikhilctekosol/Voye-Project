import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NbToastrService, NbComponentStatus, NbDialogContainerComponent, NbDialogService } from '@nebular/theme';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';

@Component({
  selector: 'ngx-reservation-details',
  templateUrl: './reservation-details.component.html',
  styleUrls: ['./reservation-details.component.scss']
})
export class ReservationDetailsComponent implements OnInit {

  token: any;
  loadingSave = false;
  properties: any[];
  options: any[];
  filteredOptions$: Observable<any[]>;
  searchdata = new Searchdata();
  reservationdata: any[] = [];
  selectedPropertyName: string;
  defaultPropertyId: string = "0";
  property = new Property();


  @ViewChild('autoInput') input;
  constructor(private http: HttpClient, private router: Router, private authService: NbAuthService
    , private toastrService: NbToastrService, private dialogService: NbDialogService) {
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      this.searchdata.property = 0;

    });

  }
  ngOnInit(): void {
    let fromdate = new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
    this.searchdata.fromDate = fromdate;
    this.searchdata.toDate = new Date();
    this.loadProperties();
  }

  private loadProperties() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list-sorted-by-name'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;

            this.options = this.properties;
            this.filteredOptions$ = of(this.options);
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-list-sorted-by-name', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  search() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/get-reservation-details?propid=' + this.searchdata.property
      + '&fromdate=' + this.formatDate(this.searchdata.fromDate)
      + '&todate=' + this.formatDate(this.searchdata.toDate)
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {
          
          this.reservationdata = res.data;
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not fetch data!', 'danger');

        }
      },
        error => {

          console.log('api/reports/get-reservation-details', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

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

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

  onPropSelectionChange($event) {
    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    // localStorage["default-reservation-property"] = this.defaultPropertyId;
    // localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);
  }

  getFilteredOptions(value: string): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }
  private filter(value: string): any[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }
  onChange() {
    this.filteredOptions$ = this.getFilteredOptions(this.input.nativeElement.value);
  }


  loadProperty() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
          this.searchdata.property = this.property.id;
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  exportToExcel() {
    var location = 'data:application/vnd.ms-excel;base64,';
    var excelTemplate = '<html> ' +
      '<head> ' +
      '<meta http-equiv="content-type" content="text/plain; charset=UTF-8"/> ' +
      '</head> ' +
      '<body> ' +
      '<table>' +
      document.getElementById("tblData").innerHTML
    '</table>' +
      '</body> ' +
      '</html>';
    window.location.href = location + window.btoa(excelTemplate);
  }

}
class Searchdata {
  property: number | undefined;
  fromDate: Date | undefined;
  toDate: Date | undefined;
  month: string;
}

class Property {

  id: number;
  propertyTypeId: string;
  destinationId: string;
  title: string;
  thumbnail: string;
  address: string;
  city: string;
  propertyStatus: string;
  sortOrder: number;
  shortDescription: string;
  longDescription: string;
  latitude: number;
  longitude: number;
  state: string;
  country: string;
  displayRadius: number;
  maxOccupancy: number;
  roomCount: number;
  bathroomCount: number;
  metaTitle: string;
  metaKeywords: string;
  metaDescription: string;
  phone: string;
  email: string;
  reserveAlert: string;
  reserveAllowed: string;
}


