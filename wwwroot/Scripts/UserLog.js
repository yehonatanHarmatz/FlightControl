// The value returned from setTimeout (log displayed to the user).
let logTimeout = null;

function displayLog(message) {
    let $log = $('#logText');

    // Stop the timer for deleting the old message.
    clearTimeout(logTimeout);

    // Display the message.
    $log.text(message);

    // Set a timer of 5 seconds that deletes the message when activated.
    logTimeout = setTimeout(() => $log.html('&nbsp;'), 10000);
}
