// Get ID of log to delete from the row to the modal.
$(".btn-delete").on("click", function () {
    console.log("clik");
    console.log($(".input-delete", $(this).parent()).val());
    $("#TimeLog_Id").val($(".input-delete", $(this).parent()).val());
});

$("body").on("click", "#guy", function () {
    console.log("yo");
});