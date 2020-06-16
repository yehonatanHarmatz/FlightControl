'use strict';

// Enters new flight plan to the server, according to entered JSON file.
function enterFlightPlan() {
    // Get a reference to the entered JSON file.
    let jsonFile = this.files[0];
    let fileReader = new FileReader();

    // Enter the filght plan to the server when finished reading the JSON file.
    fileReader.onload = function () {
        try {
            // Parse the JSON file into a flight plan object.
            let flightPlan = JSON.parse(fileReader.result);

            // Validate that the flight plan is valid.
            if (flightPlanIsValid(flightPlan)) {
                // Post the flight plan to the server.
                postFlightPlanToServer(
                    flightPlan,
                    // Define what to do on success.
                    () => {
                        $('#logText').text('The flight plan from the file "'
                            + jsonFile.name + '" was entered successfully.');
                    }
                );
            } else {
                $('#logText').text('The flight plan from file "' + jsonFile.name + '" is invalid.');
            }
        } catch (e) {
            $('#logText').text('The file "' + jsonFile.name + '" is not a valid JSON file.');
        }
    }

    // Read the JSON file.
    fileReader.readAsText(jsonFile);
}

// Check if the given flight plan is valid (contains all the necessary fields).
function flightPlanIsValid(flightPlan) {
    // The properies for every requaired object in the flight plan.
    let mainProperies = ['passengers', 'company_name', 'initial_location', 'segments'];
    let initialLocationProperties = ['longitude', 'latitude', 'date_time'];
    let segmentsProperties = ['longitude', 'latitude', 'timespan_seconds'];

    // Check if all those properies are defined inside the flight plan.
    return mainProperies.every(property => property in flightPlan)
        && initialLocationProperties.every(property => property in flightPlan.initial_location)
        && Array.isArray(flightPlan.segments)
        && flightPlan.segments.every(segment => {
            return segmentsProperties.every(property => property in segment);
        });
}

$(document).ready(function () {
    // Enter the flight plan to the server when the user chose a file, i.e.,
    // when the onchange event of the corresponding input element fires.
    $('#flightPlanInput').on('change', enterFlightPlan);
});
