﻿"use strict";

// The flights before the last update.
let previousFlights = [];

// The current flights.
let currentFlights = [];

// Updates the flights to the new ones.
function updateFlights(newFlights) {
    previousFlights = currentFlights;
    currentFlights = newFlights;
}

// Gets the flights which were added during the last update.
function getAddedFlights() {
    return currentFlights.filter(function (currentFlight) {
        return previousFlights.every(function (previousFlight) {
            return previousFlight.flight_id !== currentFlight.flight_id;
        })
    });
}

// Gets the flights which were removed during the last update.
function getRemovedFlights() {
    return previousFlights.filter(function (previousFlight) {
        return currentFlights.every(function (currentFlight) {
            return currentFlight.flight_id !== previousFlight.flight_id;
        })
    });
}

// Removes the flight specified by the given flight-id.
function removeFlight(flightId) {
    currentFlights = currentFlights.filter(currentFlight => currentFlight.flight_id !== flightId);
}