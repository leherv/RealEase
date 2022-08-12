window.addEventListener("load", (_) => {
    hideSpinner();
});

function hideSpinner() {
    let spinnerOverlayContainer = getSpinnerOverlayContainer();
    spinnerOverlayContainer.classList.add("d-none");
}

function showSpinner() {
    let spinnerOverlayContainer = getSpinnerOverlayContainer();
    spinnerOverlayContainer.classList.remove("d-none");
}

function getSpinnerOverlayContainer() {
    return document.getElementById("spinner-overlay-container");
}