let map;
let isClicked = false;
flightDic = {}
function drawMap() {
	 let mymap = L.map('mapid').setView([31.80, 35.20], 13);
       L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=INeUSkJ98XKYWli4tyyI', {
        attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
        }).addTo(mymap);
        map=mymap;
}
class Flight {
    constructor(flight) {
        this.id = flight.flight_id;
        this.longitude = flight.longitude;
        this.latitude = flight.latitude;
        this.is_external = flight.is_external;
        this.passengers = flight.passengers;
        this.data_time = flight.data_time;
        this.company_name = flight.company_name;
        this.icon;
        this.isPressed = false;
    }
}

function drawFlight(flight) {
    let airplane = L.icon({
        iconUrl: 'img/Plane_icon.svg',
        iconSize: [40, 40], // size of the icon
        iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
        //popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    //this is not new flight
    if (flight.flight_id in flightDic) {
        flightDic[flight.flight_id].latitude = flight.latitude;
        flightDic[flight.flight_id].longitude = flight.longitude;
        let latlng = L.latLng(flight.latitude, flight.longitude);
        flightDic[flight.flight_id].icon.setLatLng(latlng);
    }
    //this is new flight
    else {
        let f = new Flight(flight);
        flightDic[flight.flight_id] = f;
        flightDic[flight.flight_id].icon = L.marker([flight.longitude, flight.latitude],
            { icon: airplane }).addTo(map)
        flightDic[flight.flight_id].icon.addEventListener('click', () => {
            clickFlight(flight.flight_id);
        }, false);
    }
}


function removeFromMap(id) {
    map.removeLayer(flightDic[id].icon);
    delete flightDic[id];
}

function airplanClick(id) {
    //new Flight
    let f=flightDic[id]
    let airplane = L.icon({
        iconUrl: 'img/Plane_icon.svg',
        iconSize: [40, 40], // size of the icon
        iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
        //popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let RepAirplane = L.icon({
        iconUrl: 'img/Thesquid.ink-Free-Flat-Sample-Space-rocket.svg',
        iconSize: [40, 40], // size of the icon
        iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
    });
    //console.log(f);
    let row=document.getElementById(id);
    //console.log(row);
    clickRow(row, false);
    f.icon.setIcon(RepAirplane);
    let keys = Object.keys(flightDic);
    for (let i = 0; i < keys.length; i++) {
        if ((flightDic[keys[i]].isPressed = true) && (flightDic[keys[i]]!=f)) {
            flightDic[keys[i]].isPressed = false;
            //console.log(flightDic[keys[i]].icon)
            flightDic[keys[i]].icon.setIcon(airplane);
        }
    }
    f.isPressed = true;
}