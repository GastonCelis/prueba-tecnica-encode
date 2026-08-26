import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CredencialesService } from '../../services/credenciales';
import { Credencial } from '../../models/credencial';

@Component({
  selector: 'app-listado',
  imports: [RouterLink, FormsModule],
  templateUrl: './listado.html',
  styleUrl: './listado.css'
})
export class Listado {
  private readonly service = inject(CredencialesService);

  readonly credenciales = signal<Credencial[]>([]);
  readonly cargando = signal(true);
  readonly error = signal<string | null>(null);
  readonly expandida = signal<string | null>(null);

  busqueda = '';
  categoria = '';

  constructor() {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.service.listar({
      busqueda: this.busqueda || undefined,
      categoria: this.categoria || undefined
    }).subscribe({
      next: datos => {
        this.credenciales.set(datos);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las credenciales.');
        this.cargando.set(false);
      }
    });
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