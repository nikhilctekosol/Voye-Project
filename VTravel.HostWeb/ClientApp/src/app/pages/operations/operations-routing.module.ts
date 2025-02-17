import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OperationsComponent } from './operations.component';
import { ReservationComponent } from './reservation/reservation.component';
import { InventoryComponent } from './inventory/inventory.component';
import { ReservationnewComponent } from './reservationnew/reservationnew.component';


const routes: Routes = [{
  path: '',
  component: OperationsComponent,
  children: [
    {
      path: 'reservation',
      component: ReservationComponent,
    },
    {
      path: 'reservationnew',
      component: ReservationnewComponent,
    },
    {
      path: 'inventory',
      component: InventoryComponent,
    }, 
    ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OperationsRoutingModule { }

export const routedComponents = [
  OperationsComponent,
  ReservationComponent,
  ReservationnewComponent,
  InventoryComponent
];
