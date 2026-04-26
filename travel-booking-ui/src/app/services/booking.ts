import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, CreateBookingRequest } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private gatewayUrl = 'http://localhost:5177/gateway';
  private apiKey = 'travel-api-key-local-dev';

  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'X-Api-Key': this.apiKey
  });

  constructor(private http: HttpClient) {}

  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.gatewayUrl}/api/bookings`, { headers: this.headers });
  }

  getBooking(id: string): Observable<Booking> {
    return this.http.get<Booking>(`${this.gatewayUrl}/api/bookings/${id}`, { headers: this.headers });
  }

  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(`${this.gatewayUrl}/api/bookings`, booking, { headers: this.headers });
  }

  updateStatus(id: string, status: string): Observable<Booking> {
    return this.http.patch<Booking>(
      `${this.gatewayUrl}/api/bookings/${id}/status`,
      { status },
      { headers: this.headers }
    );
  }

  deleteBooking(id: string): Observable<void> {
    return this.http.delete<void>(`${this.gatewayUrl}/api/bookings/${id}`, { headers: this.headers });
  }
}