// ▄▀▄ ▀█▀ ▄▀▄ ▀▄▀
// █▀█ ▄█  █▀█ █ █

const SHOW_LOADING_MESSAGE_TIMEOUT = 3000;

// Refresh the partial views for Index.
function updatePage() {
    // Don't send requests with null dates.
    if (!startDate || !endDate) return;

    updateLogs();
    updateStats();
}

function updateLogs() {
    let loadingTimeout;

    $.ajax({
        type: "GET",
        url: "/TimeLog/_IndexLogs",
        data: {
            startDate: startDate.toJSON(),
            endDate: endDate.toJSON(),
            empId: savedEmpIds
        },
        beforeSend: function () {
            // Display loading message if it takes a while.
            loadingTimeout = setTimeout(function () {
                $("#logsLoadingMessage").removeClass("d-none");
                $("#timeLogs").hide();
            }, SHOW_LOADING_MESSAGE_TIMEOUT);
        },
        success: function (view) {
            clearTimeout(loadingTimeout);
            $("#indexLogsView").html(view);
            colorJobs();
        }
    });
}

function updateStats() {
    let loadingTimeout;

    $.ajax({
        type: "GET",
        url: "/TimeLog/_IndexStats",
        data: {
            startDate: startDate.toJSON(),
            endDate: endDate.toJSON(),
            empId: savedEmpIds,
            pieCount: pieFilter
        },
        beforeSend: function () {
            // Hide until load if it takes a while.
            loadingTimeout = setTimeout(function () {
                $("#indexStatsView").html("");
            }, SHOW_LOADING_MESSAGE_TIMEOUT);
        },
        success: function (view) {
            clearTimeout(loadingTimeout);
            $("#indexStatsView").html(view);
            $("#pieChartNumSelect").val(pieFilter);
        }
    });
}

// █   █▀█ █▀▀ ▄▀▄ █     █▀▀ ▀█▀ █▀█ █▀█ ▄▀▄ █▀▀ █▀▀
// █▄▄ █▄█ █▄▄ █▀█ █▄▄   ▄██  █  █▄█ █▀▄ █▀█ █▄█ ██▄

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
        savedEmpIds = new Set(filterData.savedEmpIds);
        console.log("ids:" + savedEmpIds);

        // Update the filters.
        pieFilter = parseInt(filterData.pieFilter);

        // Update the time frame and page.
        setCurrentPicker(filterData.currentPicker);
        $("#dateOption").val(filterData.currentPicker);
    } else {
        // Use default values.
        savedDate = new Date();

        // Use user id as default one.
        savedEmpIds.add($("#empIdFilter").val());

        // Default pie filter count.
        pieFilter = 5;

        setCurrentPicker("Day");
    }

    // TODO: load in filter data.
}

/** Save current filters to local storage for retrieval. */
function saveFilterData() {
    let filterData = {
        "currentPicker": currentPicker,
        "startDate": startDate,
        "endDate": endDate,
        "savedDate": savedDate,
        "savedEmpIds": savedEmpIds,
        "pieFilter": pieFilter
    };

    localStorage["filterData"] = JSON.stringify(filterData);
}