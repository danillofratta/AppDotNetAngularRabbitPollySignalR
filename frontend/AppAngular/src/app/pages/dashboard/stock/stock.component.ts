import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { StockDto } from '../../../../domain/dto/StockDto';
import { Observable, catchError, finalize, of } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { StockApi } from '../../../../domain/api/StockApi';
import { ProductDto } from '../../../../domain/dto/ProductDto';


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
  /*displayedColumns = ['actions', 'id', 'name', 'price'];*/
  displayedColumns = ['id', 'idproduct', 'nameproduct', 'amount'];

  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  constructor(
    private api: StockApi,
    private fb: FormBuilder
  ) {
    // Inicializando o formulário reativo
    this.form = this.fb.group({
      amount: ['', [Validators.required]],
      idproduct: ['', Validators.required],
      //price: ['', [Validators.required, Validators.min(0)]],
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
        await this.loadList();

        this.form.reset();
        this.productDto = null;
        this.onProductSelected(this.productDto);

        this.form.markAsPristine();
        this.form.markAsUntouched();

      } catch (error) {
        console.error('Erro ao salvar o stock:', error);
      } finally {
        this.busy = false;
      }
    }
  }

  /** Carrega a lista de produtos do servidor */
  async loadList() {
    this.isLoading = true;

    this.list$ = (await this.api.GetListAll()).pipe(
      catchError((error) => {
        console.error('Erro ao carregar lista de stock:', error);
        return of([]); // Retorna uma lista vazia em caso de erro
      }),
      finalize(() => {
        this.isLoading = false; // Finaliza o estado de carregamento
      })
    );

    // Atualiza a tabela de dados
    this.list$.subscribe((stock) => {
      this.dataSource.data = stock;
    });
  }

  onProductSelected(record: ProductDto) {
    this.productDto = record;
    //this.form.controls['idproduct'].setValue(record.id);
    if (record) {
      this.form.controls['idproduct'].setValue(record.id);
    } else {
      this.form.controls['idproduct'].reset();
    }
  }

  /** Preenche o formulário para atualização */
  //async onUpdate(id: number) {
  //  this.busy = true;

  //  try {
  //    const product = await (await this.api.GetById(id)).toPromise();
  //    if (product) {
  //      this.form.patchValue(product); // Atualiza o formulário com os dados
  //    }
  //  } catch (error) {
  //    console.error('Erro ao carregar produto para atualização:', error);
  //  } finally {
  //    this.busy = false;
  //  }
  //}

  /** Exclui um produto e recarrega a lista */
  //async onDelete(id: number) {
  //  this.isLoading = true;

  //  try {
  //    await this.api.Delete(id);
  //    await this.loadList(); // Recarrega a lista após a exclusão
  //  } catch (error) {
  //    console.error('Erro ao deletar o produto:', error);
  //  } finally {
  //    this.isLoading = false;
  //  }
  //}
}


