$("body").on("click", ".btn-edit", function () {
    // Move values from the time log card to the modal.
    $("#TimeLogEditId").val($(".timelog-id", $(this).parent()).val());
    $("#TimeLogEditJobNum").val($(".timelog-jobnum", $(this).parent()).val());
    $("#TimeLogEditDate").val($(".timelog-date", $(this).parent()).val());
    $("#TimeLogEditTime").val($(".timelog-time", $(this).parent()).val());
    $("#TimeLogEditNotes").val($(".timelog-notes", $(this).parent()).val());

    // Set checkbox to time log's value for research and design flag.
    var isRd = ($(".timelog-rd", $(this).parent()).val() === "True");
    $("#TimeLogEditRd").prop("checked", isRd);

    convertTime();
});

// Convert the stored time in seconds into hours and minutes for the input.
function convertTime() {
    let time = $('#TimeLogEditTime').val();

    let hours = Math.floor(time / 3600);
    let minutes = Math.floor((time % 3600) / 60);

    $('#timeHoursEdit').val(hours);
    $('#timeMinutesEdit').val(minutes);
};