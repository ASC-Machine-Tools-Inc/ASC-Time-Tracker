// TODO: run this only on first login, and only on main index
introJs().setOptions({
    showProgress: true,
    disableInteraction: true,
    tooltipClass: "bootstrapTooltip",
    steps: [{
        title: "Welcome",
        intro: "We see it's your first time using the site. This tour will run you " +
            "through the basics (or you can click anywhere out of this to skip it)."
    },
    {
        title: "The sidebar",
        element: document.querySelector(".sidebar"),
        intro: "Over here you can find some basic options. <b>Add Log</b> will let you " +
            "enter a time log manually without a timer, which we'll get to soon."
    },
    {
        title: "Logging out",
        element: document.getElementById("dropdownUserMenu"),
        intro: "You can change your password or log out under the account menu."
    },
    {
        title: "Adding timers",
        element: document.getElementById("addTimerBtn"),
        intro: "Adios!"
    },
    {
        title: "Filtering logs",
        element: document.querySelector(".main-log-card-header"),
        intro: "Adios!"
    },
    {
        title: "The end",
        element: document.getElementById("navbarHelpDropdownLink"),
        intro: "Adios!"
    }]
}).start();