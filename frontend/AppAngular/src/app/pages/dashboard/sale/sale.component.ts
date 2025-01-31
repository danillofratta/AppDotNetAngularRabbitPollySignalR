import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Observable, catchError, finalize, of } from 'rxjs';
import { SaleApi } from '../../../../domain/api/SaleApi';
import { SaleDto } from '../../../../domain/dto/SaleDto';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { SignalRSaleService } from '../../../../domain/signalr/SignalRSaleService';


@Component({
    selector: 'app-sale',    
    templateUrl: './sale.component.html',
    styleUrl: './sale.component.scss'
})
export class SaleComponent implements OnInit, AfterViewInit {

  public list$: Observable<SaleDto[]> = new Observable<SaleDto[]>();
  public record: Observable<SaleDto> = new Observable<SaleDto>();

  public busy = false;
  isLoading: boolean = true;
  public form: FormGroup;

  dataSource = new MatTableDataSource<SaleDto>();

  //displayedColumns = ['actions', 'id', 'namestatus', 'nameproduct'];
  displayedColumns = ['actions','id', 'idproduct', 'idorder','value', 'nameproduct', 'namestatus'];

  @ViewChild(MatPaginator) paginator: any = MatPaginator;
    

  constructor(private Api: SaleApi, private fb: FormBuilder, private signalRService: SignalRSaleService) {

    this.form = this.fb.group({
      idsale: ['', Validators.compose([
        Validators.required
      ])]
    });

  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  async ngOnInit() {
    this.isLoading = true;

    await this.LoadList();

    //super.ngOnInit();
    this.signalRService.onGetListSaleUpdated((updatedDataList) => {
      console.log('Lista de dados recebida:', updatedDataList);

      const orders$: Observable<SaleDto[]> = of(updatedDataList as SaleDto[]);

      console.log(orders$);


      this.list$ = orders$; // Atualize a lista de dados na tela

      this.loadDataSource();
    });
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

  async onPayment(id: number)
  {
    this.busy = true;

      await this.Api.Save(id);

      this.form.controls['idsale'].setValue('');

      this.busy = false;
    
  }

  async save() {
  this.busy = true;
  if (this.form.valid) {
    const sale: Partial<SaleDto> = { id: 0, idcustomer: 0, idproduct: 1, value: 10, createAt: new Date() };
    //sale.id = this.form.get('idsale')?.value;

    await this.Api.Save(this.form.get('idsale')?.value);

    this.form.controls['idsale'].setValue('');

    this.busy = false;
  }

}

}
