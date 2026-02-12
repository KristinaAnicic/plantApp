import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PlantGroupDto, PlantGroupGetDto, UpsertPlantGroupDto } from '../models/plant-group.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PlantGroupService {
  private http = inject(HttpClient);
  
  constructor() {}
  
  getAllGroups(): Observable<PlantGroupDto[]> {
    return this.http.get<PlantGroupDto[]>(`${environment.apiUrl}/group`);
  }

  getGroup(id: number): Observable<PlantGroupGetDto> {
    return this.http.get<PlantGroupGetDto>(`${environment.apiUrl}/group/${id}`);
  }

  addGroup(group: UpsertPlantGroupDto): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/group`, group);
  }

  updateGroup(id: number, group: UpsertPlantGroupDto): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/group/${id}`, group);
  }

  removeGroup(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/group/${id}`);
  }

  setMultiplePlantsToGroup(id:number, ids: number[]): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/group/${id}/plants`, ids);
  }

  addPlantToGroup(id:number, plantId: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/group/${id}/plant/${plantId}`, null);
  }

  removePlantFromGroup(plantId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/group/remove-plant/${plantId}`);
  }
}
