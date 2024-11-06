/**
 * @license
 * Copyright Akveo. All Rights Reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 */
import { Component, OnInit } from '@angular/core';
import { AnalyticsService } from './@core/utils/analytics.service';
import { SeoService } from './@core/utils/seo.service';
//import { NotMailServiceService } from './services/not-mail-service.service';
import { interval } from 'rxjs';

@Component({
  selector: 'ngx-app',
  template: '<router-outlet></router-outlet>',
})
export class AppComponent implements OnInit {

  constructor(private analytics: AnalyticsService, private seoService: SeoService) {
  }

  ngOnInit(): void {
    this.analytics.trackPageViews();
    this.seoService.trackCanonicalChanges();

    //const now = new Date();
    //now.setDate(now.getDate() + 1);
    //const next1AM = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 1, 0, 0);
    //const timeUntilNext1AM = next1AM.getTime() - now.getTime();

    //setTimeout(() => {
    //  this.makeHttpRequest();

    //  setInterval(() => {
    //    this.makeHttpRequest();
    //  }, 24 * 60 * 60 * 1000);
    //}, timeUntilNext1AM);

  }

  //makeHttpRequest() {
  //  this.apiService.firstemail().subscribe(
  //    (response) => {
  //      console.log('First notification mail sent on ' + new Date());
  //    },
  //    (error) => {
  //      console.log('Error in first notification mail sent on ' + new Date());
  //    }
  //  );

  //  this.apiService.secondemail().subscribe(
  //    (response) => {
  //      console.log('Second notification mail sent on ' + new Date());
  //    },
  //    (error) => {
  //      console.log('Error in second notification mail sent on ' + new Date());
  //    }
  //  );
  //}
}
