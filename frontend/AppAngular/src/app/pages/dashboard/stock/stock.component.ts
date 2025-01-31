import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { StockDto } from '../../../../domain/dto/StockDto';
import { Observable, catchError, finalize, of } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { StockApi } from '../../../../domain/api/StockApi';
import { ProductDto } from '../../../../domain/dto/ProductDto';
import { SignalRStockService } from '../../../../domain/SignalR/SignalRStockService';


@Component({
  selector: 'app-stock',
  templateUrl: './stock.component.html',
  styleUrl: './stock.component.css'
})
export class StockComponent implements OnInit, AfterViewInit {
  public list$: Observable<StockDto[]> = new Observable<StockDto[]>();
  public form: FormGroup;
  public busy = false;
  public isLoading = true;
  private productDto: any;

  dataSource = new MatTableDataSource<StockDto>();

  displayedColumns = ['id', 'idproduct', 'nameproduct', 'amount'];

  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  constructor(
    private api: StockApi,
    private fb: FormBuilder,
    private signalRService: SignalRStockService
  ) {
    this.form = this.fb.group({
      amount: ['', [Validators.required]],
      idproduct: ['', Validators.required],
    });
  }

  ngAfterViewInit(): void {
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  async ngOnInit() {
    this.isLoading = true;

    await this.loadList();

    this.signalRService.onGetListStockUpdated((updatedDataList) => {
      console.log('Lista de dados recebida:', updatedDataList);

      const orders$: Observable<StockDto[]> = of(updatedDataList as StockDto[]);

      console.log(orders$);

      this.list$ = orders$;

      this.loadDataSource();
    });
  }

  new() {
    this.form.reset();
  }

  async save() {

    if (!this.productDto) return;

    if (this.form.valid) {
      this.busy = true;

      const stock: StockDto = {
        id: 0,
        idproduct: this.productDto.id,
        nameproduct: '',
        amount: this.form.get('amount')?.value,
        price: 0
      };

      try {
        await this.api.Save(stock);

        this.form.reset();
        //this.form.patchValue({ idproduct: null });
        //this.productDto = null;
        //this.onProductSelected(null);

      } catch (error) {
        console.error('Erro ao salvar o stock:', error);
      } finally {
        this.busy = false;
      }
    }
  }

  async loadList() {
    this.isLoading = true;

    this.list$ = (await this.api.GetListAll()).pipe(
      catchError((error) => {
        console.error('Erro ao carregar lista de stock:', error);
        return of([]); 
      }),
      finalize(() => {
        this.isLoading = false; 
      })
    );

    this.list$.subscribe((stock) => {
      this.dataSource.data = stock;
    });
  }

  onProductSelected(record: ProductDto) {
    this.productDto = record;

    if (record) {
      this.form.controls['idproduct'].setValue(record.id);
    } else {
      this.form.controls['idproduct'].reset();
    }
  }

  loadDataSource() {
    this.list$.subscribe(
      (item) => {
        this.dataSource.data = item;
        this.dataSource._renderChangesSubscription;
      }
    )
  }
}


