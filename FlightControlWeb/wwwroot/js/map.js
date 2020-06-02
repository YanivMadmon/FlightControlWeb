let map;
function drawMap() {
	 let mymap = L.map('mapid').setView([31.80, 35.20], 13);
       L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=INeUSkJ98XKYWli4tyyI', {
        attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
        }).addTo(mymap);
        map=mymap;
}


function drawFlight(flight) {
// add a marker in the given location
L.marker([31.80, 35.20]).addTo(map);
L.marker([flight.longitude, flight.latitude]).addTo(map);

var imageUrl = 'https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/Sydney_Opera_House_-_Dec_2008.jpg/1024px-Sydney_Opera_House_-_Dec_2008.jpg',
imageBounds = [center, [-35.8650, 154.2094]];

L.imageOverlay(imageUrl, imageBounds).addTo(map);
}
