// Creates a random color to denote a certain job number. If none, defaults to red.
$('.log-color').each(function () {
    let saturation = '85%';
    let lightness = '50%';

    // Get hashcode from job number
    let jobNum = $(this).siblings('.log-job').html();
    let hash = 0;
    for (let i = 0; i < jobNum.length; i++) {
        hash = jobNum.charCodeAt(i) + ((hash << 5) - hash);
        hash = hash & hash; // Convert to 32bit integer
    }
    let hue = hash % 360;

    let hsl = 'hsl(' + hue + ',' + saturation + ',' + lightness + ')';
    $(this).css('background-color', hsl);
});