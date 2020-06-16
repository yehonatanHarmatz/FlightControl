'use strict';

// Adds and removes flights to/from the list of flights.
function updateFlightsList(flightsToAdd, flightsToRemove) {
    // Add flights to the list of flights.
    flightsToAdd.forEach(flight => appendToFlightsList(flight));

    // Remove flights from the list of flights.
    flightsToRemove.forEach(flight => removeFromFlightsList(flight.flight_id));
}

// Appends a flight to the end of the list of flights.
function appendToFlightsList(flight) {
    // Create a table row for the flight.
    let $flightRow = $('<tr>', { id: 'rowFlight' + flight.flight_id });

    // Add the flight data to its table row.
    let $flightIdCell = $('<td>').text(flight.flight_id);
    let $companyNameCell = $('<td>').text(flight.company_name);
    $flightRow.append($flightIdCell, $companyNameCell);

    // Check whether the flight is internal or external.
    if (flight.is_external) {
        // Append the flight's row to the external flights table.
        $('#externalFlightsTable tbody').append($flightRow);
    } else {
        // Add a 'delete flight' button flight's row.
        let $deleteFlightButton = $('<button>', {
            type: 'button',
            'class': 'close',
            click: () => deleteFlight(event, flight.flight_id)
        }).html('&times;');
        $companyNameCell.append($deleteFlightButton);

        // Append the flight's row to the internal flights table.
        $('#internalFlightsTable tbody').append($flightRow);
    }
}

// Removes the flight specified by the given flight ID from the list of flights.
function removeFromFlightsList(flightId) {
    $('#rowFlight' + flightId).remove();
}

// Deletes the flight specified by the given flight ID (both on server and client sides).
function deleteFlight(event, flightId) {
    event.stopPropagation();

    // Remove the flight from the list of flights.
    $('#rowFlight' + flightId).fadeOut('fast', () => $(this).remove());
    removeFlight(flightId);

    // Check if the flight has been selected.
    if (isSelectedFlight(flightId)) {
        // Cancel its selection.
        cancelFlightSelection();

        // Remove the plane of the flight from the map.
        removePlane(flightId);
    }

    // Delete the flight from the server.
    deleteFlightFromServer(
        flightId,
        // Define what to do on success.
        () => {
            $('#logText').text('The flight with the id ' + flightId + ' was deleted successfully.');
        }
    );
}

// Registers an event handler for the click event of the row of the flight
// specified by the given flight ID.
function addClickEventToFlightsListItem(flightId, handler) {
    $('#rowFlight' + flightId).click(handler);
}
