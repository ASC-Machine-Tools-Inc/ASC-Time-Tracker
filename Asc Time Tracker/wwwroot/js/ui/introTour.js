// Handles the tour for first-time desktop page viewers.
function startTour() {
    if ($("#playIntro").length === 0) {
        localStorage.setItem("redirectToTour", "true");
        window.location.replace("/");

        // Don't start the intro if we're in the middle of redirecting.
        return;
    }

    introJs().setOptions({
        showProgress: true,
        disableInteraction: true,
        tooltipClass: "bootstrapTooltip",
        steps: [{
            title: "Hey there!",
            intro: "First time? This tour will run you through using the time " +
                "tracker (or you can click out to skip it)."
        },
        {
            title: "The sidebar",
            element: document.querySelector(".sidebar"),
            intro: "Over here are some general options."
        },
        {
            title: "Adding logs manually",
            element: document.getElementById("addLogLink"),
            intro: "<b>Add Log</b> will let you " +
                "enter a time log manually without a timer."
        },
        {
            title: "Help",
            element: document.getElementById("navbarHelpDropdownLink"),
            intro: "Under the <b>Help</b> menu, you can replay this tour later or " +
                "find where to reach out for questions/bug reports."
        },
        {
            title: "Logging out",
            element: document.getElementById("dropdownUserMenu"),
            intro: "You can change your password or log out under the <b>Account</b> menu."
        },
        {
            title: "Adding timers",
            element: document.getElementById("addTimerBtn"),
            intro: "You can add multiple timers to track tasks you're working on."
        },
        {
            title: "Filtering logs",
            element: document.getElementById("mainLogs"),
            intro: "Down here, you can filter your logs from finished tasks, view " +
                "stats for them, and export your logs."
        },
        {
            title: "The end",
            intro: "Thanks for checking out the time tracker! If you ever want to " +
                "replay this tour, you can do it under the <b>Help</b> menu."
        }]
    }).start();
}

// Check on load if we're running the tour.
$(document).ready(function () {
    // Create the redirect flag if it doesn't exist.
    var redirect = localStorage.getItem("redirectToTour");
    if (redirect == null) {
        localStorage.setItem("redirectToTour", "false");
    }

    // Changed window to main index after click, run the tour.
    if (redirect === "true") {
        // A trick to get around running code after a page redirect by
        // setting a flag before redirecting.
        localStorage.setItem("redirectToTour", "false");
        startTour();
    } else {
        // Only run on the main index, and if the flag is set.
        var runIntro = $("#playIntro").val();

        if (!runIntro) return;

        // Don't run on mobile.
        if (document.documentElement.clientWidth < 576) return;

        startTour();
    }
});