import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { PlaceDto, PlaceGetDto, UpsertPlaceDto } from '../models/place.interface';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';
import { Reference } from '../models/reference.interface';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class PlaceService {
  private http = inject(HttpClient);
  
  constructor() {}
  
  getAllPlaces(): Observable<PlaceDto[]> {
    return this.http.get<PlaceDto[]>(`${environment.apiUrl}/place`);
  }

  getPlace(id: number): Observable<PlaceGetDto> {
    return this.http.get<PlaceGetDto>(`${environment.apiUrl}/place/${id}`);
  }

  addPlace(place: UpsertPlaceDto): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/place`, place);
  }

  updatePlace(id: number, place: UpsertPlaceDto): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/place/${id}`, place);
  }

  removePlace(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/place/${id}`);
  }

  getCountries(): Observable<Reference[]> {
    return this.http.get<Reference[]>(`${environment.apiUrl}/place/country`);
  }

  private countries$ = this.getCountries().pipe(
    shareReplay(1) 
  );
  readonly countries = toSignal(this.countries$, { initialValue: [] as Reference[] });
}
