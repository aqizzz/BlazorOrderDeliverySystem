let directionsService;
let directionsRenderer;
let routePoints = [];
let workerMarker;
let map;
let connection;

function initMap(startLat, startLng, endLat, endLng,orderId) {
    console.log(startLat, startLng, "to: ", endLat, endLng, orderId);
    const mapOptions = {
        zoom: 16,
        center: { lat: startLat, lng: startLng },
    };
    map = new google.maps.Map(document.getElementById('map'), mapOptions);

    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        polylineOptions: {
            strokeColor: 'blue',
            strokeOpacity: 0.7,
            strokeWeight: 5
        }
    });
    directionsRenderer.setMap(map);

    displayRoute(startLat, startLng, endLat, endLng);

    //setUpSignalR(orderId);

   
}

function displayRoute(startLat, startLng, endLat, endLng) {

    console.log(startLat, startLng, "to: ", endLat, endLng);

    const start = new google.maps.LatLng(startLat, startLng);
    const end = new google.maps.LatLng(endLat, endLng);

    const request = {
        origin: start,
        destination: end,
        travelMode: 'DRIVING',
    };

    directionsService.route(request, function (result, status) {
        if (status === 'OK') {
            directionsRenderer.setDirections(result);
            const route = result.routes[0];
            routePoints = route.legs[0].steps.map(step => step.start_location);
            routePoints.push(route.legs[0].end_location);
            startSimulatingMovement();
        } else {
            console.error("Directions request failed due to " + status);
        }
    });
}
function updateWorkerLocation(lat, lng) {
    const position = new google.maps.LatLng(lat, lng);

    if (!workerMarker) {
        workerMarker = new google.maps.Marker({
            position: position,
            map: map,
            title: "Delivery Worker"
        })
    } else {
        workerMarker.setPosition(position);
    }
    if (routePoints.length === 0) return; 

    const traveled = google.maps.geometry.spherical.computeDistanceBetween(routePoints[0], position);
    const total = calculateTotalRouteDistance();
    const progress = traveled / total;

    const newLocationIndex = Math.min(Math.floor(progress * (routePoints.length - 1)), routePoints.length - 1);
    const newLocation = routePoints[newLocationIndex];

    workerMarker.setPosition(newLocation);

    map.panTo(newLocation);
}
function calculateTotalRouteDistance() {
    let total = 0;

    for (let i = 0; i < routePoints.length - 1; i++) {
        total += google.maps.geometry.spherical.computeDistanceBetween(routePoints[i], routePoints[i + 1]);
    }
    return total;
}

function startSimulatingMovement() {
    if (routePoints.length === 0) return;

    let currentIndex = 0;
    if (!workerMarker) {
        workerMarker = new google.maps.Marker({
            position: routePoints[currentIndex],
            map: map,
            title: "Delivery Worker",
            icon: { url: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png' } 
        });
    } else {
        
        workerMarker.setPosition(routePoints[currentIndex]);
    }

     const simulationInterval = setInterval(() => {
        currentIndex++;
        if (currentIndex >= routePoints.length) {
            clearInterval(simulationInterval); 
            return;
        }

        workerMarker.setPosition(routePoints[currentIndex]);
        map.panTo(routePoints[currentIndex]); 

    }, 5000);
}

function setUpSignalR(orderId){
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/orderTrackingHub")
        .build();

    connection.on("ReceiveLocationUpdate", (latitude, longitude) => {
        updateWorkerLocation(latitude, longitude);
    });

    connection.start().then(() => {
        console.log("SignalR connection established.", orderId);
        connection.invoke("TrackOrder", orderId)
    }).catch(err => console.error(err.toString()));
}
function stopTrackingOrder(orderId) {
    if (connection) {
        connection.invoke("StopTrackingOrder", orderId).catch(err => console.error(err.toString()));
    }
}
