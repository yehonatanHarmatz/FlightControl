'use strict';

function updateFlightsList(flightsToAdd, flightsToRemove) {
    flightsToAdd.forEach(flight => appendToFlightsList(flight));
    flightsToRemove.forEach(flight => removeFromFlightsList(flight.flight_id));
}

function appendToFlightsList(flight) {
    let $flightRow = $('<tr>', { id: 'rowFlight' + flight.flight_id });
    let $flightIdCell = $('<td>').text(flight.flight_id);
    let $companyNameCell = $('<td>').text(flight.company_name);
    $flightRow.append($flightIdCell, $companyNameCell);

    if (flight.is_external) {
        $('#externalFlightsTable tbody').append($flightRow);
    } else {
        let $deleteFlightButton = $('<button>', {
            type: 'button',
            'class': 'close',
            click: () => deleteFlight(flight.flight_id)
        }).html('&times;');
        $companyNameCell.append($deleteFlightButton);
        $('#internalFlightsTable tbody').append($flightRow);
    }
}

function removeFromFlightsList(flightId) {
    $('#rowFlight' + flightId).remove();
}

function deleteFlight(flightId) {
    $('#rowFlight' + flightId).fadeOut('fast', () => $(this).remove());
    removeFlight(flightId);
    deleteFlightFromServer(flightId);
}
