'use strict';

// Deletes the flight specified by the given flight ID from the server.
function deleteFlightFromServer(flightId) {
    $.ajax({
        type: 'DELETE',
        url: 'api/Flights/' + flightId
    })
}

// Get the flight plan of the flight specified by the given flight ID from
// the server.
function getFlightPlanFromServer(flightId, callback) {
    $.getJSON('api/FlightPlan/' + flightId, callback);
}

// Gets all the active flights from the server, relative to the current time.
function getActiveFlightsFromServer(callback) {
    // Get the ISO string representation of the current time, without milliseconds.
    let currentTime = new Date().toISOString().split('.')[0] + 'Z';

    // Get all the active flights from the server, relative to the current time.
    $.getJSON('api/Flights?relative_to=' + currentTime + '&sync_all' , callback);
}

// Post a flight plan object to the server.
function postFlightPlanToServer(flightPlan) {
    // Stringify the flight plan object.
    let jsonFlightPlan = JSON.stringify(flightPlan);

    // Post the flight plan to the server.
    $.ajax({
        type: 'POST',
        url: 'api/FlightPlan',
        data: jsonFlightPlan,
        contentType: 'application/json',
        dataType: 'json',
    });
}
