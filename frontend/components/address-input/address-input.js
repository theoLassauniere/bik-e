class AddressInput extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({mode: "open"});

        fetch("/frontend/components/address-input/address-input.html").then(async (response) => {
            let htmlContent = await response.text();
            let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
            this.shadowRoot.appendChild(templateContent.cloneNode(true));

            this.updatePlaceholder();
            // Call setup logic after the template is loaded
            this.setupLogic();

        });
    }

    setupLogic() {
        const searchButton = this.shadowRoot.getElementById('search-btn');
        const addressInput = this.shadowRoot.getElementById('departure-address');
        const suggestionsBox = this.shadowRoot.getElementById('suggestions');
        const arrivalAddress = [43.61575401901517, 7.07180936206396];
        const departureAddress = [43.61575401901517, 7.07180936206396];


        // Attach event listener to search button
        searchButton.addEventListener('click', () => this.searchAddress(addressInput.value).then(
            () => this.updateDepartureInputDisplay()
        ));
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
            div.className = "suggestion-item";

            // Rendre la suggestion cliquable
            div.addEventListener("click", () => {
                const input = this.shadowRoot.getElementById('departure-address');
                input.value = address.properties.label;

                // Déclencher un événement personnalisé
                let event = new CustomEvent("optionChosen", {
                    detail: { address },
                });
                document.dispatchEvent(event);

                // Masquer les suggestions
                suggestionsBox.classList.add('displayNone');
            });

            list.appendChild(div);
        });

        // Ajouter un écouteur global pour masquer les suggestions quand on clique à l'extérieur
        if (!this._outsideClickListener) {
            this._outsideClickListener = this.handleOutsideClick.bind(this);
            document.addEventListener("click", this._outsideClickListener);
        }
    }

    handleOutsideClick(event) {
        const suggestionsBox = this.shadowRoot.getElementById('suggestions');
        const input = this.shadowRoot.getElementById('departure-address');

        if (suggestionsBox && !suggestionsBox.contains(event.target) && !input.contains(event.target)) {
            suggestionsBox.classList.add('displayNone');
        }
    }

    updateDepartureInputDisplay(){
        const departureField = document.getElementById("departure-input");
        departureField.classList.add('displayBlock');
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

            const {lat, lon, name} = data[0];
            map.setView([lat, lon], 13);
            L.marker([lat, lon]).addTo(map).bindPopup(address).openPopup();

            const addressArrivalElement = document.getElementById("arrival-input");
            const addressDepartureElement = document.getElementById("arrival-input");
            if(addressArrivalElement.style.display !== "none"){
                addressArrivalElement.style.display = "none";
                document.getElementById("arrival-value").style.display = "block";
                document.getElementById("arrival-value").innerText = "To:" + name;
                const arrivalPosition = {arrivalLat: lat, arrivalLon: lon};
                localStorage.setItem('arrivalPosition', JSON.stringify(arrivalPosition));
            }
            else{
                addressDepartureElement.style.display = "none";
                document.getElementById("departure-value").style.display = "block";
                document.getElementById("departure-value").innerText = "From:" + name;
                const departurePosition= {departureLat: lat, departureLon: lon};
                localStorage.setItem('departurePosition', JSON.stringify(departurePosition));
            }


        } catch (error) {
            console.error('Erreur lors de la recherche de l\'adresse:', error);
            alert('Une erreur est survenue lors de la recherche.');
        }
    }

    updatePlaceholder() {
        const input = this.shadowRoot.getElementById('departure-address');
        const placeholder = this.getAttribute('placeholder') || "Let's go somewhere ?";
        input.setAttribute('placeholder', placeholder);
    }
}

customElements.define("app-address-input", AddressInput);
