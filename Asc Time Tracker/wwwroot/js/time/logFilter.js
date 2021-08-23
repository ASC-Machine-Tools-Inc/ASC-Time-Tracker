/* Handles all the filtering for the index, updating the view
 * with ajax.
 */

var datePickers = {};
var fieldPickers = {};

var startDate, endDate, savedDate;
var savedFilter;

var savedEmpId;

// Set default filter.
var pieFilter = 5;

// ▀█▀ █▄ █ ▀█▀ ▀█▀ ▀█▀ ▄▀▄ █   ▀█▀ ▀██ ▄▀▄ ▀█▀ ▀█▀ █▀█ █▄ █
// ▄█▄ █ ▀█ ▄█▄  █  ▄█▄ █▀█ █▄▄ ▄█▄ ██▄ █▀█  █  ▄█▄ █▄█ █ ▀█

function startPickers() {
    startDatePickers();
    startFieldPickers();

    // Use user id as default one.
    if (savedEmpId == null) savedEmpId = $("#empIdFilter").val();
}

function startDatePickers() {
    // Prep the datepicker for days.
    datePickers["day"] = $(".day-picker");
    datePickers["day"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#dayContainer",
    }).on("changeDate", function (e) {
        setDayPicker(e.date);
    });

    // Prep the datepicker for weeks.
    datePickers["week"] = $(".week-picker");
    datePickers["week"].hide();
    datePickers["week"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#weekContainer"
    }).on("changeDate", function (e) {
        setWeekPicker(e.date);
    });

    // Prep the datepicker for months.
    datePickers["month"] = $(".month-picker");
    datePickers["month"].hide();
    datePickers["month"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#monthContainer",
        startView: "months",
        minViewMode: "months"
    }).on("changeDate", function (e) {
        setMonthPicker(e.date);
    });

    // Set a dummy value for selecting all.
    datePickers["all"] = null;

    // Prep the custom range picker.
    datePickers["range"] = $("#dateRangePicker");
    datePickers["range"].hide();
    datePickers["rangeStart"] = $(".range-picker-start");
    datePickers["rangeEnd"] = $(".ranger-picker-end");
    datePickers["rangeStart"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#rangeStartContainer"
    }).on("changeDate", function (e) {
        setRangeStartPicker(e.date);
    });
    datePickers["rangeEnd"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#rangeEndContainer"
    }).on("changeDate", function (e) {
        setRangeEndPicker(e.date);
    });

    // Initialize.
    datePickers["current"] = datePickers["day"];
};

// TODO: add field filters
function startFieldPickers() {
    fieldPickers["jobNum"] = $(".job-number-picker");
    fieldPickers["jobNum"].on("change", function () {
        console.log("Job number changed");
    });

    fieldPickers["notes"] = $(".notes-picker");
    fieldPickers["notes"].hide();

    fieldPickers["rd"] = $(".rd-picker");
    fieldPickers["rd"].hide();

    // Initialize.
    fieldPickers["current"] = fieldPickers["jobNum"];
}

// █▀▀ █ █ █▀▀ █▄ █ ▀█▀  █   ▀█▀ █▀▀ ▀█▀ █▀▀ █▄ █ █▀▀ █▀█ █▀▀
// ██▄ ▀▄▀ ██▄ █ ▀█  █   █▄▄ ▄█▄ ▄██  █  ██▄ █ ▀█ ██▄ █▀▄ ▄██

// Event handlers for shifting the current date.
$(".date-prev").on("click", function () {
    var prev = new Date(startDate.getTime());

    // Set the first range picker if we're using those.
    prev.setDate(prev.getDate() - 1);

    if (datePickers["current"] === datePickers["range"]) {
        setRangeStartPicker(prev);
        return;
    } else {
        shiftDate(prev);
    }
});

$(".date-next").on("click", function () {
    var next = new Date(endDate.getTime());

    // Set the second range picker if we're using those.
    if (datePickers["current"] === datePickers["range"]) {
        setRangeEndPicker(next);
        return;
    }

    // Don't add one if we're using the dayPicker,
    // since its endDate is already one ahead..
    if (datePickers["current"] !== datePickers["day"]) {
        next.setDate(next.getDate() + 1);
    }

    shiftDate(next);
});

// Update the date when we switch the filter.
$("#dateOption").on("change", function () {
    // Show the arrows to switch if choosing a time frame.
    $(".date-prev").show();
    $(".date-next").show();

    switch (this.value) {
        case "Day":
            if (savedDate) setDayPicker(savedDate);
            datePickers["current"] = datePickers["day"];
            break;
        case "Week":
            if (savedDate) setWeekPicker(savedDate);
            datePickers["current"] = datePickers["week"];
            break;
        case "Month":
            if (savedDate) setMonthPicker(savedDate);
            datePickers["current"] = datePickers["month"];
            break;
        case "All":
            $(".date-prev").hide();
            $(".date-next").hide();
            datePickers["current"] = datePickers["all"];

            // Get all logs from epoch to 5138 (if they enter logs for the future).
            // Should be fine, right? If anyone sees this software from the future
            // because it's broken I apologize.
            startDate = new Date(0);
            endDate = new Date(100000000000000);
            updatePage();
            break;
        case "Custom range":
            datePickers["current"] = datePickers["range"];
            break;
    }

    // Toggle visibility of date pickers.
    let dateOptions = ["day", "week", "month", "range"];
    for (let option of dateOptions) {
        if (datePickers["current"] === datePickers[option]) {
            datePickers[option].show();
        } else {
            datePickers[option].hide();
        }
    }
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
    updateStats();
});

$("#empIdFilterForm").submit(function (e) {
    e.preventDefault();

    savedEmpId = $("#empIdFilter").val();

    // Append the email to the employee id if it doesn't have one.
    if (savedEmpId.substr(-9) !== "@ascmt.com") savedEmpId += "@ascmt.com";

    updatePage();
});

// █▄█ █▀▀ █   █▀█ █▀▀ █▀█ █▀▀
// █ █ ██▄ █▄▄ █▀▀ ██▄ █▀▄ ▄██

// Called by the prev and next buttons to change the date.
function shiftDate(date) {
    if (datePickers["current"] === datePickers["day"]) {
        setDayPicker(date);
    } else if (datePickers["current"] === datePickers["week"]) {
        setWeekPicker(date);
    } else if (datePickers["current"] === datePickers["month"]) {
        setMonthPicker(date);
    }
}

function setDayPicker(date) {
    startDate = date;
    // Advance day by 1 to next day's midnight to grab any logs during the day.
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);

    datePickers["day"].datepicker("update", date);

    savedDate = date;

    datePickers["day"].val(dateToString(date));

    // Update the partial views.
    updatePage();
}

function setWeekPicker(date) {
    startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay());
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 7);

    // Set selected day to the start of the week for styling.
    datePickers["week"].datepicker("update", startDate);

    // Store saved date if we switch filtering.
    savedDate = date;

    // Display end date as one day less so it doesn't look like it's running into next week.
    let displayEndDate = new Date(
        endDate.getFullYear(),
        endDate.getMonth(),
        date.getDate() - date.getDay() + 6);

    // Show the display as the corresponding week.
    datePickers["week"].val(
        dateToString(startDate) +
        " - " +
        dateToString(displayEndDate));

    // Update the partial views.
    updatePage();
}

function setMonthPicker(date) {
    // Set day to 1 so we don't get results from last day of first month!
    startDate = new Date(date.getFullYear(), date.getMonth() + 0, 1);
    endDate = new Date(date.getFullYear(), date.getMonth() + 1, 1);

    datePickers["month"].datepicker("update", date);

    // Store saved date if we switch filtering.
    savedDate = date;

    // Show the display as the corresponding month.
    datePickers["month"].val(
        date.toLocaleString('default', { month: 'long' }) +
        " " +
        date.getFullYear());

    // Update the partial views.
    updatePage();
}

function setRangeStartPicker(date) {
    startDate = date;

    datePickers["rangeStart"].datepicker("update", date);

    datePickers["rangeStart"].val(dateToString(date));

    updatePage();
}

function setRangeEndPicker(date) {
    // Advance day by 1 to next day's midnight to grab any logs during the day.
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);

    datePickers["rangeEnd"].datepicker("update", date);

    datePickers["rangeEnd"].val(dateToString(date));

    updatePage();
}

// Return the date in a nice readable format.
function dateToString(date) {
    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
}

// ▄▀▄ ▀█▀ ▄▀▄ ▀▄▀
// █▀█ ▄█  █▀█ █ █

// Refresh the partial views for Index.
function updatePage() {
    // Don't send requests with null dates.
    if (!startDate || !endDate) return;

    updateLogs();
    updateStats();
}

function updateLogs() {
    $.ajax({
        type: "GET",
        url: "/TimeLog/IndexLogs?startDate=" + startDate.toJSON() +
            "&endDate=" + endDate.toJSON() +
            "&empId=" + savedEmpId,
        success: function (view) {
            $("#indexLogsView").html(view);
            colorJobs();
        }
    });
}

function updateStats() {
    $.ajax({
        type: "GET",
        url: "/TimeLog/IndexStats?startDate=" + startDate.toJSON() +
            "&endDate=" + endDate.toJSON() +
            "&empId=" + savedEmpId +
            "&pieCount=" + pieFilter,
        success: function (view) {
            $("#indexStatsView").html(view);
            $("#pieChartNumSelect").val(pieFilter);
        }
    });
}