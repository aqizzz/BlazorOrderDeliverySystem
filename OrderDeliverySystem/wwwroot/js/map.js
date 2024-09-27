let directionsService;
let directionsRenderer;
function initMap(startLat, startLng, endLat, endLng) {
    const mapOptions = {
        zoom: 16,
        center: { lat: startLat, lng: startLng },
    };
    const map = new google.maps.Map(document.getElementById('map'), mapOptions);

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
}

function displayRoute(startLat, startLng, endLat, endLng) {

    //console.log(startLat, startLng, "to: ", endLat, endLng);

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
        } else {
            console.error("Directions request failed due to " + status);
        }
    });
}