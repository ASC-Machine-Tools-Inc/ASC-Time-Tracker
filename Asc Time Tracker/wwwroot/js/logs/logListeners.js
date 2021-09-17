// █▀▀ █ █ █▀▀ █▄ █ ▀█▀  █   ▀█▀ █▀▀ ▀█▀ █▀▀ █▄ █ █▀▀ █▀█ █▀▀
// ██▄ ▀▄▀ ██▄ █ ▀█  █   █▄▄ ▄█▄ ▄██  █  ██▄ █ ▀█ ██▄ █▀▄ ▄██

// Event handlers for shifting the current date.
$(".date-prev").on("click", function () {
    var prev = new Date(startDate.getTime());
    prev.setDate(prev.getDate() - 1);

    // Set the first range picker if we're using those.
    if (datePickers["current"] === datePickers["range"]) {
        setRangeStartPicker(prev);
    } else {  // Otherwise, just shift the date a unit back.
        shiftDate(prev);
    }
});

$(".date-next").on("click", function () {
    var next = new Date(endDate.getTime());
    next.setDate(next.getDate() + 1);

    // Set the second range picker if we're using those.
    if (datePickers["current"] === datePickers["range"]) {
        setRangeEndPicker(next);
    } else {  // Otherwise, just shift the date a unit ahead.
        shiftDate(next);
    }
});

// Update the date when we switch the filter.
$("#dateOption").on("change", function () {
    setCurrentPicker(this.value);
});

// Update the filter when we switch the field.
$("#fieldOption").on("change", function () {
    switch (this.value) {
        case "Job number":
            // if (savedFilter) doThing;
            fieldPickers["current"] = fieldPickers["jobNum"];
            break;
        case "Notes":
            // if (savedFilter) doThing;
            fieldPickers["current"] = fieldPickers["notes"];
            break;
        case "Research and design":
            // if (savedFilter) doThing;
            fieldPickers["current"] = fieldPickers["rd"];
            break;
    }

    // Toggle visibility of field pickers.
    let fieldOptions = ["jobNum", "notes", "rd"];
    for (let option of fieldOptions) {
        if (fieldPickers["current"] === fieldPickers[option]) {
            fieldPickers[option].show();
        } else {
            fieldPickers[option].hide();
        }
    }
});

// Update number of jobs shown for pie chart when dropdown changed.
$("body").on("change", "#pieChartNumSelect", function () {
    pieFilter = $("#pieChartNumSelect").val();
    saveFilterData();
    updateStats();
});

$("#empIdFilterForm").submit(function (e) {
    e.preventDefault();

    savedEmpId = $("#empIdFilter").val();

    // Append the email to the employee id if it doesn't have one.
    if (savedEmpId.substr(-10) !== "@ascmt.com") savedEmpId += "@ascmt.com";

    saveFilterData();
    updatePage();
});

// Clear filter data on logout.
$("#logoutButton").on("click",
    function () {
        localStorage.removeItem("filterData");
    });