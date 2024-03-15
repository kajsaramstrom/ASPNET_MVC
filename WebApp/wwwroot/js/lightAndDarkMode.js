var isDarkMode = localStorage.getItem("darkMode") === "true" ? true : false;
var logoElement = document.getElementById("logo");
var logoImage = isDarkMode ? "/images/images/logo-header-darkmode.svg" : "/images/logotypes/solid.svg";
var notFoundElement = document.getElementById("errorImage");
var notFoundImage = isDarkMode ? "/images/images/404darkmode.svg" : "/images/images/error404.svg";
var desktopSwitch = document.getElementById("switch");
var mobileSwitch = document.getElementById("switch-mobile");

logoElement.innerHTML = "<img src='" + logoImage + "' alt='Silicon Logo'>";
desktopSwitch.checked = isDarkMode;
mobileSwitch.checked = isDarkMode;

// Funktion för att byta temaläge och uppdatera logotypen
function toggleBothSwitches() {
    isDarkMode = !isDarkMode;
    localStorage.setItem("darkMode", isDarkMode); // Spara användarens temainställning i localStorage

    desktopSwitch.checked = isDarkMode;
    mobileSwitch.checked = isDarkMode;

    if (isDarkMode) {
        activateDarkMode();
    } else {
        deactivateDarkMode();
    }

    updateLogo();
    update404();
}

// Funktion för att aktivera mörkt läge
function activateDarkMode() {
    document.body.classList.add("dark-mode");
}

// Funktion för att inaktivera mörkt läge
function deactivateDarkMode() {
    document.body.classList.remove("dark-mode");
}

// Funktion för att uppdatera logotypen baserat på temainställningen
function updateLogo() {
    logoImage = isDarkMode ? "/images/images/logo-header-darkmode.svg" : "/images/logotypes/solid.svg";
    logoElement.innerHTML = "<img src='" + logoImage + "' alt='Silicon Logo'>";
}

function update404() {
    notFoundImage = isDarkMode ? "/images/images/404darkmode.svg" : "/images/images/error404.svg";
    notFoundElement.innerHTML = "<img src='" + notFoundImage + "' alt='Silicon Error'>"
}

// Körs när sidan laddas för att sätta temaläget och uppdatera logotypen
window.onload = function () {
    if (isDarkMode) {
        activateDarkMode();
    } else {
        deactivateDarkMode();
    }
    updateLogo();
    update404();
};