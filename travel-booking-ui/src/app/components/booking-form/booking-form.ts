import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking';
import { CreateBookingRequest } from '../../models/booking.model';

@Component({
  selector: 'app-booking-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-form.html',
  styleUrl: './booking-form.css'
})
export class BookingFormComponent {
  @Output() bookingCreated = new EventEmitter<void>();

  booking: CreateBookingRequest = {
    customerName: '',
    customerEmail: '',
    destination: '',
    departureDate: '',
    returnDate: '',
    passengers: 1,
    totalPrice: 0,
    notes: ''
  };

  submitting = false;
  error = '';
  success = '';

  constructor(private bookingService: BookingService) {}

  onSubmit(): void {
    this.submitting = true;
    this.error = '';
    this.success = '';

    this.bookingService.createBooking(this.booking).subscribe({
      next: () => {
        this.success = 'Booking created successfully!';
        this.submitting = false;
        this.resetForm();
        this.bookingCreated.emit();
      },
      error: () => {
        this.error = 'Failed to create booking. Please try again.';
        this.submitting = false;
      }
    });
  }

  resetForm(): void {
    this.booking = {
      customerName: '',
      customerEmail: '',
      destination: '',
      departureDate: '',
      returnDate: '',
      passengers: 1,
      totalPrice: 0,
      notes: ''
    };
  }
}