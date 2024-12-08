class DirectionsBubble extends HTMLElement{
    constructor() {
        super();
        this.attachShadow({mode: "open"});

        fetch("/frontend/components/directions/directions.html").then(async (response) => {
            const htmlContent = await response.text();
            const templateContent = new DOMParser()
                .parseFromString(htmlContent, "text/html")
                .querySelector("template").content;
            this.shadowRoot.appendChild(templateContent.cloneNode(true));
        }).catch(err => console.error("Error loading template:", err));
    }


    updateDirectionsBubble(direction, distance) {
        const nextDirectionElement = this.shadowRoot.getElementById("next-direction");
        const lengthElement = this.shadowRoot.getElementById("length");

        if (nextDirectionElement && lengthElement) {
            nextDirectionElement.innerText = direction;
            lengthElement.innerText = distance + " m";
        } else {
            console.error("Elements not found in shadow DOM");
        }
    }


    simulate(directions, distances){
        let currentStep = 0;
        setInterval(() => {
            if (currentStep < directions.length) {
                this.updateDirectionsBubble(directions[currentStep], distances[currentStep]);
                currentStep++;
            }
        }, 3000); // Change directions every 3 seconds
    }

}



customElements.define("directions-bubbles", DirectionsBubble);