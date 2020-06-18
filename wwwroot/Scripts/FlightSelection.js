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
    if (wasRemoved(flight.flight_id)) {
        return;
    }

    // Check if there is a selected flight already.
    if (selectedFlight) {
        // Stop emphasizing the selected flight on the map.
        deemphasizeFlightOnMap(selectedFlight.flight_id);
    }

    // Select the flight.
    selectedFlight = flight;

    // Get the flight plan of the selected flight.
    getFlightPlanFromServer(flight.flight_id, flightPlan => {
        // Emphasize the selected flight on the map.
        emphasizeFlightOnMap(flight.flight_id, flightPlan);

        // Display the selected flight's details. Notice: we use the most recent
        // value we got from the server because the flight can change its position.
        showFlightDetails(getFlightById(flight.flight_id), flightPlan);
    });
}

// Hides the flight details section.
function hideFlightDetails() {
    $('#flightDetailsTable').attr('hidden', true);
    $('#SelectFlightDescription').attr('hidden', false);
}

// Shows the flight details of the given flight.
function showFlightDetails(flight, flightPlan) {
    // Calculate the locations.
    let initialLocation = '(' + roundKDigits(flightPlan.initial_location.latitude, 3)
        + ', ' + roundKDigits(flightPlan.initial_location.longitude, 3) + ')';

    let lastSegment = flightPlan.segments[flightPlan.segments.length - 1];
    let destination = '(' + roundKDigits(lastSegment.latitude, 3)
        + ', ' + roundKDigits(lastSegment.longitude, 3) + ')';

    // Calculate the departure and landing times.
    let departureTime = new Date(flightPlan.initial_location.date_time);

    let landingTime = departureTime;
    flightPlan.segments.forEach(
        // Add the timespan of the current segmet to the landing time.
        segment => landingTime.setSeconds(landingTime.getSeconds() + segment.timespan_seconds)
    );

    // Sets the fields of the flight details section with the
    // correct values.
    $('#flightDetailsId').text(flight.flight_id);
    $('#flightDetailsCompany').text(flight.company_name);
    $('#flightDetailsPassengers').text(flight.passengers);
    $('#flightDetailsInitialLocation').text(initialLocation);
    $('#flightDetailsDepartureTime').text(departureTime.toUTCString());
    $('#flightDetailsDestination').text(destination);
    $('#flightDetailsLandingTime').text(landingTime.toUTCString());

    // Show the flight details section.
    $('#SelectFlightDescription').attr('hidden', true);
    $('#flightDetailsTable').attr('hidden', false);
}

// Rounds the given number by k digits.
function roundKDigits(num, k) {
    return Math.round((num + Number.EPSILON) * Math.pow(10, k)) / Math.pow(10, k);
}

// Returns true or false whether the flight specified by the given
// flight ID is selected or not.
function isSelectedFlight(flightId) {
    return selectedFlight && flightId === selectedFlight.flight_id;
}

// Cancel the selection of a removed flight (most likely removed externaly).
function checkSelectionOfRemovedFlights(removedFlights) {
    removedFlights.forEach(flight => {
        if (isSelectedFlight(flight)) {
            // Cancel the selection of that flight.
            hideFlightDetails()
            selectedFlight = null;
        }
    })
}
