export interface Food {
  id?: number;
  name: string;
  description: string;
  price: number;
  imageUrl?: string;
  category: string;
  availability: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}
