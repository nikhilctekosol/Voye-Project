import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ReportsComponent } from './reports.component';
import { AvailabilityComponent } from './availability/availability.component';
import { SettlementComponent } from './settlement/settlement.component';
import { SheetapprovalComponent } from './sheetapproval/sheetapproval.component';
import { TripcompletionComponent } from './tripcompletion/tripcompletion.component';
import { ReservationDetailsComponent } from './reservation-details/reservation-details.component';

const routes: Routes = [{
  path: '',
  component: ReportsComponent,
  children: [
    {
      path: 'availability',
      component: AvailabilityComponent,
    },
    {
      path: 'settlement',
      component: SettlementComponent,
    },
    {
      path: 'sheetapproval',
      component: SheetapprovalComponent,
    },
    {
      path: 'tripcompletion',
      component: TripcompletionComponent,
    },
    {
      path: 'reservation-detail',
      component: ReservationDetailsComponent,
    }
  ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
export const routedComponents = [
  ReportsComponent,
  AvailabilityComponent,
  SettlementComponent,
  SheetapprovalComponent,
  TripcompletionComponent,
  ReservationDetailsComponent
];
