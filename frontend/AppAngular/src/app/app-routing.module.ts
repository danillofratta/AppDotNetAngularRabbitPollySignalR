import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderComponent } from './pages/dashboard/order/order.component';
import { SaleComponent } from './pages/dashboard/sale/sale.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ProductComponent } from './pages/dashboard/product/product.component';
import { StockComponent } from './pages/dashboard/stock/stock.component';

const routes: Routes =
  [
    {
      path: '',
      //canActivate: [AuthService],
      component: DashboardComponent,      
      children: [
        { path: 'order', component: OrderComponent },
        { path: 'sale', component: SaleComponent },
        { path: 'product', component: ProductComponent },
        { path: 'stock', component: StockComponent },
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' } // Rota padrão
      ]
    }
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
