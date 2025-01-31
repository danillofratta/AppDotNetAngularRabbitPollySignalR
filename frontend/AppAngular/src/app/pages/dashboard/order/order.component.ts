import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { catchError, finalize, Observable, of } from 'rxjs';
import { OrderDto } from '../../../../domain/dto/OrderDto';
import { OrderApi } from '../../../../domain/api/OrderApi';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { SignalROrderService } from '../../../../domain/SignalR/SignalROrderService ';
import { ProductDto } from '../../../../domain/dto/ProductDto';

@Component({
    selector: 'app-order',    
    templateUrl: './order.component.html',
  styleUrl: './order.component.scss',
  standalone: false
})
export class OrderComponent implements OnInit, AfterViewInit {

  public list$: Observable<OrderDto[]> = new Observable<OrderDto[]>();
  public record: Observable<OrderDto> = new Observable<OrderDto>();

  private productDto: any;
  
  public busy = false;
  isLoading: boolean = true;
  public form: FormGroup;

  receivedMessage: string | undefined;

  dataSource = new MatTableDataSource<OrderDto>();

  displayedColumns = ['id', 'idproduct', 'value', 'nameproduct', 'namestatus'];  

  @ViewChild(MatPaginator) paginator: any = MatPaginator;

  constructor(private Api: OrderApi, private fb: FormBuilder, private signalRService: SignalROrderService) {

    this.form = this.fb.group({
      amount: ['', Validators.compose([
        Validators.required
      ])],
      idproduct: ['']
    });

  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }
  
  async ngOnInit() {
    this.isLoading = true;

    await this.LoadList();

    this.signalRService.onGetListOrderUpdated((updatedDataList) => {
      console.log('Lista de dados recebida:', updatedDataList);

      const orders$: Observable<OrderDto[]> = of(updatedDataList as OrderDto[]);

      console.log(orders$);

      this.list$ = orders$; 

      this.loadDataSource();
    });
  }

  async save() {
    this.busy = true;

    try {


      if (!this.productDto) return;

      if (this.form.valid) {
        const order: Partial<OrderDto> = {
          id: 0,
          idcustomer: 0,
          idproduct: this.productDto.id,
          value: 10,
          createAt: new Date()
        };
        order.amount = this.form.get('amount')?.value;

        await this.Api.Save(order);

        this.form.reset();
        this.productDto = null;
        this.onProductSelected(this.productDto);        
      }
    } finally {
      this.busy = false;
    }

  }

  async LoadList() {
    this.list$ = await (await this.Api.GetListAll()).pipe(
      catchError((error) => {
        //this.error = 'Failed to load data';  // Handle error and display message
        console.error('Error fetching data:', error);
        return [];  // Return empty array to avoid breaking the UI
      }),
      finalize(() => {
        console.log(this.list$);
        this.isLoading = false;  // Set loading flag to false after completion
      })
    );

    //todo set isloading here
    this.loadDataSource();
  }

  loadDataSource() {
    this.list$.subscribe(
      (item) => {
        this.dataSource.data = item;
        this.dataSource._renderChangesSubscription;
      }
    )
  }

  onProductSelected(record: ProductDto) {
    this.productDto = record;

    if (record) {
      this.form.controls['idproduct'].setValue(record.id);
    } else {
      this.form.controls['idproduct'].reset();
    }
  }
}
