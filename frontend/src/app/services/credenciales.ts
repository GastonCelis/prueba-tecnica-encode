import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AltaCredencialRequest,
  AltaCredencialResponse,
  Credencial,
  FiltroCredenciales
} from '../models/credencial';

@Injectable({ providedIn: 'root' })
export class CredencialesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/credentials`;

  listar(filtro: FiltroCredenciales = {}): Observable<Credencial[]> {
    let params = new HttpParams();

    if (filtro.busqueda) params = params.set('busqueda', filtro.busqueda);
    if (filtro.categoria) params = params.set('categoria', filtro.categoria);
    if (filtro.estado !== undefined) params = params.set('estado', filtro.estado);

    return this.http.get<Credencial[]>(this.url, { params });
  }

  alta(request: AltaCredencialRequest): Observable<AltaCredencialResponse> {
    return this.http.post<AltaCredencialResponse>(this.url, request);
  }
}