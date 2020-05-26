let map;
function drawMap() {
	 let mymap = L.map('mapid').setView( [4, 120], 13);
       L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=INeUSkJ98XKYWli4tyyI', {
        attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
        }).addTo(mymap);
        map=mymap;
}


function drawFlight(flight) {
// add a marker in the given location
//L.marker(center).addTo(map);
//L.marker([-35.8650, 154.2094]).addTo(map);
console.log(flight.longitude)
var imageUrl = 'https://upload.wikimedia.org/wikipedia/commons/1/1e/Airplane_silhouette.png',
imageBounds = [[flight.longitude,flight.latitude], [flight.longitude-2,flight.latitude+3]];
//guglihlihl
L.imageOverlay(imageUrl, imageBounds).addTo(map);
}
