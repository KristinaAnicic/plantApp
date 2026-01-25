import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { GroupedPlantedDto, PlantedDto, PlantedGetDto, PlantedReference, UpsertPlantedDto } from '../models/planted.interface';
import { environment } from '../../environments/environment';
import { PlaceGetDto } from '../models/place.interface';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class PlantedService {
  private http = inject(HttpClient)

  constructor() {}

  private references$ = this.getReferences().pipe(shareReplay(1));
  readonly references = toSignal<PlantedReference | null>(this.references$, { 
    initialValue: null 
  });

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

  getAllPlantedPlantsByPlaceId(placeId: number): Observable<PlaceGetDto> {
    return this.http.get<PlaceGetDto>(`${environment.apiUrl}/planted/place/${placeId}`);
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

  getReferences(): Observable<PlantedReference> {
    return this.http.get<PlantedReference>(`${environment.apiUrl}/planted/references`);
  }
}
