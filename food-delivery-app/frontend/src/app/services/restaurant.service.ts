import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Restaurant, RestaurantRequest } from '../models/restaurant.model';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private apiUrl = `${environment.apiUrl}/restaurants`;

  constructor(private http: HttpClient) {}

  getRestaurants(search?: string): Observable<Restaurant[]> {
    const query = search ? `?search=${encodeURIComponent(search)}` : '';
    return this.http.get<Restaurant[]>(`${this.apiUrl}${query}`);
  }

  getRestaurant(id: number): Observable<Restaurant> {
    return this.http.get<Restaurant>(`${this.apiUrl}/${id}`);
  }

  createRestaurant(request: RestaurantRequest): Observable<Restaurant> {
    return this.http.post<Restaurant>(this.apiUrl, request);
  }

  updateRestaurant(id: number, request: RestaurantRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteRestaurant(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
