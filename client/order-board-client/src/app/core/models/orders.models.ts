export type OrderStatus = 'New' | 'Preparing' | 'Ready' | 'Completed' | 'Canceled';

export interface OrderItemDto {
  name: string;
  quantity: number;
}

export interface OrderResponse {
  id: string;
  customerName: string;
  boardId: string;
  status: OrderStatus;
  createdAt: string;
  items: OrderItemDto[];
}

export interface CreateOrderRequest {
  customerName: string;
  boardId: string;
  note?: string | null;
  items: { name: string; quantity: number }[];
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}