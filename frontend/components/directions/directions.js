// Example data for directions (to simulate GPS data or routing API results)
const exampleDirections = [
    { icon: "/frontend/assets/fleches-gauche.png", instruction: "Turn left onto Elm St.", length: "500m" },
    { icon: "/frontend/assets/fleches-droite.png", instruction: "Turn right onto Oak St.", length: "1.2km" },
    { icon: "/frontend/assets/fleches-avancer.png", instruction: "Continue on Main St.", length: "3km" },
];

// Function to update the directions bubble
function updateDirectionsBubble(direction) {
    // Find the template element and create a clone
    const template = document.getElementById("directions-bubble");
    const bubbleClone = template.content.cloneNode(true);

    // Update the icon
    const directionIcon = bubbleClone.querySelector("#direction-icon img");
    directionIcon.src = direction.icon;

    // Update the direction text
    const nextDirection = bubbleClone.querySelector("#next-direction");
    nextDirection.textContent = direction.instruction;

    // Update the length
    const lengthElement = bubbleClone.querySelector("#length");
    lengthElement.textContent = direction.length;

    // Append to the DOM (replace existing bubble)
    const container = document.getElementById("directions-container");
    container.innerHTML = ""; // Clear any existing bubbles
    container.appendChild(bubbleClone);
}

// Initial Setup: Create a container for the directions bubble
document.body.innerHTML += '<div id="directions-container"></div>';

// Simulate updating the directions every few seconds
let currentStep = 0;
setInterval(() => {
    if (currentStep < exampleDirections.length) {
        updateDirectionsBubble(exampleDirections[currentStep]);
        currentStep++;
    }
}, 3000); // Change directions every 3 seconds
