// Initialize the OpenStreetMap using Leaflet
var map = L.map('map').setView([48.8566, 2.3522], 12);  // Centered on Paris by default

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
}).addTo(map);

// Sample data for "Home" and "Work" addresses
const homeAddress = "123 Home St, City";
const workAddress = "456 Work Blvd, City";

// Function to swap departure and arrival addresses
document.getElementById('swap-btn').addEventListener('click', function() {
    let departure = document.getElementById('departure-address').value;
    let arrival = document.getElementById('arrival-address').value;

    // Swap the values
    document.getElementById('departure-address').value = arrival;
    document.getElementById('arrival-address').value = departure;
});

// Autofill "Home" address when the HOME button is clicked
document.getElementById('home-btn').addEventListener('click', function() {
    document.getElementById('departure-address').value = homeAddress;
});

// Autofill "Work" address when the WORK button is clicked
document.getElementById('work-btn').addEventListener('click', function() {
    document.getElementById('departure-address').value = workAddress;
});

// Autofill saved addresses when saved buttons are clicked
document.getElementById('saved1-btn').addEventListener('click', function() {
    document.getElementById('arrival-address').value = "19 rue Example, City";
});

document.getElementById('saved2-btn').addEventListener('click', function() {
    document.getElementById('arrival-address').value = "2155 rue Example, City";
});

document.getElementById('saved3-btn').addEventListener('click', function() {
    document.getElementById('arrival-address').value = "67 rue Example, City";
});
