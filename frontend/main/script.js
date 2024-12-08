// ActiveMQ Connection details
const brokerURL = "ws://localhost:61614/stomp"; // ActiveMQ WebSocket endpoint

// Create a STOMP client
const client = new StompJs.Client({
    brokerURL: brokerURL,
    connectHeaders: {
        login: "admin",    // ActiveMQ username
        passcode: "admin", // ActiveMQ password
    },
    debug: () => {
    },
    reconnectDelay: 5000, // Auto-reconnect after 5 seconds
    heartbeatIncoming: 4000,
    heartbeatOutgoing: 4000,
});

// Connect to the broker
client.onConnect = () => {
    console.log("Connected to ActiveMQ!");

    // Subscribe to the queue
    client.subscribe("/queue/itinerary", (message) => {
        if (message.body) {
            // Parse and display the JSON message
            const jsonMessage = JSON.parse(message.body);
            console.log("Received:", jsonMessage);
        } else {
            console.log("Empty message received");
        }
    });
};

client.onStompError = (frame) => {
    console.error("Broker reported error:", frame.headers["message"]);
    console.error("Additional details:", frame.body);
};

// Activate the client
client.activate();

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
    fetch(url + et + originLatitude + et + originLongitude + et + destinationLatitude + et + destinationLongitude)
        .then(async (response) => response.json())
        .then(async (data) => {

            const coordinates = data.GetInstructionsResult.Coordinates.map(coord => [coord[1], coord[0]]);
            const stations = JSON.parse(data.GetInstructionsResult.Stations)

            for (let i = 0; i < stations.length; i++) {
                const marker = L.marker([stations[i].latitude, stations[i].longitude]).addTo(map);
                marker.bindPopup("Station n°" + i);
            }

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