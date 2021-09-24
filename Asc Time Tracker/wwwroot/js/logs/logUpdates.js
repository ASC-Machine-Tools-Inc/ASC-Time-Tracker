// ▀█▀ █▄ █ ▀█▀ ▀█▀ ▀█▀ ▄▀▄ █   ▀█▀ ▀██ ▄▀▄ ▀█▀ ▀█▀ █▀█ █▄ █
// ▄█▄ █ ▀█ ▄█▄  █  ▄█▄ █▀█ █▄▄ ▄█▄ ██▄ █▀█  █  ▄█▄ █▄█ █ ▀█

function startPickers() {
    startDatePickers();

    loadSavedFilterData();
}

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
            empIds: empIdsFieldToSet(),
            startDate: startDate.toJSON(),
            endDate: endDate.toJSON(),
            category: savedCategory,
            jobNum: savedJobNum,
            notes: savedNotes,
            rd: savedRd
        },
        traditional: true,  // Needed to send employee IDs
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
            empIds: empIdsFieldToSet(),
            startDate: startDate.toJSON(),
            endDate: endDate.toJSON(),
            pieCount: pieFilter
        },
        traditional: true,  // Needed to send employee IDs
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
        savedEmpIds = new Set(JSON.parse(filterData.savedEmpIds));

        // Update the filters.
        savedCategory = filterData.savedCategory;

        savedJobNum = filterData.savedJobNum;

        savedNotes = filterData.savedNotes;

        savedRd = filterData.savedRd;

        pieFilter = parseInt(filterData.pieFilter);

        // Update the time frame and page.
        setCurrentPicker(filterData.currentPicker);

        if (updateUi) {
            $("#empIdFilter").val(empIdsFieldToSet());

            $("#categoriesFilter").val(savedCategory);

            $("#jobNumFilter").val(savedJobNum);

            $("#notesFilter").val(savedNotes);

            $("#researchCheck").prop("checked", savedRd);

            $("#dateOption").val(filterData.currentPicker);
        }
    } else {
        // Use default values.
        savedDate = new Date();

        // Use user id as default one.
        savedEmpIds = new Set();
        savedEmpIds.add($("#empIdFilter").val());

        // Default pie filter count.
        pieFilter = 5;

        setCurrentPicker("Day");

        saveFilterData();
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
        "savedEmpIds": JSON.stringify([...savedEmpIds]),  // Stringify set converted to array.
        "pieFilter": pieFilter,
        "savedCategory": savedCategory,
        "savedJobNum": savedJobNum,
        "savedNotes": savedNotes,
        "savedRd": savedRd
    };

    localStorage["filterData"] = JSON.stringify(filterData);
}