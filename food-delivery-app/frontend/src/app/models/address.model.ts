export interface Address {
  id: number;
  userId: number;
  street: string;
  city: string;
  state: string;
  pincode: string;
}

export interface AddressRequest {
  street: string;
  city: string;
  state: string;
  pincode: string;
}
