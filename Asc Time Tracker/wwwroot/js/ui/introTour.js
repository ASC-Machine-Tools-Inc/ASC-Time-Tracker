// TODO: run this only on first login, and only on main index, and not on mobile
function startIntro() {
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

// Don't run on mobile.
if (document.documentElement.clientWidth >= 576) {
    startIntro();
}