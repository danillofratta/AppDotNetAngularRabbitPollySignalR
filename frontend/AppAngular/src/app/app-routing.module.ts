import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderComponent } from './pages/dashboard/order/order.component';
import { SaleComponent } from './pages/dashboard/sale/sale.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';

const routes: Routes =
  [
    {
      path: '',
      //canActivate: [AuthService],
      component: DashboardComponent,      
      children: [
        { path: 'order', component: OrderComponent },
        { path: 'sale', component: SaleComponent }        
      ]
    }
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
