import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AnalyticsDto, PlantedAnalyticsDto } from '../models/analytics.interface';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService {
  http = inject(HttpClient);

  getAnalytics(): Observable<AnalyticsDto> {
    return this.http.get<AnalyticsDto>(`${environment.apiUrl}/analytics`);
  }

  getPlantedAnalytics(plantedId: number): Observable<PlantedAnalyticsDto> {
    return this.http.get<PlantedAnalyticsDto>(`${environment.apiUrl}/analytics/${plantedId}`);
  }
}
