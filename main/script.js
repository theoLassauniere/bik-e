// Initialize the OpenStreetMap using Leaflet
let map = L.map('map').setView([43.61575401901517, 7.07180936206396], 16);  // Centered on Paris by default

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
}).addTo(map);

document.getElementById('search-btn').addEventListener('click', searchAddress);

// Function to swap departure and arrival addresses
document.getElementById('swap-btn').addEventListener('click', function () {
    let departure = document.getElementById('departure-address').value;
    // Swap the values
    document.getElementById('departure-address').value = document.getElementById('arrival-address').value;
    document.getElementById('arrival-address').value = departure;
});

async function searchAddress() {
    const address = document.getElementById('departure-address').value;
    if (!address) {
        alert('Veuillez entrer une adresse.');
        return;
    }

    try {
        const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}`);
        const data = await response.json();

        if (data.length === 0) {
            alert('Adresse non trouvée.');
            return;
        }

        // Récupération des coordonnées du premier résultat
        const {lat, lon} = data[0];

        // Centrer la carte sur les coordonnées trouvées
        map.setView([lat, lon], 13);

        // Ajouter un marqueur à l'emplacement
        L.marker([lat, lon]).addTo(map).bindPopup(address).openPopup();

    } catch (error) {
        console.error('Erreur lors de la recherche de l\'adresse :', error);
        alert('Une erreur est survenue lors de la recherche.');
    }
}
