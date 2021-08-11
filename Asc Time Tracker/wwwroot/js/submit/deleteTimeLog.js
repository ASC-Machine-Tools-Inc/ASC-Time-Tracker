// Get ID of log to delete from the row to the modal.
$('.btn-delete').on('click', function () {
    $('#TimeLog_ID').val($('.input-delete', $(this).parent()).val());
});