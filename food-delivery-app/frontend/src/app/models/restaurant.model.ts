import { Food } from './food.model';

export interface Restaurant {
  id: number;
  name: string;
  description: string;
  location: string;
  rating: number;
  imageUrl?: string;
  createdAt?: string;
  foodItems?: Food[];
}

export interface RestaurantRequest {
  name: string;
  description: string;
  location: string;
  rating: number;
  imageUrl?: string;
}
