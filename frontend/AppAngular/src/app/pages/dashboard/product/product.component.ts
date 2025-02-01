import { AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ProductDto } from '../../../../domain/dto/ProductDto';
import { catchError, delay, finalize, firstValueFrom, Observable, of } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ProductApi } from '../../../../domain/api/ProductApi';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent implements OnInit, AfterViewInit {
  public list$: Observable<ProductDto[]> = new Observable<ProductDto[]>();
  public form: FormGroup;
  public busy = false;
  public isLoading = true;

  dataSource = new MatTableDataSource<ProductDto>();
  displayedColumns = ['actions', 'id', 'name', 'price'];

  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  constructor(
    private api: ProductApi,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    // Inicializando o formulário reativo
    this.form = this.fb.group({
      id: [''],
      name: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
    });
  }

  ngAfterViewInit(): void {
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    } else {
      setTimeout(() => {
        this.dataSource.paginator = this.paginator!;
      });
    }
  }

  async ngOnInit() {
    this.isLoading = true;

    // Carregar a lista inicial de produtos
    await this.loadList();
  }

  /** Limpa o formulário para criação de um novo produto */
  new() {
    this.form.reset();
  }

  /** Salva ou atualiza um produto */
  async save() {
    if (this.form.valid) {
      this.busy = true;

      const product = this.form.value as ProductDto;

      try {
        if (product.id) {
          await this.api.Update(product);
        } else {
          await this.api.Save(product);
        }
        
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

    try {
      const products = await firstValueFrom(await this.api.GetListAll());
      this.dataSource.data = products;
      
      //this.dataSource.data = [...products]; 
      //await this.cdr.detectChanges();

    } catch (error) {
      console.error('Erro ao carregar lista de produtos:', error);
      this.dataSource.data = []; // Se houver erro, limpa a lista
    } finally {
      this.isLoading = false;
    }
  }

  /** Preenche o formulário para atualização */
  async onUpdate(id: number) {
    this.busy = true;

    try {
      const product = await (await this.api.GetById(id)).toPromise();
      if (product) {
        this.form.patchValue(product); // Atualiza o formulário com os dados
      }
    } catch (error) {
      console.error('Erro ao carregar produto para atualização:', error);
    } finally {
      this.busy = false;
    }
  }

  /** Exclui um produto e recarrega a lista */
  async onDelete(id: number) {
    this.isLoading = true;

    try {
      await this.api.Delete(id);
      await this.loadList(); // Recarrega a lista após a exclusão
    } catch (error) {
      console.error('Erro ao deletar o produto:', error);
    } finally {
      this.isLoading = false;
    }
  }
}

