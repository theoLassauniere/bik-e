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
    const url = "http://localhost:8733/Design_Time_Addresses/RestBikeMVP/Service1/getInstructions?"
    const originLatitude = "originLatitude=" + departurePosition.departureLat;
    const originLongitude = "originLongitude=" + departurePosition.departureLon;
    const destinationLatitude = "destinationLatitude=" + arrivalPosition.arrivalLat;
    const destinationLongitude = "destinationLongitude=" + arrivalPosition.arrivalLon;
    const et = "&";
    fetch("http://localhost:8733/Design_Time_Addresses/RestBikeMVP/Service1/getInstructions?originLatitude=45.583627118765456&originLongitude=4.886171291231583&destinationLatitude=45.95258312357312&destinationLongitude=4.715806540675931")//url + et + originLatitude + et + originLongitude + et + destinationLatitude + et + destinationLongitude)
        .then(async (response) => response.json())
        .then(async (data) => {

            console.log(data);
            const coordinates = data.GetInstructionsResult.Coordinates.map(coord => [coord[1], coord[0]]);
            console.log(coordinates.length);

            const polyline = L.polyline(coordinates, { color: 'blue' }).addTo(map);
            map.fitBounds(polyline.getBounds());
        }
    )
})



/* TODO Cacher la barre sur le coté
document.getElementById("close-tab").addEventListener(('click'), () => this.updateSideBarDisplay())

function updateSideBarDisplay() {
    const sideBar = document.getElementById("sidebar");
    sideBar.style.width = "0px";
}
 */