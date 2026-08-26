import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, switchMap, startWith, delay } from 'rxjs';
import { CredencialesService } from '../../services/credenciales';
import { Credencial } from '../../models/credencial';
import { Alta } from '../alta/alta';

@Component({
  selector: 'app-listado',
  imports: [FormsModule, Alta],
  templateUrl: './listado.html',
  styleUrl: './listado.css'
})
export class Listado {
  private static readonly DEMORA_MINIMA = 300;

  private readonly service = inject(CredencialesService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly filtrosCambiaron = new Subject<void>();

  readonly credenciales = signal<Credencial[]>([]);
  readonly cargando = signal(true);
  readonly cargaInicial = signal(true);
  readonly error = signal<string | null>(null);
  readonly expandida = signal<string | null>(null);
  readonly modalAbierto = signal(false);

  busqueda = '';
  categoria = '';

  constructor() {
    this.filtrosCambiaron
      .pipe(
        startWith(undefined),
        debounceTime(350),
        switchMap(() => {
          this.cargando.set(true);
          this.error.set(null);
          return this.service
            .listar({
              busqueda: this.busqueda.trim() || undefined,
              categoria: this.categoria || undefined
            })
            .pipe(delay(Listado.DEMORA_MINIMA));
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: datos => {
          this.credenciales.set(datos);
          this.cargando.set(false);
          this.cargaInicial.set(false);
        },
        error: () => {
          this.error.set('No se pudieron cargar las credenciales.');
          this.cargando.set(false);
          this.cargaInicial.set(false);
        }
      });
  }

  cargar(): void {
    this.filtrosCambiaron.next();
  }

  abrirModal(): void {
    this.modalAbierto.set(true);
  }

  cerrarModal(huboAlta: boolean): void {
    this.modalAbierto.set(false);
    if (huboAlta) this.cargar();
  }

  alternarDetalle(id: string): void {
    this.expandida.set(this.expandida() === id ? null : id);
  }

  formatearFecha(iso: string): string {
    return new Date(iso).toLocaleDateString('es-AR');
  }

  formatoVc(vc: unknown): string {
    return JSON.stringify(vc, null, 2);
  }
}