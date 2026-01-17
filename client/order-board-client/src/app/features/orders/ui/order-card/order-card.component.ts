import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { OrderResponse } from '../../../../core/models/orders.models';

@Component({
  selector: 'app-order-card',
  imports: [CommonModule],
  templateUrl: './order-card.component.html',
  styleUrls: ['./order-card.component.scss'],
})
export class OrderCardComponent {
  @Input({ required: true }) order!: OrderResponse;
  @Input() readonly = false;

  @Output() next = new EventEmitter<OrderResponse>();
  @Output() cancel = new EventEmitter<OrderResponse>();
}
