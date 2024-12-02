// Initialize the OpenStreetMap using Leaflet
let map = L.map('map', {
    attributionControl: false,
    zoomControl: false,
    scrollWheelZoom: true,
    doubleClickZoom: true,
    touchZoom: true
}).setView([43.61575401901517, 7.07180936206396], 16);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {maxZoom: 19,}).addTo(map);

// Function to swap departure and arrival addresses
document.getElementById('swap-btn').addEventListener('click', function () {
    let departure = addressInput.value;
    addressInput.value = document.getElementById('arrival-address').value;
    document.getElementById('arrival-address').value = departure;
});

document.getElementById('itinary-search').addEventListener('click', function () {
    const arrivalPosition = JSON.parse(localStorage.getItem('arrivalPosition'));
    const departurePosition = JSON.parse(localStorage.getItem('departurePosition'));
    console.log(arrivalPosition.arrivalLat);
})


/* TODO Cacher la barre sur le coté
document.getElementById("close-tab").addEventListener(('click'), () => this.updateSideBarDisplay())

function updateSideBarDisplay() {
    const sideBar = document.getElementById("sidebar");
    sideBar.style.width = "0px";
}
 */