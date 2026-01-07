import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GroupedPlantedDto, PlantedDto, PlantedGetDto, UpsertPlantedDto } from '../models/planted.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PlantedService {
  constructor(private http: HttpClient) {}

  getAllPlantedPlants(userId?: number): Observable<PlantedDto[]> {
    let params = new HttpParams();
    if (userId !== undefined){
      params = params.set('userId', userId.toString());   
    } 

    return this.http.get<PlantedDto[]>(`${environment.apiUrl}/planted`, { params });
  }

  getAllPlantedPlantsGroupedByPlace(userId?: number): Observable<GroupedPlantedDto[]> {
    let params = new HttpParams();
    if (userId !== undefined){
      params = params.set('userId', userId.toString());   
    } 

    return this.http.get<GroupedPlantedDto[]>(`${environment.apiUrl}/planted/grouped`, { params });
  }

  getPlanted(id: number): Observable<PlantedGetDto> {
    return this.http.get<PlantedGetDto>(`${environment.apiUrl}/planted/${id}`);
  }

  addPlanted(planted: UpsertPlantedDto) {
    return this.http.post(`${environment.apiUrl}/planted`, planted);
  }

  updatePlanted(id: number, planted: UpsertPlantedDto){
    return this.http.put(`${environment.apiUrl}/planted/${id}`, planted);
  }

  removePlanted(id: number){
    return this.http.delete(`${environment.apiUrl}/planted/${id}`);
  }

  addImage(id: number, images: string[]) {
    return this.http.post(`${environment.apiUrl}/planted/${id}/images`, images);
  }

  removeImage(plantedId: number, imageId: number) {
    return this.http.delete(`${environment.apiUrl}/planted/${plantedId}/images/${imageId}`);
  }
}
