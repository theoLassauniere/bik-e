class DirectionsBubble extends HTMLElement{
    constructor() {
        super();
        this.attachShadow({mode: "open"});

        fetch("/frontend/components/directions/directions.html").then(async (response) => {
            let htmlContent = await response.text();
            let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
            this.shadowRoot.appendChild(templateContent.cloneNode(true));
        })
    }


    updateDirectionsBubble(direction) {
        // Find the template element and create a clone
        console.log(direction);
        const bubble = document.getElementById("directions-bubble");
        document.getElementById("direction-icon-image").src = direction.icon;
        document.getElementById("next-direction").innerText = direction.instruction;
        document.getElementById("length").innerText=direction.length;


        /*const template = document.getElementById("directions-bubble");
        const bubbleClone = template.content.cloneNode(true);

        // Update the icon
        const directionIcon = bubbleClone.getElementById("direction-icon-image");
        directionIcon.src = direction.icon;

        // Update the direction text
        const nextDirection = bubbleClone.getElementById("next-direction");
        nextDirection.textContent = direction.instruction;

        // Update the length
        const lengthElement = bubbleClone.getElementById("length");
        lengthElement.textContent = direction.length;

        document.body.innerHTML += '<div id="directions-container"></div>';
        // Append to the DOM (replace existing bubble)
        const container = document.getElementById("directions-bubble");
        container.innerHTML = ""; // Clear any existing bubbles
        container.appendChild(bubbleClone);*/
    }


    simulate(){
        // Example data for directions (to simulate GPS data or routing API results)
        const exampleDirections = [
            { icon: "/frontend/assets/fleches-gauche.png", instruction: "Turn left onto Elm St.", length: "500m" },
            { icon: "/frontend/assets/fleches-droite.png", instruction: "Turn right onto Oak St.", length: "1.2km" },
            { icon: "/frontend/assets/fleches-avancer.png", instruction: "Continue on Main St.", length: "3km" },
        ];

        // Simulate updating the directions every few seconds
        let currentStep = 0;
        setInterval(() => {
            if (currentStep < exampleDirections.length) {
                this.updateDirectionsBubble(exampleDirections[currentStep]);
                currentStep++;
            }
        }, 3000); // Change directions every 3 seconds
    }

}



customElements.define("directions-bubbles", DirectionsBubble);