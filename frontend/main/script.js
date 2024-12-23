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

const directionComponent = document.querySelector("directions-bubbles");
const loader = document.getElementById("loader");
directionComponent.classList.add('displayNone');
loader.classList.add('displayNone');

console.log(loader.classList.contains('displayNone'));

// Connect to the broker
client.onConnect = () => {
    console.log("Connected to ActiveMQ!");

    // Subscribe to the queue
    client.subscribe("/queue/itinerary", (message) => {
        if (message.body) {
            // Parse and display the JSON message
            const data = JSON.parse(message.body);
            const coordinates = data.Coordinates.map(coord => [coord[1], coord[0]]);
            const stations = JSON.parse(data.Stations);
            const segments = data.Segments;
            const allSteps = segments.map(segment => segment.steps).flat();
            const instructions = allSteps.map(step => step.instruction).flat();
            const distanceByStep = allSteps.map(step => step.distance).flat();

            const directionComponent = document.querySelector("directions-bubbles");
            directionComponent.classList.remove('displayNone');
            for (let i = 0; i < stations.length; i++) {
                const marker = L.marker([stations[i].latitude, stations[i].longitude]).addTo(map);
                marker.bindPopup("Station n°" + (i + 1));
            }

            const polyline = L.polyline(coordinates, {color: 'blue'}).addTo(map);
            map.fitBounds(polyline.getBounds());

            directionComponent.classList.remove('displayNone');
            directionComponent.simulate(instructions, distanceByStep);
            console.log("Received:", data);
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

document.getElementById("logo-container").addEventListener('click', () => {
    location.reload();
})

document.getElementById('itinary-search').addEventListener('click', function () {
    loader.classList.add('displayFlex');
    loader.classList.remove('displayNone');
    const arrivalPosition = JSON.parse(localStorage.getItem('arrivalPosition'));
    const departurePosition = JSON.parse(localStorage.getItem('departurePosition'));
    const url = "http://localhost:8733/Design_Time_Addresses/RestBikeMVP/Service1/getInstructions?"
    const originLatitude = "originLatitude=" + departurePosition.departureLat;
    const originLongitude = "originLongitude=" + departurePosition.departureLon;
    const destinationLatitude = "destinationLatitude=" + arrivalPosition.arrivalLat;
    const destinationLongitude = "destinationLongitude=" + arrivalPosition.arrivalLon;
    const et = "&";
    fetch(url + et + originLatitude + et + originLongitude + et + destinationLatitude + et + destinationLongitude)
        .then(res => {
            loader.classList.remove('displayFlex');
            loader.classList.add('displayNone');
        });
})