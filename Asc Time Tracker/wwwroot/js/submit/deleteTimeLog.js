// Since the elements are dynamically generated with AJAX, we bind to the body
// instead and listen for any clicks on our delete buttons inside, instead of
// binding to them directly.
$("body").on("click", ".btn-delete", function () {
    // Get ID of log to delete from the row to the modal.
    $("#TimeLog_Id").val($(".timelog-id", $(this).parent()).val());
});

// Post to the TimeLogController to delete and refresh.
$("#deleteForm").submit(function (e) {
    e.preventDefault();

    let logId = $("#TimeLog_Id").val();

    $.ajax({
        type: "POST",
        url: $(this).attr("action"),
        data: {
            __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
            id: logId
        },
        success: function () {
            updatePage();
        }
    });
});