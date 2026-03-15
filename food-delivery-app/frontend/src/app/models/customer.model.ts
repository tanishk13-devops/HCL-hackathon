export interface Customer {
  id?: number;
  name: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  zipCode: string;
  createdAt?: Date;
  updatedAt?: Date;
}
