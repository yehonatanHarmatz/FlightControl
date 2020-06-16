'use strict';

// Deletes the flight specified by the given flight ID from the server.
function deleteFlightFromServer(flightId, onSuccess) {
    $.ajax({
        method: 'DELETE',
        url: 'api/Flights/' + flightId,
        success: onSuccess,
        error: jqXHR => {
            let message = 'Cannot delete the flight with the ID ' + flightId;
            switch (jqXHR.status) {
                case 400:
                    // The flight is external.
                    message += ' because it is external.';
                    break;
                case 404:
                    // The given ID doesn't belong to any flight.
                    message += ' because there is no such flight.';
                    break;
                default:
                    // An unknown error.
                    message += ' because of an unknown error.';
            }
            // Display the error message to the user.
            $('#logText').text(message + ' [HTTP status code: ' + jqXHR.status + ']');
        }
    });
}

// Get the flight plan of the flight specified by the given flight ID from
// the server.
function getFlightPlanFromServer(flightId, onSuccess) {
    $.ajax({
        method: 'GET',
        url: 'api/FlightPlan/' + flightId,
        dataType: 'json',
        success: onSuccess,
        error: jqXHR => {
            let message = 'Cannot get the flight plan of the flight with the ID ' + flightId;
            switch (jqXHR.status) {
                case 404:
                    // The given ID doesn't belong to any flight.
                    message += ' because there is no such flight.';
                    break;
                case 500:
                    // There was an error on one of the external server.
                    message += ' because of an error on an external server.';
                    break;
                default:
                    // An unknown error.
                    message += ' because of an unknown error.';
            }
            // Display the error message to the user.
            $('#logText').text(message + ' [HTTP status code: ' + jqXHR.status + ']');
        }
    });
}

// Gets all the active flights from the server, relative to the current time.
function getActiveFlightsFromServer(onSuccess) {
    // Get the ISO string representation of the current time, without milliseconds.
    let currentTime = new Date().toISOString().split('.')[0] + 'Z';

    // Get all the active flights from the server, relative to the current time.
    $.ajax({
        method: 'GET',
        url: 'api/Flights?relative_to=' + currentTime + '&sync_all',
        // The respone will be JSON format.
        dataType: 'json',
        success: onSuccess,
        error: jqXHR => {
            let message = 'Cannot get the active flights relative to ' + currentTime;
            switch (jqXHR.status) {
                case 500:
                    // There was an error on one of the external server.
                    message += ' because of an error on an external server.';
                    break;
                default:
                    // An unknown error.
                    message += ' because of an unknown error.';
            }
            // Display the error message to the user.
            $('#logText').text(message + ' [HTTP status code: ' + jqXHR.status + ']');
        }
    });
}

// Post a flight plan object to the server.
function postFlightPlanToServer(flightPlan, onSuccess) {
    // Stringify the flight plan object.
    let jsonFlightPlan = JSON.stringify(flightPlan);

    // Post the flight plan to the server.
    $.ajax({
        method: 'POST',
        url: 'api/FlightPlan',
        data: jsonFlightPlan,
        contentType: 'application/json',
        dataType: 'json',
        success: onSuccess,
        error: jqXHR => {
            let message = 'Cannot post a flight plan to the server';
            switch (jqXHR.status) {
                case 400:
                    // The flight plan is invalid.
                    message += ' because it is an invalid flight plan.';
                    break;
                default:
                    // An unknown error.
                    message += ' because of an unknown error.';
            }
            // Display the error message to the user.
            $('#logText').text(message + ' [HTTP status code: ' + jqXHR.status + ']');
        }
    });
}
