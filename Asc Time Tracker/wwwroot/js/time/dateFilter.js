var weekPicker, startDate, endDate;
var savedDay;

$(document).ready(function () {
    weekPicker = $(".week-picker");
    weekPicker.datepicker({
        autoclose: true,
        forceParse: false,
    }).on("changeDate", function (e) {
        setWeekPicker(e.date);
    });;

    $(".week-prev").on("click",
        function () {
            var prev = new Date(startDate.getTime());
            prev.setDate(prev.getDate() - 1);
            setWeekPicker(prev);
        });

    $(".week-next").on("click",
        function () {
            var next = new Date(endDate.getTime());
            next.setDate(next.getDate() + 1);
            setWeekPicker(next);
        });
    setWeekPicker(new Date);
});

function setWeekPicker(date) {
    startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay());
    endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 6);

    // Set selected day to the start of the week for styling.
    weekPicker.datepicker('update', startDate);

    // Store saved day if we switch back to filtering by day.
    savedDay = date;

    // Show the display as the corresponding week.
    weekPicker.val((startDate.getMonth() + 1) +
        "/" +
        startDate.getDate() +
        "/" +
        startDate.getFullYear() +
        " - " +
        (endDate.getMonth() + 1) +
        "/" +
        endDate.getDate() +
        "/" +
        endDate.getFullYear());
}