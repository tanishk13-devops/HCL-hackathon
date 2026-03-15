import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from '../models/order.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient) {}

  placeOrder(addressId: number, paymentMethod: string): Observable<Order> {
    return this.http.post<Order>(this.apiUrl, { addressId, paymentMethod });
  }

  getOrders(): Observable<Order[]> {
    return this.getMyOrders();
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/my-history`);
  }

  getAllOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`);
  }

  updateOrderStatus(id: number, status: string): Observable<Order> {
    return this.http.patch<Order>(`${this.apiUrl}/${id}/status`, { status });
  }

  trackOrder(id: number): Observable<{ id: number; status: string; workflow: string[]; createdAt: string }> {
    return this.http.get<{ id: number; status: string; workflow: string[]; createdAt: string }>(`${this.apiUrl}/${id}/track`);
  }
}
