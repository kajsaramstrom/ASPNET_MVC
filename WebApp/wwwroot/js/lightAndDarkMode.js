var isDarkMode = false;
var logoElement = document.getElementById("logo");
var logoImage = "/images/silicon-logo.svg";
logoElement.innerHTML = "<img src='" + logoImage + "' alt='Silicon Logo'>";

function toggleBothSwitches() {
    isDarkMode = !isDarkMode;
    var desktopSwitch = document.getElementById("switch");
    desktopSwitch.checked = isDarkMode;


    var mobileSwitch = document.getElementById("switch-mobile");
    mobileSwitch.checked = isDarkMode;
    if (isDarkMode) {
        activateDarkMode();
    } else {
        deactivateDarkMode();
    }

    updateLogo();
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