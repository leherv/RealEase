let timeoutId;

window.addEventListener("load", (_) => {
    hideSpinner();
});

function hideSpinner() {
    let spinnerOverlayContainer = getSpinnerOverlayContainer();
    clearTimeout(timeoutId);
    spinnerOverlayContainer.classList.add("d-none");
}

function showSpinner() {
    let spinnerOverlayContainer = getSpinnerOverlayContainer();
    timeoutId = setTimeout(() => spinnerOverlayContainer.classList.remove("d-none"), 500);
}

function getSpinnerOverlayContainer() {
    return document.getElementById("spinner-overlay-container");
}