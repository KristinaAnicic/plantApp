import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DiseasePredictionResponse } from '../models/disease-prediction-response.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DiseaseService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:8000/predict';

  /*predictDisease(file: File): Observable<DiseasePredictionResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<DiseasePredictionResponse>(this.apiUrl, formData);
  }*/

  predictDisease(file: File): Observable<DiseasePredictionResponse> {
    const formData = new FormData();
    formData.append('image', file);
    return this.http.post<DiseasePredictionResponse>(`${environment.apiUrl}/plant/disease`, formData);
  }
}
