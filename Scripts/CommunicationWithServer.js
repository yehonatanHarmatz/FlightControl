"use strict";

function deleteFlightFromServer(flightId) {
    $.ajax({
        url: 'api/Flights/' + flightId,
        type: 'DELETE'
    })
}

function getFlightPlanFromServer(flightId, callback) {
    $.getJSON('api/FlightPlan/' + flightId, callback);
}

function getAllFlightsFromServer(callback) {
    $.getJSON('api/Flights', callback);
}
