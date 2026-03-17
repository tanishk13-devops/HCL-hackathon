import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { Restaurant, RestaurantRequest } from '../models/restaurant.model';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private apiUrl = `${environment.apiUrl}/restaurants`;
  private readonly fallbackRestaurants: Restaurant[] = [
    {
      id: 1,
      name: 'Spice Route',
      description: 'North Indian favorites, biryanis and tandoor specials.',
      location: 'Noida Sector 18',
      rating: 4.4,
      imageUrl: 'https://images.unsplash.com/photo-1552566626-52f8b828add9?q=80&w=1200'
    },
    {
      id: 2,
      name: 'Coastal Bowl',
      description: 'South Indian and coastal comfort food.',
      location: 'Gurgaon CyberHub',
      rating: 4.3,
      imageUrl: 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?q=80&w=1200'
    }
  ];

  constructor(private http: HttpClient) {}

  getRestaurants(search?: string): Observable<Restaurant[]> {
    const query = search ? `?search=${encodeURIComponent(search)}` : '';
    return this.http.get<Restaurant[]>(`${this.apiUrl}${query}`).pipe(
      catchError(() => {
        const filtered = search
          ? this.fallbackRestaurants.filter(r =>
              r.name.toLowerCase().includes(search.toLowerCase()) ||
              r.location.toLowerCase().includes(search.toLowerCase()))
          : this.fallbackRestaurants;
        return of(filtered);
      })
    );
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
