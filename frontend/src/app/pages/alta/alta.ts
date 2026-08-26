import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CredencialesService } from '../../services/credenciales';
import { AltaCredencialRequest, AltaCredencialResponse } from '../../models/credencial';

@Component({
  selector: 'app-alta',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './alta.html',
  styleUrl: './alta.css'
})
export class Alta {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(CredencialesService);
  private readonly router = inject(Router);

  readonly enviando = signal(false);
  readonly error = signal<string | null>(null);
  readonly resultado = signal<AltaCredencialResponse | null>(null);

  readonly form = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    apellido: ['', [Validators.required, Validators.maxLength(100)]],
    dni: ['', [Validators.required, Validators.pattern(/^\d{7,9}$/)]],
    categoria: ['', Validators.required],
    foto: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]]
  });

  enviar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando.set(true);
    this.error.set(null);

    this.service.alta(this.form.getRawValue() as AltaCredencialRequest).subscribe({
      next: respuesta => {
        this.resultado.set(respuesta);
        this.enviando.set(false);
      },
      error: err => {
        this.error.set(err.error?.detail ?? 'No se pudo emitir la credencial.');
        this.enviando.set(false);
      }
    });
  }

  volverAlListado(): void {
    this.router.navigate(['/credenciales']);
  }

  formatearFecha(iso: string): string {
    return new Date(iso).toLocaleDateString('es-AR');
  }
}