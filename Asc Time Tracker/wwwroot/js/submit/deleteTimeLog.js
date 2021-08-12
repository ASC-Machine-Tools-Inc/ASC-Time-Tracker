// Get ID of log to delete from the row to the modal.
$('.btn-delete').on('click', function () {
    $('#TimeLog_Id').val($('.input-delete', $(this).parent()).val());
});