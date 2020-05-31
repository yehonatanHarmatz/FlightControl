"use strict";

$(document).ready(function () {
    // Refresh data (flights) every 2 seconds.
    setInterval(function () {
        // Get all flights from the server.
        getAllFlightsFromServer(function (flights) {
            updateFlights(flights);

            // Update the list of flights. Add the new flights and remove
            // the old ones.
            updateFlightsList(getAddedFlights(), getRemovedFlights());
        });
    }, 2000);
});
