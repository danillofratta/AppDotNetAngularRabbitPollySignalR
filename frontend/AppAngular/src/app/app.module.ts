import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OrderApi } from '../domain/api/OrderApi';
import { OrderComponent } from './pages/dashboard/order/order.component';
import { SaleComponent } from './pages/dashboard/sale/sale.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HttpClientModule } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SaleApi } from '../domain/api/SaleApi';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { SignalRSaleService } from '../domain/SignalR/SignalRSaleService';
import { SignalROrderService } from '../domain/SignalR/SignalROrderService ';

@NgModule({
    declarations: [
        AppComponent,

        OrderComponent,
        SaleComponent,
        DashboardComponent,

        LoadingSpinnerComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        FormsModule,
        ReactiveFormsModule,

        HttpClientModule,

      MatIconModule,
      MatButtonModule,
      MatToolbarModule,
      MatSidenavModule,
      MatListModule,
      MatTableModule,
      MatPaginatorModule,
      MatProgressSpinnerModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule,
      BrowserAnimationsModule,
      MatCardModule  
    ],
    providers: [OrderApi, SaleApi, provideAnimationsAsync(),
      SignalRSaleService, SignalROrderService],
    //,
    //{
    //  provide: APP_INITIALIZER,
    //  useFactory: (signalrService: SignalRService) => () => signalrService.onDataListUpdated(),
    //  deps: [SignalRService],
    //  multi: true,
    //}],
    bootstrap: [AppComponent]
})
export class AppModule {
}
