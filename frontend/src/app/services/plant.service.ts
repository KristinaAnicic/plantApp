import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PlantGetDto, PlantListResponse, UpsertPlantDto } from '../models/plant.interface';
import { PlantFilterDto } from '../models/filter.interface';

@Injectable({
  providedIn: 'root',
})
export class PlantService {
  constructor(private http: HttpClient) {};

  getAllPlants(page?: number): Observable<PlantListResponse> {
    let params = new HttpParams();
    if (page && page > 0) {
      params = params.set('page', page.toString());
    }

    return this.http.get<PlantListResponse>(`${environment.apiUrl}/plant`, { params });
  }

  getAllPlantsFiltered(filter: PlantFilterDto, page?: number): Observable<PlantListResponse> {
    let params = new HttpParams();
    if (page && page > 0) {
      params = params.set('page', page.toString());
    }

    return this.http.post<PlantListResponse>(`${environment.apiUrl}/plant/search`, filter, { params });
  }

  getPlant(id: number): Observable<PlantGetDto> {
    return this.http.get<PlantGetDto>(`${environment.apiUrl}/plant/${id}`);
  }

  addPlant(plant: UpsertPlantDto) {
    return this.http.post(`${environment.apiUrl}/plant`, plant);
  }

  updatePlant(id: number, plant: UpsertPlantDto){
    return this.http.put(`${environment.apiUrl}/plant/${id}`, plant);
  }

  addImage(id: number, images: string[]) {
    return this.http.post(`${environment.apiUrl}/plant/${id}/images`, images);
  }

  removeImage(plantId: number, imageId: number) {
    return this.http.delete(`${environment.apiUrl}/plant/${plantId}/images/${imageId}`);
  }
}
