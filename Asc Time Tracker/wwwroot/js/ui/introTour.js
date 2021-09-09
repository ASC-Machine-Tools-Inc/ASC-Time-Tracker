// TODO: run this only on first login
introJs().setOptions({
    showProgress: true,
    steps: [{
        title: 'Welcome',
        intro: 'Hello World!'
    },
    {
        element: document.querySelector(".sidebar"),
        intro: 'This <b>STEP</b> focuses on an image. <br/> We also used some HTML tags!'
    },
    {
        title: 'Farewell!',
        element: document.getElementById("addTimerBtn"),
        intro: "Adios!"
    }]
}).start();