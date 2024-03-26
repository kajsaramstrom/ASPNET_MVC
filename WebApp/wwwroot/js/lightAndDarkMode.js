var isDarkMode = localStorage.getItem("darkMode") === "true" ? true : false;
var logoElement = document.getElementById("logo");
var logoImage = "/images/logotypes/solid.svg";
logoElement.innerHTML = "<img src='" + logoImage + "' alt='Silicon Logo'>";
var notFoundElement = document.getElementById("errorImage");
var notFoundImage = isDarkMode ? "/images/images/404darkmode.svg" : "/images/images/error404.svg";
var workWithUsElement = document.getElementById("workwithus");
var workWithUsImage = "/images/images/workwithus-image.svg";
workWithUsImage = isDarkMode ? "/images/images/workwithus-image-darkmode.svg" : "/images/images/workwithus-image.svg";
workWithUsElement.innerHTML = "<img src='" + workWithUsImage + "' alt='work with us image'>";
var desktopSwitch = document.getElementById("switch");
var mobileSwitch = document.getElementById("switch-mobile");
desktopSwitch.checked = isDarkMode;
mobileSwitch.checked = isDarkMode;


function toggleBothSwitches() {
    isDarkMode = !isDarkMode;
    localStorage.setItem("darkMode", isDarkMode);


    if (isDarkMode) {
        activateDarkMode();
    } else {
        deactivateDarkMode();
    }

    updateLogo();
    update404();
    workWithUs();
}


function activateDarkMode() {
    document.body.classList.add("dark-mode");
}

function deactivateDarkMode() {
    document.body.classList.remove("dark-mode");
}


function updateLogo() {
    logoImage = isDarkMode ? "/images/images/logo-header-darkmode.svg" : "/images/logotypes/solid.svg";
    logoElement.innerHTML = "<img src='" + logoImage + "' alt='Silicon Logo'>";
}

function update404() {
    notFoundImage = isDarkMode ? "/images/images/404darkmode.svg" : "/images/images/error404.svg";
    notFoundElement.innerHTML = "<img src='" + notFoundImage + "' alt='Page not found image'>";
}

function workWithUs() {
    workWithUsImage = isDarkMode ? "/images/images/workwithus-image-darkmode.svg" : "/images/images/workwithus-image.svg";
    workWithUsElement.innerHTML = "<img src='" + workWithUsImage + "' alt = 'work with us image'>"
}
window.onload = function () {
    if (isDarkMode) {
        activateDarkMode();
    }
    else {
        deactivateDarkMode();
    }
    updateLogo();
    update404();
    workWithUs();
}