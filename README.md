# travel-booking-platform

A travel booking system built with .NET 10, MongoDB, Angular, and a YARP API gateway that sits in front of everything.

The interesting part is not the bookings themselves. It is the gateway layer. Every request from Angular goes through the gateway first. The gateway checks the API key, enforces rate limits, and then proxies the request to the actual API. The API never gets called directly. This is how Azure API Management works in production, and this project replicates that pattern locally.

## What it looks like

The Angular dashboard has a Qatar Airways colour scheme because I work in Qatar and thought it would be a fun touch. You can create bookings, confirm them, cancel them, and delete them. Status badges update in real time without page refresh.

## How the pieces fit together

There are three .NET projects and one Angular app.

**TravelBooking.Api** handles the actual work. It saves bookings to MongoDB, reads them back, updates statuses, and logs a notification whenever a booking is created or confirmed. The notification goes to Logic Apps in production. Locally it just logs to the console with all the booking details.

**TravelBooking.Gateway** sits in front of the API. Built with YARP (Yet Another Reverse Proxy), Microsoft's open source reverse proxy library. It does three things: checks that every request has a valid API key in the X-Api-Key header, enforces a rate limit of 10 requests per 10 seconds, and proxies the request to the API with the /gateway prefix stripped. Angular calls the gateway on port 5177. The API runs on port 5153. They never talk directly.

**TravelBooking.Core** holds the shared models. Booking, BookingEvent, and the interfaces for the repository and notification service live here.

**travel-booking-ui** is the Angular 19 frontend. It talks exclusively to the gateway, never to the API directly. The API key goes in every request header automatically through the booking service.

## Running it locally

You need .NET 10, Node.js 20, and Docker Desktop.

Start MongoDB:

```bash
docker run -d --name travel-mongo -p 27017:27017 mongo:7
```

Copy the example configs:

```bash
cp TravelBooking.Api/appsettings.example.json TravelBooking.Api/appsettings.json
cp TravelBooking.Gateway/appsettings.example.json TravelBooking.Gateway/appsettings.json
```

Start the API in one terminal:

```bash
cd TravelBooking.Api
dotnet run
```

Start the Gateway in another terminal:

```bash
cd TravelBooking.Gateway
dotnet run
```

Install Angular dependencies and start the frontend:

```bash
cd travel-booking-ui
npm install
ng serve
```

Open http://localhost:4200 and you should see the dashboard.

## Testing the gateway directly

All requests need the API key header. Without it you get a 401:

```bash
curl -X GET http://localhost:5177/gateway/api/bookings \
  -H "X-Api-Key: travel-api-key-local-dev"
```

Try it without the header and you get:

```
Unauthorized: Invalid or missing API key
```

That is the gateway enforcing the policy before the request ever reaches the API.

## API endpoints

All of these are available through the gateway at /gateway/api/bookings.

```
POST   /api/bookings              Create a booking
GET    /api/bookings              List all bookings
GET    /api/bookings/{id}         Get one booking
PATCH  /api/bookings/{id}/status  Update booking status
DELETE /api/bookings/{id}         Delete a booking
GET    /health                    Health check
```

## Stack

- .NET 10
- ASP.NET Core 10
- YARP reverse proxy
- MongoDB via MongoDB.Driver
- Angular 19
- TypeScript
- Docker

## Contributing

Pull requests welcome. Open an issue if something is not working as expected.

---

Built with .NET 10, MongoDB, YARP, and Angular. MIT licence.