"use strict";

let mymap = L.map('map').setView([20, 0], 2); // the map created

let flights_marker = new Map(); // the current flights' markers

let flights_deg = new Map(); // the current flights' deg

let flight_clicked_id;
let flight_route = []; // the route of the current 

let planeIcon = L.Icon.extend({ // the plane icon's settings
    options: {
        iconUrl: 'Images/airplane.png'
    }
});

let line_style = { // the lines' style
    "color": "#ff7800",
    "weight": 5,
    "opacity": 0.65
};

let plane = new planeIcon({iconSize: [30, 30] }); // default plane icon
let big_plane = new planeIcon({iconSize: [60, 60] }); // big plane icon

// must be put for the map to work:
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 18,
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, ' +
        '<a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
        'Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);

/*
 * Name: addMarker
 * Args: None
 * Details: Updates plane's icon location.
 *          This function is being activated every 2 seconds.
 */
function addMarker() {
    currentFlights.forEach(flight => {
        let flight_icon = getFlightIcon(flight.flight_id); // gets the icon 
        let new_lat = flight.latitude; // the current latitude of the plane
        let new_long = flight.longitude; // the current longitude of the plane
        let old_lat; // the old latitude
        let old_long; // the old longitude
        if (getFlightIcon(flight.flight_id) === -1) { // this is a new plane
            flight_icon = L.marker([new_lat, new_long], { icon: plane }).addTo(mymap); // creates the icon
            flights_marker.set(flight.flight_id, flight_icon); // adds to map with the flight's id
            flights_deg.set(flight.flight_id, 0); // adds to map with the flight's id
        } else {
            previousFlights.forEach(old_flight => { // goes on each old flight
                if (old_flight.flight_id === flight.flight_id) { // if this is the same plane as this current plane
                    old_lat = old_flight.latitude; // sets the old latitude
                    old_long = old_flight.longitude; // sets the old longitude
                }
            });
            let first_arg = old_long - new_long; // gets the first sin argument
            let second_arg = old_lat - new_lat; // gets the second sin argument
            let deg = Math.atan2(first_arg, second_arg) * (180 / Math.PI) + 180 - 45; // calculate the angle of the plane
            flight_icon.setLatLng(new L.LatLng(new_lat, new_long)); // sets the new location
            flight_icon.setRotationAngle(deg); // sets the rotation.
            flight_icon.setRotationOrigin("center");
            flights_deg.set(flight.flight_id, deg);
        }
    });
    for(let flight_id of flights_marker.keys()) { // deletes the deleted flights
        let delete_flight = 1;
        currentFlights.forEach(flight => {
            if (flight_id === flight.flight_id) {
                delete_flight = 0;
            }
        });
        if (delete_flight === 1) {
            (flights_marker.get(flight_id)).remove();
            flights_marker.delete(flight_id);
        }
    };
}

// Removes the plane of the flight specified by the given flight ID from the map.
function removePlane(flightId) {
    (flights_marker.get(flightId)).remove();
    flights_marker.delete(flightId);
}

/*
 * Name: emphasizeFlightOnMap
 * Args: flight's id and its flight plan
 * Details: does the following actions to the flight(with the given id):
 *          1. Changes the plane's icon to the big red one.
 *          2. Activates the showRoute function for the flight's id.
 */
function emphasizeFlightOnMap(flight_id, flightPlan) {
    let flight_icon = getFlightIcon(flight_id); // gets the icon;
    flight_icon.setIcon(big_plane);
    showRoute(flightPlan);
}

/*
 * Name: deemphasizeFlightOnMap
 * Args: flight's id
 * Details: does the following actions to the flight(with the given id):
 *          1. Changes the plane's icon to return to normal
 *          2. Activates the deleteRoute function for the flight's id.
 */
function deemphasizeFlightOnMap(flightId) {
    let flight_icon = getFlightIcon(flightId); // gets the icon;
    flight_icon.setIcon(plane);
    deleteRoute();
}

/*
 * Name: showRoute
 * Args: flight's id
 * Details: being called to contine the route creation
 */
function showRoute(flight_plan) {
    let i = 0; // counter
    let lat_1, lat_2, long_1, long_2;
    lat_1 = flight_plan.initial_location.latitude;
    long_1 = flight_plan.initial_location.longitude;
    (flight_plan.segments).forEach(segment => { // sets a line between each segment
        lat_2 = segment.latitude;
        long_2 = segment.longitude;
        let line_settings = {
            "type": "LineString",
            "coordinates": [[long_1, lat_1], [long_2, lat_2]]
        }
        let line = L.geoJSON(line_settings, { style: line_style });
        line.addTo(mymap);
        flight_route.push(line);
        i++;
        lat_1 = segment.latitude;
        long_1 = segment.longitude;
    })
}

/*
 * Name: deleteRoute
 * Args: flight's id
 * Details: deletes the current shown route.
 */
function deleteRoute() {
    flight_route.forEach(line => { line.remove(); });
    flight_route = [];
}

/*
 * Name: getFlightIcon
 * Args: flight's id
 * Details: returns the flight icon for the flight id.
 *          If the flight doesn't exists, returns -1.
 */
function getFlightIcon(flight_id) {
    if(flights_marker.has(flight_id)) {
        return flights_marker.get(flight_id);
    }
    return -1;
}

/*
 * Name: addClickEventToFlightIcon
 * Args: flight's id and a function
 * Details: Adds a click event to the flight icon with the given id.
 */
function addClickEventToFlightIcon(flight_id, func) {
    let flight_icon = getFlightIcon(flight_id); // gets the icon;
    flight_icon.on('click', func);
}

/*
 * Name: addClickEventToMap
 * Args: flight's id and a function
 * Details: Adds a click event to the map;
 */
function addClickEventToMap(func) {
    mymap.on('click', func);
}
