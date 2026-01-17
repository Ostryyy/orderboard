import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { OrderResponse } from '../models/orders.models';

export type OrderEventType = 'OrderCreated' | 'OrderUpdated' | 'OrderCanceled';

@Injectable({ providedIn: 'root' })
export class OrdersRealtimeService {
  private connection?: signalR.HubConnection;

  async connect(boardId: string): Promise<void> {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/orders')
      .withAutomaticReconnect()
      .build();

    await this.connection.start();
    await this.connection.invoke('JoinBoard', boardId);
  }

  on(event: OrderEventType, handler: (order: OrderResponse) => void): void {
    this.connection?.on(event, handler);
  }

  async disconnect(boardId: string): Promise<void> {
    if (!this.connection) return;
    try {
      await this.connection.invoke('LeaveBoard', boardId);
    } finally {
      await this.connection.stop();
      this.connection = undefined;
    }
  }
}