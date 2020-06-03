'use strict';

// Enters new flight plan to the server, according to entered JSON file.
function enterFlightPlan() {
    // Get a reference to the entered JSON file.
    let jsonFile = this.files[0];
    let fileReader = new FileReader();

    // Enter the filght plan to the server when finished reading the JSON file.
    fileReader.onload = function () {
        // Parse the JSON file into a flight plan object.
        let flightPlan = JSON.parse(fileReader.result);

        // Post the flight plan to the server.
        postFlightPlanToServer(flightPlan);
    }

    // Read the JSON file.
    fileReader.readAsText(jsonFile);
}

$(document).ready(function () {
    // Enter the flight plan to the server when the user chose a file, i.e.,
    // when the onchange event of the corresponding input element fires.
    $('#flightPlanInput').on('change', enterFlightPlan);
});
