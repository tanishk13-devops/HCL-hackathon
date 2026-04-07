import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, shareReplay, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Restaurant, RestaurantRequest } from '../models/restaurant.model';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private apiUrl = `${environment.apiUrl}/restaurants`;
  private restaurantsCache = new Map<string, Observable<Restaurant[]>>();
  private restaurantCache = new Map<number, Observable<Restaurant>>();

  constructor(private http: HttpClient) {}

  getRestaurants(search?: string): Observable<Restaurant[]> {
    const normalizedSearch = (search || '').trim().toLowerCase();

    if (!this.restaurantsCache.has(normalizedSearch)) {
      const query = normalizedSearch ? `?search=${encodeURIComponent(normalizedSearch)}` : '';
      const request$ = this.http.get<Restaurant[]>(`${this.apiUrl}${query}`).pipe(
        shareReplay(1),
        catchError(error => {
          this.restaurantsCache.delete(normalizedSearch);
          return throwError(() => error);
        })
      );

      this.restaurantsCache.set(normalizedSearch, request$);
    }

    return this.restaurantsCache.get(normalizedSearch)!;
  }

  getRestaurant(id: number): Observable<Restaurant> {
    if (!this.restaurantCache.has(id)) {
      const request$ = this.http.get<Restaurant>(`${this.apiUrl}/${id}`).pipe(
        shareReplay(1),
        catchError(error => {
          this.restaurantCache.delete(id);
          return throwError(() => error);
        })
      );

      this.restaurantCache.set(id, request$);
    }

    return this.restaurantCache.get(id)!;
  }

  createRestaurant(request: RestaurantRequest): Observable<Restaurant> {
    return this.http.post<Restaurant>(this.apiUrl, request).pipe(
      tap(() => this.clearCache())
    );
  }

  updateRestaurant(id: number, request: RestaurantRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request).pipe(
      tap(() => this.clearCache())
    );
  }

  deleteRestaurant(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.clearCache())
    );
  }

  private clearCache(): void {
    this.restaurantsCache.clear();
    this.restaurantCache.clear();
  }
}
