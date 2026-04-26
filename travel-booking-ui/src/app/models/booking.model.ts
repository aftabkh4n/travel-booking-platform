export interface Booking {
  id?: string;
  customerName: string;
  customerEmail: string;
  destination: string;
  departureDate: string;
  returnDate: string;
  passengers: number;
  totalPrice: number;
  status: string;
  createdAt?: string;
  notes?: string;
}

export interface CreateBookingRequest {
  customerName: string;
  customerEmail: string;
  destination: string;
  departureDate: string;
  returnDate: string;
  passengers: number;
  totalPrice: number;
  notes?: string;
}