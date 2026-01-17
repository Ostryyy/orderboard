import { Injectable, computed, signal } from '@angular/core';
import { OrdersApi } from '../../../core/api/orders.api';
import { OrderResponse } from '../../../core/models/orders.models';

@Injectable({ providedIn: 'root' })
export class OrdersStore {
  private _orders = signal<OrderResponse[]>([]);
  private _loading = signal(false);
  private _error = signal<string | null>(null);

  orders = this._orders.asReadonly();
  loading = this._loading.asReadonly();
  error = this._error.asReadonly();

  newOrders = computed(() => this._orders().filter(o => o.status === 'New'));
  preparingOrders = computed(() => this._orders().filter(o => o.status === 'Preparing'));
  readyOrders = computed(() => this._orders().filter(o => o.status === 'Ready'));
  completedOrders = computed(() => this._orders().filter(o => o.status === 'Completed'));

  constructor(private api: OrdersApi) {}

  loadActive(): void {
    this._loading.set(true);
    this._error.set(null);

    this.api.getActive().subscribe({
      next: orders => {
        this._orders.set(orders);
        this._loading.set(false);
      },
      error: err => {
        this._error.set(err?.error?.detail ?? 'Failed to load orders');
        this._loading.set(false);
      }
    });
  }

  create(order: OrderResponse): void {
    this.upsert(order);
  }

  update(order: OrderResponse): void {
    this.upsert(order);
  }

  cancel(order: OrderResponse): void {
    this.upsert(order);
  }

  private upsert(order: OrderResponse): void {
    const current = this._orders();
    const idx = current.findIndex(o => o.id === order.id);
    if (idx === -1) {
      this._orders.set([order, ...current]);
      return;
    }
    const updated = [...current];
    updated[idx] = order;
    this._orders.set(updated);
  }
}