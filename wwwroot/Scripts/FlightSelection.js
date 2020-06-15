'use strict';

// Cancel the selection when clicking anywhere on the map.
$(document).ready(() => {
    addClickEventToMap(cancelFlightSelection);
});

// The selected flight.
let selectedFlight = null;

// Enables the option to select the given flights by either clicking on
// their airplane icon or clicking on their row in the list of flights.
function enableSelectionOfFlights(flights) {
    flights.forEach(function (flight) {
        // Add a click event to the icon of the flight that selects
        // the flight.
        addClickEventToFlightIcon(flight.flight_id,
            () => selectFlight(flight));

        // Add a click event to the row of the flight (in the list of flights)
        // that selects the flight.
        addClickEventToFlightsListItem(flight.flight_id,
            () => selectFlight(flight));
    })
}

// Cancels the last selection of a flight.
function cancelFlightSelection() {
    // Check if there is a selected flight.
    if (selectedFlight) {
        // Stop emphasizing the selected flight on the map.
        deemphasizeFlightOnMap(selectedFlight.flight_id);

        // Stop displaying the selected flight's details.
        hideFlightDetails();

        selectedFlight = null;
    }
}

// Selects the flight with the given flight.
function selectFlight(flight) {
    // Check if there is a selected flight already.
    if (selectedFlight) {
        cancelFlightSelection();
    }

    // Select the flight.
    selectedFlight = flight;

    // Emphasize the selected flight on the map.
    emphasizeFlightOnMap(flight.flight_id);

    // Display the selected flight's details.
    showFlightDetails(flight);
}

// Hides the flight details section.
function hideFlightDetails() {
    $('#flightDetailsTable').attr('hidden', true);
}

// Shows the flight details of the given flight.
function showFlightDetails(flight) {
    // Sets the fields of the flight details section with the
    // correct values.
    $('#flightDetailsId').text(flight.flight_id);
    $('#flightDetailsCompany').text(flight.company_name);
    $('#flightDetailsPassengers').text(flight.passengers);
    $('#flightDetailsLongitude').text(flight.longitude);
    $('#flightDetailsLatitude').text(flight.latitude);

    // Show the flight details section.
    $('#flightDetailsTable').attr('hidden', false);
}

// Returns true or false whether the flight specified by the given
// flight ID is selected or not.
function isSelectedFlight(flightId) {
    return selectedFlight && flightId === selectedFlight.flight_id;
}
