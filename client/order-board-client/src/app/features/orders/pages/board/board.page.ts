import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrderResponse, OrderStatus } from '../../../../core/models/orders.models';
import { OrdersApi } from '../../../../core/api/orders.api';
import { OrdersRealtimeService } from '../../../../core/realtime/orders-realtime.service';
import { OrdersStore } from '../../data-access/orders.store';
import { OrderCardComponent } from '../../ui/order-card/order-card.component';

@Component({
  selector: 'app-board',
  imports: [CommonModule, OrderCardComponent],
  templateUrl: './board.page.html',
  styleUrl: './board.page.scss',
})
export class BoardPage implements OnInit, OnDestroy{
private readonly store = inject(OrdersStore);
  private readonly realtime = inject(OrdersRealtimeService);
  private readonly api = inject(OrdersApi);

  readonly boardId = 'main';

  readonly loading = this.store.loading;
  readonly error = this.store.error;

  readonly newOrders = this.store.newOrders;
  readonly preparingOrders = this.store.preparingOrders;
  readonly readyOrders = this.store.readyOrders;
  readonly completedOrders = this.store.completedOrders;

  async ngOnInit(): Promise<void> {
    this.store.loadActive();

    await this.realtime.connect(this.boardId);

    this.realtime.on('OrderCreated', (o) => this.store.create(o));
    this.realtime.on('OrderUpdated', (o) => this.store.update(o));
    this.realtime.on('OrderCanceled', (o) => this.store.cancel(o));
  }

  ngOnDestroy(): void {
    void this.realtime.disconnect(this.boardId);
  }

  nextStatus(order: OrderResponse): void {
    const next = getNextStatus(order.status);
    if (!next) return;

    this.api.updateStatus(order.id, { status: next }).subscribe({
      error: (err) => console.error(err),
    });
  }

  cancel(order: OrderResponse): void {
    this.api.cancel(order.id).subscribe({
      error: (err) => console.error(err),
    });
  }
}

function getNextStatus(status: OrderStatus): OrderStatus | null {
  switch (status) {
    case 'New': return 'Preparing';
    case 'Preparing': return 'Ready';
    case 'Ready': return 'Completed';
    default: return null;
  }
}
