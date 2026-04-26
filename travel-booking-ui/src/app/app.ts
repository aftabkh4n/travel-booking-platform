import { Component } from '@angular/core';
import { BookingFormComponent } from './components/booking-form/booking-form';
import { BookingListComponent } from './components/booking-list/booking-list';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [BookingFormComponent, BookingListComponent, HttpClientModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {
  title = 'Travel Booking Platform';

  onBookingCreated(): void {
    const listComponent = document.querySelector('app-booking-list');
    if (listComponent) {
      (listComponent as any).__ngContext__[8].loadBookings();
    }
  }
}