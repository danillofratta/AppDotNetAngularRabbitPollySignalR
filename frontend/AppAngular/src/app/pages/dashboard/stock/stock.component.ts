import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { StockDto } from '../../../../domain/dto/StockDto';
import { Observable, catchError, finalize, of } from 'rxjs';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { StockApi } from '../../../../domain/api/StockApi';
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

  dataSource = new MatTableDataSource<StockDto>();
  /*displayedColumns = ['actions', 'id', 'name', 'price'];*/
  displayedColumns = ['id', 'idproduct', 'nameproduct', 'amount'];

  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  constructor(
    private api: StockApi,
    private fb: FormBuilder,
    private signalRService: SignalRStockService
  ) {
    // Inicializando o formulário reativo
    this.form = this.fb.group({
      id: ['']
      //name: ['', Validators.required],
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

    // Carregar a lista inicial de produtos
    await this.loadList();

    // Configuração do SignalR para atualizações em tempo real (descomentado caso necessário)
    // this.signalRService.onGetListOrderUpdated((updatedDataList) => {
    //   this.dataSource.data = updatedDataList as ProductDto[];
    // });
  }

  /** Limpa o formulário para criação de um novo produto */
  new() {
    this.form.reset();
  }

  /** Salva ou atualiza um produto */
  async save() {
    if (this.form.valid) {
      this.busy = true;

      const product = this.form.value as StockDto;

      try {
        if (product.id) {
          // Atualiza o produto existente
          //await this.api.Update(product);
        } else {
          // Salva um novo produto
          await this.api.Save(product);
        }

        // Após salvar, recarregar a lista e limpar o formulário
        await this.loadList();
        this.form.reset();
      } catch (error) {
        console.error('Erro ao salvar o produto:', error);
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
        console.error('Erro ao carregar lista de produtos:', error);
        return of([]); // Retorna uma lista vazia em caso de erro
      }),
      finalize(() => {
        this.isLoading = false; // Finaliza o estado de carregamento
      })
    );

    // Atualiza a tabela de dados
    this.list$.subscribe((products) => {
      this.dataSource.data = products;
    });
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


