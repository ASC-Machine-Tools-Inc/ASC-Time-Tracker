// Handle creating a time log and show the spinner while it's being added.
$("#timeLogForm").on("submit", function (e) {
    e.preventDefault();

    var form = $(this);
    var url = form.attr('action');

    $.ajax({
        type: "POST",
        url: url,
        data: form.serialize(),
        beforeSend: function () {
            // Show loading symbol.
            document.getElementById("timeLogCreateSpinner").style.display = "inline-block";
        },
        success: function () {
            document.getElementById("timeLogCreateSpinner").style.display = "none";
            $("#timeLogSubmitModal").modal("hide");
            updatePage();

            // Reset form fields.
            form.each(function () {
                this.reset();
            });
        }
    });
});