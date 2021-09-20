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
            empId: savedEmpId
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
            empId: savedEmpId,
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