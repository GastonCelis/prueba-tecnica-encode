export type Categoria = 'adulto' | 'juvenil' | 'niño';

export interface AltaCredencialRequest {
    nombre: string;
    apellido: string;
    dni: string;
    categoria: Categoria;
    foto: string;
}

export interface AltaCredencialResponse {
    id: string;
    numeroSocio: string;
    validFrom: string;
    validUntil: string;
}

export interface Credencial {
    id: string;
    nombre: string;
    apellido: string;
    dni: string;
    numeroSocio: string;
    categoria: string;
    foto: string;
    validFrom: string;
    validUntil: string;
    credentialStatus: number;
    vc: unknown;
}

export interface FiltroCredenciales {
    busqueda?: string;
    categoria?: string;
    estado?: number;
}