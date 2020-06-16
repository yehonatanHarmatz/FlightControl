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

    // Check if the flight plan contains all the requaired fields.
    let conatainsAllFields = mainProperies.every(property => property in flightPlan)
        // Fileds of initial location.
        && initialLocationProperties.every(property => property in flightPlan.initial_location)
        // Fileds of every segment.
        && Array.isArray(flightPlan.segments)
        && flightPlan.segments.every(segment => {
            return segmentsProperties.every(property => property in segment);
        });

    if (conatainsAllFields) {
        // Check if the values of the fields are valid.
        // Validate the lonigude and latitude in the initial location.
        let valuesAreValid = flightPlan.initial_location.longitude > -180
            && flightPlan.initial_location.longitude < 180
            && flightPlan.initial_location.latitude > -90
            && flightPlan.initial_location.latitude < 90
            // Validate the lonigude and latitude in every segment.
            && flightPlan.segments.every(segment => {
                return segment.longitude > -180 && segment.longitude < 180
                    && segment.latitude > -90 && segment.latitude < 90;
            });

        return valuesAreValid;
    }

    return false;
}

$(document).ready(function () {
    // Enter the flight plan to the server when the user chose a file, i.e.,
    // when the onchange event of the corresponding input element fires.
    $('#flightPlanInput').on('change', enterFlightPlan);
});
