class AddressInput extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({mode: "open"});

        fetch("/components/address-input/address-input.html").then(async (response) => {
            let htmlContent = await response.text();
            let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
            this.shadowRoot.appendChild(templateContent.cloneNode(true));

            // Call setup logic after the template is loaded
            this.setupLogic();
        });
    }

    setupLogic() {
        const searchButton = this.shadowRoot.getElementById('search-btn');
        const addressInput = this.shadowRoot.getElementById('departure-address');
        const suggestionsBox = this.shadowRoot.getElementById('suggestions');


        // Attach event listener to search button
        searchButton.addEventListener('click', () => this.searchAddress(addressInput.value));
        suggestionsBox.classList.add('displayNone');

        addressInput.addEventListener("keydown", (e) => {
            if (this.callTimeout) clearTimeout(this.callTimeout);

            this.callTimeout = setTimeout(() => {
                let inputContent = addressInput.value.replaceAll(" ", "+");
                let url = `https://api-adresse.data.gouv.fr/search/?q=${inputContent}&limit=5`;
                fetch(url)
                    .then(response => response.json())
                    .then(data => this.updateAutoCompleteList(data.features));
            }, 700);
        });
    }

    updateAutoCompleteList(addresses) {
        const suggestionsBox = this.shadowRoot.getElementById('suggestions');
        suggestionsBox.classList.remove('displayNone');
        const list = this.shadowRoot.getElementById('address-list');
        list.innerHTML = "";

        addresses.forEach((address) => {
            let div = document.createElement("div");
            div.textContent = address.properties.label;

            div.addEventListener("click", () => {
                let event = new CustomEvent("optionChosen", {
                    detail: {address},
                });
                document.dispatchEvent(event);
            });

            list.appendChild(div);
        });
    }

    async searchAddress(address) {
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

            const {lat, lon} = data[0];
            map.setView([lat, lon], 13);
            L.marker([lat, lon]).addTo(map).bindPopup(address).openPopup();

        } catch (error) {
            console.error('Erreur lors de la recherche de l\'adresse:', error);
            alert('Une erreur est survenue lors de la recherche.');
        }
    }
}

customElements.define("app-address-input", AddressInput);
