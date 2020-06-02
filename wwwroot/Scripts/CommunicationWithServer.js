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
    let d = new Date();
    let s = d.toISOString().split('.')[0] + 'Z';
    $.getJSON('api/Flights?relative_to=' + s + '&sync_all' , callback);
}
