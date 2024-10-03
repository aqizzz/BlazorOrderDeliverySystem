let directionsService;
let directionRenderer;
let directionsRenderers = [];
//let routePoints = [];
let workerMarker;
let map;
let connection;
let mapInitialized = false;
let mapReadyPromise;

function initMap(startLat, startLng, endLat, endLng, orderId) {
    mapReadyPromise = new Promise((resolve, reject) => {
        console.log(startLat, startLng, "to: ", endLat, endLng, orderId);
        const mapOptions = {
            zoom: 16,
            center: { lat: startLat, lng: startLng },
        };
        map = new google.maps.Map(document.getElementById('map'), mapOptions);

        directionsService = new google.maps.DirectionsService();
        directionsRenderer = new google.maps.DirectionsRenderer({
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'blue',
                strokeOpacity: 0.7,
                strokeWeight: 5
            }
        });
        directionsRenderer.setMap(map);

        addMarkers(startLat, startLng, endLat, endLng);

        displayRoute(startLat, startLng, endLat, endLng);

        mapInitialized = true;
        resolve();
    });
}

function addMarkers(startLat, startLng, endLat, endLng) {
    const merchantMarker = new google.maps.Marker({
        position: { lat: startLat, lng: startLng },
        map: map,
        title: "Merchant Location",
        icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png'
    });

    const customerMarker = new google.maps.Marker({
        position: { lat: endLat, lng: endLng },
        map: map,
        title: "Customer Location",

    });
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

    directionsService.route(request, (result, status) => {
        if (status === 'OK') {
            directionsRenderer.setDirections(result);
            //const route = result.routes[0];
            //routePoints = route.legs[0].steps.map(step => step.start_location);
            //routePoints.push(route.legs[0].end_location);
            //startSimulatingMovement();
        } else {
            console.error("Directions request failed due to " + status);
        }
    });
}
function displayRouteWithWorker(lat, lng, startLat, startLng, endLat, endLng) {
    const worker = new google.maps.LatLng(lat, lng);
    const merchant = new google.maps.LatLng(startLat, startLng);
    const end = new google.maps.LatLng(endLat, endLng);

    const request = {
        origin: worker,
        destination: end,
        waypoints: [{ location: merchant }],
        travelMode: 'DRIVING',
        provideRouteAlternatives: true
    };

    const infoWindow = new google.maps.InfoWindow();

    directionsService.route(request, (result, status) => {
        if (status === 'OK') {
            clearExistingRoutes();
            //directionsRenderer.setDirections(result);
            //const route = result.routes[0];
            //routePoints = route.legs[0].steps.map(step => step.start_location);
            //routePoints.push(route.legs[0].end_location);
            //startSimulatingMovement();
            result.routes.forEach((route, index) => {
                const directionsRenderer = new google.maps.DirectionsRenderer({
                    map: map,
                    directions: result,
                    routeIndex: index,
                    polylineOptions: {
                        strokeColor: getRouteColor(index),
                        strokeOpacity: 0.7,
                        strokeWeight: 5
                    }
                });
                directionsRenderers.push(directionsRenderer);

                const routeInfo = route.legs[0];
                const travelTime = routeInfo.duration.text;
                const distance = routeInfo.distance.text;
                console.log(`Route ${index + 1}: Time - ${travelTime}, Distance - ${distance}`);

                const content = `Route ${index + 1}:<br>Time - ${travelTime}<br>Distance - ${distance}`;

                const position = route.legs[0].start_location;

                infoWindow.setContent(content);
                infoWindow.setPosition(position);
                infoWindow.open(map); 
            });
        } else {
            console.error("Worker Directions request failed due to " + status);
        }
    });
}
function clearExistingRoutes() {
    directionsRenderers.forEach(renderer => renderer.setMap(null));
    directionsRenderers = [];
}

function getRouteColor(index) {
    const colors = ['blue', 'green', 'orange', 'purple', 'red'];
    return colors[index % colors.length];
}
function updateWorkerLocation(lat, lng, startLat, startLng, endLat, endLng) {
    mapReadyPromise.then(() => {
        console.log("Worker: ", lat, lng);
        console.log("UpdateWorkerLocation: ", startLat, startLng, "to: ", endLat, endLng);
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
        /*if (routePoints.length === 0) return; 
    
        const traveled = google.maps.geometry.spherical.computeDistanceBetween(routePoints[0], position);
        const total = calculateTotalRouteDistance();
        const progress = traveled / total;
    
        const newLocationIndex = Math.min(Math.floor(progress * (routePoints.length - 1)), routePoints.length - 1);
        const newLocation = routePoints[newLocationIndex];*/

        workerMarker.setPosition(position);
        if (map) {
            map.panTo(position);
        } else {
            console.error("Map not initialized!")
        }
        displayRouteWithWorker(lat, lng, startLat, startLng, endLat, endLng);
    }).catch(err => {
        console.error("Error initializing map: ", err);
    });
}
function calculateTotalRouteDistance() {
    let total = 0;

    for (let i = 0; i < routePoints.length - 1; i++) {
        total += google.maps.geometry.spherical.computeDistanceBetween(routePoints[i], routePoints[i + 1]);
    }
    return total;
}

/*function startSimulatingMovement() {
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
}*/

//For Customer Side
function setUpSignalR(orderId, startLat, startLng, endLat, endLng) {

    console.log("setUpSignalR: ", startLat, startLng, "to: ", endLat, endLng);
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/orderTrackingHub")
        .build();

    connection.on("ReceiveLocationUpdate", (latitude, longitude) => {
        updateWorkerLocation(latitude, longitude, startLat, startLng, endLat, endLng);
    });

    connection.start().then(() => {
        console.log("SignalR connection established for customer.", orderId);
        connection.invoke("TrackOrder", orderId)
    }).catch(err => console.error(err.toString()));
}
//For Worker Side
function setUpWorkerSignalR(orderId) {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/orderTrackingHub")
        .build();

    connection.start().then(() => {
        console.log("SignalR connection established for worker.", orderId);
    }).catch(err => console.error(err.toString()));
}
function startWorkerTracking(orderId, startLat, startLng, endLat, endLng) {
    console.log("startWorkerTracking: ", startLat, startLng, "to: ", endLat, endLng);
    localStorage.setItem('isSharingLocation', 'true');
    localStorage.setItem('OrderId', orderId);

    localStorage.setItem('startLatitude', startLat);
    localStorage.setItem('startLongitude', startLng);
    localStorage.setItem('endLatitude', endLat);
    localStorage.setItem('endLongitude', endLng);

    if (navigator.geolocation) {
            navigator.geolocation.watchPosition((position) => {
                const lat = position.coords.latitude;
                const lng = position.coords.longitude;

                localStorage.setItem('lastLatitude', lat);
                localStorage.setItem('lastLongitude', lng);

                updateWorkerLocation(lat, lng, startLat, startLng, endLat, endLng);

                console.log("orderId:", orderId, "latitude:", lat, "longitude:", lng);
                if (connection) {
                    connection.invoke("UpdateWorkerLocation", orderId, lat, lng)
                        .catch(err => console.error(err.toString()));
                }
            }, (error) => {
                console.error("Geolocation error: ", error.message);
            }, { enableHighAccuracy: true });
        } else {
            console.error("Geolocation is not supported by this browser.");
        }
    }

    document.addEventListener('DOMContentLoaded', () => {
        const isSharingLocation = localStorage.getItem('isSharingLocation');
        if (isSharingLocation === 'true') {
            const orderId = localStorage.getItem('OrderId');
            
            const lastLatitude = localStorage.getItem('lastLatitude');
            const lastLongitude = localStorage.getItem('lastLongitude');

            const startLatitude = localStorage.getItem('startLatitude');
            const startLongitude = localStorage.getItem('startLongitude');
            const endLatitude = localStorage.getItem('endLatitude');
            const endLongitude = localStorage.getItem('endLongitude');

            if (lastLatitude && lastLongitude && startLatitude && startLongitude && endLatitude && endLongitude) {
                startWorkerTracking(orderId, parseFloat(startLatitude), parseFloat(startLongitude), parseFloat(endLatitude), parseFloat(endLongitude));
            } else {
                console.log("No last known coordinates found.");
            }
        }
    });

    function stopTracking(orderId, startLat, startLng, endLat, endLng) {
        if (connection) {
            connection.invoke("StopTrackingOrder", orderId).catch(err => console.error(err.toString()));
        }
        initMap(startLat, startLng, endLat, endLng, orderId)
        localStorage.removeItem('isSharingLocation');
        localStorage.removeItem('OrderId');
        localStorage.removeItem('lastLatitude');
        localStorage.removeItem('lastLongitude');

        localStorage.removeItem('startLatitude');
        localStorage.removeItem('startLongitude');
        localStorage.removeItem('endLatitude');
        localStorage.removeItem('endLongitude');
    }
