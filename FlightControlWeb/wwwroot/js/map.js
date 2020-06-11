let map;
//let polylinePoints=[]
let isClicked = false;
let mapedClicked = false;
let polyline = undefined;
flightDic = {}
function drawMap() {
	 let mymap = L.map('mapid').setView([31.80, 35.20], 13);
       L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=INeUSkJ98XKYWli4tyyI', {
        attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
        }).addTo(mymap);
    map = mymap;
    map.on('click', onMapClick);
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
    //this is not new flight
    if (flight.flight_id in flightDic) {
        flightDic[flight.flight_id].latitude = flight.latitude;
        flightDic[flight.flight_id].longitude = flight.longitude;
        let latlng = L.latLng(flight.latitude, flight.longitude);
        flightDic[flight.flight_id].icon.setLatLng(latlng);
    }
    //this is new flight
    else {
        let airplane = L.icon({
            iconUrl: 'img/Black-airplane-flight.svg',
            iconSize: [40, 40], // size of the icon
            iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
            //popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
        });
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
    //remove the airplane
    map.removeLayer(flightDic[id].icon);
    //remove from the dictionary
    delete flightDic[id];
    cleanPath();
}

function airplanClick(id) {
    cleanPath();
    //new Flight
    mapedClicked = false;
    let f=flightDic[id]
    let airplane = L.icon({
        iconUrl: 'img/Black-airplane-flight.svg',
        iconSize: [40, 40], // size of the icon
        iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
    });

    let RepAirplane = L.icon({
        iconUrl: 'img/blue-airplane-flight.svg',
        iconSize: [40, 40], // size of the icon
        iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
    });
    let row = document.getElementById(id);
    clickRow(row, false);
    f.icon.setIcon(RepAirplane);
    let keys = Object.keys(flightDic);
    for (let i = 0; i < keys.length; i++) {
        if ((flightDic[keys[i]].isPressed = true) && (flightDic[keys[i]]!=f)) {
            flightDic[keys[i]].isPressed = false;
            flightDic[keys[i]].icon.setIcon(airplane);
        }
    }
    f.isPressed = true;

}

function drawPath(segments,lat,lon) {
    //console.log(segments);
    polylinePoints = [];
    polylinePoints.push([lat, lon]);
    let i = 0
    for (i = 0; i < segments.length;i++) {
        let x = segments[i].latitude;
        let y = segments[i].longitude;
        polylinePoints.push([x, y]);
    }
    polyline = L.polyline(polylinePoints).addTo(map);
    //console.log(polyline);
    /*polylinePoints.forEach(function (polylinePoints) {
        new L.Marker(polylinePoints).addTo(map);
    });*/
}

function cleanPath() {
    if (polyline != undefined) {
        map.removeLayer(polyline);
        polyline = undefined;
    }
}

function onMapClick(e) {
    if (mapedClicked === false) {
        cleanPath();
        cleanFlightDetails();
        let keys = Object.keys(flightDic);

        let airplane = L.icon({
            iconUrl: 'img/Black-airplane-flight.svg',
            iconSize: [40, 40], // size of the icon
            iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
        });
        for (let i = 0; i < keys.length; i++) {
            if (flightDic[keys[i]].isPressed = true) {
                flightDic[keys[i]].isPressed = false;
                flightDic[keys[i]].icon.setIcon(airplane);
            }

            if (idCol != -1) { //to found if any row is in color
                let rowCol = document.getElementById(idCol);
                if (rowCol != null) {
                    rowCol.style.backgroundColor = "lightgray";
                }
                idCol = -1;
            }
        }
        mapedClicked = true
    }
}