/* Handles all the filtering for the index, updating the view
 * with ajax.
 */
var datePickers = {};
var currentPicker;  // For localstorage.

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

    loadSavedFilterData();
}

function startDatePickers() {
    // Prep the datepicker for days.
    datePickers["day"] = $(".day-picker");
    datePickers["day"].datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#dayContainer"
    }).on("changeDate", function (e) {
        setDayPicker(e.date);
    });

    // Prep the datepicker for weeks.
    datePickers["week"] = $(".week-picker");
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

/** Grab saved filter info from local storage. */
function loadSavedFilterData() {
    let filterData = localStorage["filterData"];
    if (filterData != null) {
        filterData = JSON.parse(filterData);

        // Update the saved dates.
        startDate = new Date(filterData.startDate);
        endDate = new Date(filterData.endDate);
        savedDate = new Date(filterData.savedDate);

        // Update the employee id.
        if (filterData.savedEmpId == null) {
            // Use user id as default one.
            savedEmpId = $("#empIdFilter").val();
        } else {
            savedEmpId = filterData.savedEmpId;
        }

        // Update the filters.
        pieFilter = parseInt(filterData.pieFilter);

        // Update the time frame and page.
        setCurrentPicker(filterData.currentPicker);
        $("#dateOption").val(filterData.currentPicker);
    }

    // TODO: update UI
}

/** Save current filters to local storage for retrieval. */
function saveFilterData() {
    let filterData = {
        "currentPicker": currentPicker,
        "startDate": startDate,
        "endDate": endDate,
        "savedDate": savedDate,
        "savedEmpId": savedEmpId,
        "pieFilter": pieFilter
    };

    localStorage["filterData"] = JSON.stringify(filterData);
}

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

/** Update the current picker. */
function setCurrentPicker(picker) {
    // Show the arrows to switch if choosing a time frame.
    $(".date-prev").show();
    $(".date-next").show();

    if (picker == null) {
        picker = "Day";
    }

    currentPicker = picker;
    switch (picker) {
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
            if (startDate) setRangeStartPicker(startDate);
            if (endDate) setRangeEndPicker(endDate);
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
}

function setDayPicker(date) {
    startDate = date;
    // Advance day by 1 to next day's midnight to grab any logs during the day.
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);

    datePickers["day"].datepicker("update", date);

    savedDate = date;

    datePickers["day"].val(dateToString(date));

    // Update the partial views.
    saveFilterData();
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
        date.getFullYear(),
        date.getMonth(),
        date.getDate() - date.getDay() + 6);

    // Show the display as the corresponding week.
    datePickers["week"].val(
        dateToString(startDate) +
        " - " +
        dateToString(displayEndDate));

    // Update the partial views.
    saveFilterData();
    updatePage();
}

function setMonthPicker(date) {
    // Set day to 1 so we don't get results from last day of first month!
    startDate = new Date(date.getFullYear(), date.getMonth(), 1);
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
    saveFilterData();
    updatePage();
}

function setRangeStartPicker(date) {
    startDate = date;

    datePickers["rangeStart"].datepicker("update", date);

    datePickers["rangeStart"].val(dateToString(date));

    saveFilterData();
    updatePage();
}

function setRangeEndPicker(date) {
    endDate = date;

    datePickers["rangeEnd"].datepicker("update", date);

    datePickers["rangeEnd"].val(dateToString(date));

    saveFilterData();
    updatePage();
}

// Return the date in a nice readable format.
function dateToString(date) {
    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
}