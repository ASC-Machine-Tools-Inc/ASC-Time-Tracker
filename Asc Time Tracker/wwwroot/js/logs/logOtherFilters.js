var savedEmpIds = new Set();

// ▀█▀ █▄ █ ▀█▀ ▀█▀ ▀█▀ ▄▀▄ █   ▀█▀ ▀██ ▄▀▄ ▀█▀ ▀█▀ █▀█ █▄ █
// ▄█▄ █ ▀█ ▄█▄  █  ▄█▄ █▀█ █▄▄ ▄█▄ ██▄ █▀█  █  ▄█▄ █▄█ █ ▀█

// TODO: load in filter data.

// █▀▀ █ █ █▀▀ █▄ █ ▀█▀  █   ▀█▀ █▀▀ ▀█▀ █▀▀ █▄ █ █▀▀ █▀█ █▀▀
// ██▄ ▀▄▀ ██▄ █ ▀█  █   █▄▄ ▄█▄ ▄██  █  ██▄ █ ▀█ ██▄ █▀▄ ▄██

// Process applying filters.
$("#logFilters").submit(function (e) {
    e.preventDefault();

    let empIds = $("#empIdFilter").val();

    // Append the email to the employee id if it doesn't have one.
    if (empIds.substr(-10) !== "@ascmt.com") empIds += "@ascmt.com";

    savedEmpIds.add(empIds);

    console.log(savedEmpIds);

    saveFilterData();
    updatePage();
});