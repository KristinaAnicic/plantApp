import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PlantExchangeDto, PlantExchangeGetDto, PlantExchangeResponse, UpsertPlantExchangeDto } from '../models/plant-exchange.interface';
import { environment } from '../../environments/environment';
import { PlantExchangeFilterDto } from '../models/filter.interface';

@Injectable({
  providedIn: 'root',
})
export class PlantExchangeService {
  constructor(private http: HttpClient) {}

  getAllActivePlantExchanges(page?: number): Observable<PlantExchangeResponse> {
    let params = new HttpParams();
    if (page && page > 0) {
      params = params.set('page', page.toString());
    }

    return this.http.get<PlantExchangeResponse>(`${environment.apiUrl}/exchange`, { params });
  }

  getAllPlantExchangesFiltered(filter: PlantExchangeFilterDto, page?: number): Observable<PlantExchangeResponse> {
    let params = new HttpParams();
    if (page && page > 0) {
      params = params.set('page', page.toString());
    }

    return this.http.post<PlantExchangeResponse>(`${environment.apiUrl}/exchange/search`, filter, { params });
  }
  
  getPlantExchange(id: number): Observable<PlantExchangeGetDto> {
    return this.http.get<PlantExchangeGetDto>(`${environment.apiUrl}/exchange/${id}`);
  }

  addPlantExchange(exchange: UpsertPlantExchangeDto): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/exchange`, exchange);
  }

  updatePlantExchange(id: number, exchange: UpsertPlantExchangeDto): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/exchange/${id}`, exchange);
  }

  removePlantExchange(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/exchange/${id}`);
  }

  addImage(id: number, images: string[]): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/exchange/${id}/images`, images);
  }

  removeImage(exchangeId: number, imageId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/exchange/${exchangeId}/images/${imageId}`);
  }
}
