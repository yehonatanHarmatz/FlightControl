'use strict';

// The flights before the last update.
let previousFlights = [];

// The current flights.
let currentFlights = [];

// Updates the flights to the new ones.
function updateFlights(newFlights) {
    previousFlights = currentFlights;
    currentFlights = newFlights;
}

// Gets all of the current flights.
function getAllFlights() {
    return currentFlights;
}

// Gets the flights that were added in the last update.
function getAddedFlights() {
    // Return the current flights that were not on the previous flights.
    return currentFlights.filter(function (currentFlight) {
        return previousFlights.every(function (previousFlight) {
            return previousFlight.flight_id !== currentFlight.flight_id;
        })
    });
}

// Gets the flights that were removed in the last update.
function getRemovedFlights() {
    // Return the previous flights that are not on the current flights.
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

// Gets the flight specified by the given flight ID.
function getFlightById(flightId) {
    return currentFlights.find(flight => flight.flight_id === flightId);
}
