import { Injectable } from '@angular/core';
import { GrowthLogDto, GrowthLogGetDto, UpsertGrowthLogDto } from '../models/growth-log.interface';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GrowthLogService {
  constructor(private http: HttpClient) {}

  //for current user using tokens
  getAllLogs(): Observable<GrowthLogDto[]> {
    return this.http.get<GrowthLogDto[]>(`${environment.apiUrl}/log`);
  }

  getAllLogsForPlanted(plantedId: number): Observable<GrowthLogDto[]> {
    let params = new HttpParams().set('plantedId', plantedId.toString());
    return this.http.get<GrowthLogDto[]>(`${environment.apiUrl}/log/planted`, { params });
  }

  getLog(id: number): Observable<GrowthLogGetDto> {
    return this.http.get<GrowthLogGetDto>(`${environment.apiUrl}/log/${id}`);
  }

  addLog(log: UpsertGrowthLogDto): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/log`, log);
  }
  
  updateLog(id: number, log: UpsertGrowthLogDto): Observable<void>{
    return this.http.put<void>(`${environment.apiUrl}/log/${id}`, log);
  }

  removeLog(id: number): Observable<void>{
    return this.http.delete<void>(`${environment.apiUrl}/log/${id}`);
  }
  
  addImage(id: number, images: string[]): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/log/${id}/images`, images);
  }

  removeImage(logId: number, imageId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/log/${logId}/images/${imageId}`);
  }
}
