var dayPicker, weekPicker, monthPicker,
    rangePickers, rangeStartPicker, rangeEndPicker,
    currPicker;
var startDate, endDate, savedDate;

$(document).ready(function () {
    // Prep the datepicker for days.
    dayPicker = $(".day-picker");
    dayPicker.datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#dayContainer",
    }).on("changeDate", function (e) {
        setDayPicker(e.date);
    });

    // Prep the datepicker for weeks.
    weekPicker = $(".week-picker");
    weekPicker.hide();
    weekPicker.datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#weekContainer"
    }).on("changeDate", function (e) {
        setWeekPicker(e.date);
    });

    // Prep the datepicker for months.
    monthPicker = $(".month-picker");
    monthPicker.hide();
    monthPicker.datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#monthContainer",
        startView: "months",
        minViewMode: "months"
    }).on("changeDate", function (e) {
        setMonthPicker(e.date);
    });

    // Prep the custom range picker.
    rangePickers = $("#dateRangePicker");
    rangePickers.hide();
    rangeStartPicker = $(".range-picker-start");
    rangeEndPicker = $(".ranger-picker-end");
    rangeStartPicker.datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#rangeStartContainer"
    }).on("changeDate", function (e) {
        setRangeStartPicker(e.date);
    });
    rangeEndPicker.datepicker({
        autoclose: true,
        forceParse: false,
        todayBtn: "linked",
        container: "#rangeEndContainer"
    }).on("changeDate", function (e) {
        setRangeEndPicker(e.date);
    });

    // Initialize.
    currPicker = dayPicker;
    setDayPicker(new Date());
});

// Event handlers for shifting the current date.
$(".date-prev").on("click", function () {
    var prev = new Date(startDate.getTime());

    // Set the first range picker if we're using those.
    if (currPicker === rangePickers) {
        setRangeStartPicker(next);
        return;
    }

    prev.setDate(prev.getDate() - 1);
    shiftDate(prev);
});

$(".date-next").on("click", function () {
    var next = new Date(endDate.getTime());

    // Set the second range picker if we're using those.
    if (currPicker === rangePickers) {
        setRangeEndPicker(next);
        return;
    }

    // Don't add one if we're using the dayPicker,
    // since its endDate is already one ahead..
    if (currPicker !== dayPicker) {
        next.setDate(next.getDate() + 1);
    }

    shiftDate(next);
});

// Update the date when we switch the filter.
$("#dateOption").on("change", function () {
    // Show the arrows to switch (All will hide them if selected).
    $(".date-prev").show();
    $(".date-next").show();

    switch (this.value) {
        case "Day":
            if (savedDate) setDayPicker(savedDate);

            dayPicker.show();
            weekPicker.hide();
            monthPicker.hide();
            rangePickers.hide();

            currPicker = dayPicker;
            break;
        case "Week":
            if (savedDate) setWeekPicker(savedDate);

            dayPicker.hide();
            weekPicker.show();
            monthPicker.hide();
            rangePickers.hide();

            currPicker = weekPicker;
            break;
        case "Month":
            if (savedDate) setMonthPicker(savedDate);

            dayPicker.hide();
            weekPicker.hide();
            monthPicker.show();
            rangePickers.hide();

            currPicker = monthPicker;
            break;
        case "All":
            dayPicker.hide();
            weekPicker.hide();
            monthPicker.hide();
            rangePickers.hide();
            $(".date-prev").hide();
            $(".date-next").hide();

            // Get all logs from epoch to 5138 (if they enter logs for the future).
            // Should be fine, right? If anyone sees this software from the future
            // because it's broken I apologize.
            startDate = new Date(0);
            endDate = new Date(100000000000000);
            updatePage();
            break;
        case "Custom range":
            dayPicker.hide();
            weekPicker.hide();
            monthPicker.hide();
            rangePickers.show();

            currPicker = rangePickers;
            break;
    }
});

// Called by the prev and next buttons to change the date.
function shiftDate(date) {
    if (currPicker === dayPicker) {
        setDayPicker(date);
    } else if (currPicker === weekPicker) {
        setWeekPicker(date);
    } else if (currPicker === monthPicker) {
        setMonthPicker(date);
    }
}

function setDayPicker(date) {
    startDate = date;
    // Advance day by 1 to next day's midnight to grab any logs during the day.
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);

    dayPicker.datepicker("update", date);

    savedDate = date;

    dayPicker.val(dateToString(date));

    // Update the partial views.
    updatePage();
}

function setWeekPicker(date) {
    startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay());
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 6);

    // Set selected day to the start of the week for styling.
    weekPicker.datepicker("update", startDate);

    // Store saved date if we switch filtering.
    savedDate = date;

    // Show the display as the corresponding week.
    weekPicker.val(
        dateToString(startDate) +
        " - " +
        dateToString(endDate));

    // Update the partial views.
    updatePage();
}

function setMonthPicker(date) {
    // Set day to 1 so we don't get results from last day of first month!
    startDate = new Date(date.getFullYear(), date.getMonth() + 0, 1);
    endDate = new Date(date.getFullYear(), date.getMonth() + 1, 1);

    monthPicker.datepicker("update", date);

    // Store saved date if we switch filtering.
    savedDate = date;

    // Show the display as the corresponding month.
    monthPicker.val(
        date.toLocaleString('default', { month: 'long' }) +
        " " +
        date.getFullYear());

    // Update the partial views.
    updatePage();
}

function setRangeStartPicker(date) {
    startDate = date;

    rangeStartPicker.datepicker("update", date);

    rangeStartPicker.val(dateToString(date));

    updatePage();
}

function setRangeEndPicker(date) {
    // Advance day by 1 to next day's midnight to grab any logs during the day.
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);

    rangeEndPicker.datepicker("update", date);

    rangeEndPicker.val(dateToString(date));

    updatePage();
}

// Refresh the partial views for Index.
function updatePage() {
    // Don't send requests with null dates.
    if (!startDate || !endDate) return;

    $.ajax({
        type: "GET",
        url: "TimeLog/_IndexLogs?startDate=" + startDate.toJSON() + "&endDate=" + endDate.toJSON(),
        success: function (view) {
            $("#indexLogsView").html(view);
            colorJobs();
        }
    });

    $.ajax({
        type: "GET",
        url: "TimeLog/_IndexStats?startDate=" + startDate.toJSON() + "&endDate=" + endDate.toJSON(),
        success: function (view) {
            $("#indexStatsView").html(view);
        }
    });
}

// Return the date in a nice readable format.
function dateToString(date) {
    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
}