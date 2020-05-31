"use strict";

function updateFlightsList(flightsToAdd, flightsToRemove) {
    flightsToAdd.forEach(flight => appendToFlightsList(flight));
    flightsToRemove.forEach(flight => removeFromFlightsList(flight.flight_id));
}

function appendToFlightsList(flight) {
    let row = '<tr id="tableRow' + flight.flight_id + '"><td>'
        + flight.flight_id + '</td><td>'
        + flight.company_name;
    if (flight.is_external) {
        row += '</td></tr>'
        $('#externalFlightsTable tbody').append(row);
    } else {
        row += '<button type="button" class="close" onclick="deleteFlight(\''
            + flight.flight_id + '\')">&times;</button></td></tr>';
        $('#internalFlightsTable tbody').append(row);
    }
}

function removeFromFlightsList(flightId) {
    $('#tableRow' + flightId).remove();
}

function deleteFlight(flightId) {
    $('#tableRow' + flightId).fadeOut("fast", () => $(this).remove());
    removeFlight(flightId);
    deleteFlightFromServer(flightId);
}
