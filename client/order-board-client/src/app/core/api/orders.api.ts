import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateOrderRequest, OrderResponse, UpdateOrderStatusRequest } from '../models/orders.models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrdersApi {
  private http = inject(HttpClient);

  create(req: CreateOrderRequest): Observable<OrderResponse> {
    return this.http.post<OrderResponse>('/api/orders', req);
  }

  getActive(): Observable<OrderResponse[]> {
    return this.http.get<OrderResponse[]>('/api/orders?active=true');
  }

  updateStatus(id: string, req: UpdateOrderStatusRequest): Observable<OrderResponse> {
    return this.http.patch<OrderResponse>(`/api/orders/${id}/status`, req);
  }

  cancel(id: string): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(`/api/orders/${id}/cancel`, {});
  }
}