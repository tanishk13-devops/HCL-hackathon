export interface Review {
  id?: number;
  userId?: number;
  restaurantId: number;
  rating: number;
  comment: string;
  createdAt?: string;
  user?: { id: number; name: string };
}
