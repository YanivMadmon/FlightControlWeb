let thisFlight = "";
let idCol = -1;
let detailsShow = -1;
async function getFlights() {
    try {
        const currentTime = getCurrentTime();
        let url = 'http://rony9.atwebpages.com' + '/api/Flights?relative_to=' + currentTime;
        const response = await fetch(url);
        // get the flights
        let flightPlans = await response.json();
        // iterate
        IterateAllFlights(flightPlans);
        // update table
        $('#flight_table tbody').empty();
        $('#flight_table tbody').append(thisFlight);
        thisFlight = "";
    }
    catch (err) {
        console.log('problem in GetFlights' + err);
    }
}

function flightsTable() {
    setInterval(function () {
        getFlights();
    }, 1000);
}

function getCurrentTime() {
    let d = new Date();
    let currentTime = d.getFullYear() + "-" + ("00" + (d.getMonth() + 1)).slice(-2) +
        "-" + ("00" + d.getDate()).slice(-2) + "T" + ("00" + d.getHours()).slice(-2) +
        ":" + ("00" + d.getMinutes()).slice(-2) + ":" + ("00" + d.getSeconds()).slice(-2) + "Z";
    return currentTime;
}

function addFlights(flight) {
    let butt = "<button class=\"button button1\" onclick=\"removeFlight(this)\">x</button>"
    thisFlight += "<tr id=\"" + flight.flight_id + "\"";
    if (idCol === flight.flight_id) {
        thisFlight += "style=\"background-color: green;\" ";
    }
    else {
        thisFlight += "style=\"background-color: lightgray;\" ";
    }
    thisFlight +=" "+"onclick =\"clickRow(this,true)\"><td>" + flight.flight_id +
        "</td>" + "<td>" + flight.company_name +
        "</td>" + "<td>" + flight.is_external + "</td>" + "<td>" + butt + "</td></tr>";
    //console.log(thisFlight);
}



function removeFlight(row) {
    //console.log(row);
    let a = row.parentNode.parentNode;
    let idFlighet = a.cells[0].innerHTML;
    //console.log(idFlighet + " = " + idCol);
    if (idCol === idFlighet) {
        idCol = -1;
    }
    //console.log("new" + idCol);
    let url1 = "http://rony9.atwebpages.com/api/Flights/" + idFlighet;
    //console.log(url1);
    $.ajax({
        url: url1,
        type: 'DELETE',
        success: function (result) {
            console.log("remove succes");
        }
    });
    a.parentNode.removeChild(a);
    removeFromMap(idFlighet);
    if (detailsShow === idFlighet) {
        cleanFlightDetails();
    }
}

function IterateAllFlights(data) {
    data.forEach(function (flight) {
        $(flight).each(function (index, value) {
            addFlights(value);
            drawFlight(value);
        })
    })
}

function clickRow(row, isFromRowClick) {
    mapedClicked = false;
    if (row != null) {
        let flightId = row.cells[0].innerHTML;
        if (flightId in flightDic) {
            if (isFromRowClick) {
                clickFlight(flightId);
            }
            if ((idCol != -1) && (idCol != flightId)) { //to found if any row is in color
                let rowCol = document.getElementById(idCol);
                if (rowCol != null) {
                    rowCol.style.backgroundColor = "lightgray";
                }
            }
            row.style.backgroundColor = "green";
            idCol = flightId;
        }
    }
}

function clickFlight(id) {
    airplanClick(id);
    flightDetails(id);
}

function flightDetails(id) {
    $.getJSON("http://rony9.atwebpages.com/api/FlightPlan/" + id, function (data) {
        //console.log(data);
        detailsShow = id;
        //update passengers
        const pas = document.getElementById("passengers");
        pas.innerHTML = data["passengers"];

        //update comanpany name
        const comp = document.getElementById("company");
        comp.innerHTML = data["company_name"];

        //update initial location
        const initLocation = data["initial_location"]
        const initLat = document.getElementById("initialLat");
        initLat.innerHTML ="lat: " + initLocation["latitude"];
        const initLon = document.getElementById("initialLon");
        initLon.innerHTML = "lon: " + initLocation["longitude"];

        //update daparture
        const initTime = document.getElementById("departure");
        const dateFlight = new Date(initLocation["date_time"]);
        initTime.innerHTML = dateFlight.toUTCString();

        //update arrival time
        const segments = data["segments"];
        segments.unshift(initLocation);
        drawPath(segments, data["initial_location"]["latitude"], data["initial_location"]["longitude"]);
        let time = 0;
        //sum all the time seconds
        let i = 1;
        for (i = 1; i < segments.length; i++) {
            time += segments[i]["timespan_seconds"];
        }
        const arrive = document.getElementById("arrival");
        const arrDate = dateFlight;
        arrDate.setSeconds(dateFlight.getSeconds() + time);
        arrive.innerHTML = arrDate.toUTCString();

        //update final location
        const finalLat = document.getElementById("finalLat");
        finalLat.innerHTML = "lat: " + segments[i - 1]["latitude"];
        const finalLoc = document.getElementById("finalLon");
        finalLoc.innerHTML = "lon: " + segments[i - 1]["longitude"];

    }).fail(function (data) {
        errorHandle(data.status, "not find a flight plan");
    });
}

function cleanFlightDetails() {
    const comp = document.getElementById("company");
    comp.innerHTML = "";
    const pas = document.getElementById("passengers");
    pas.innerHTML = "";
    const initLat = document.getElementById("initialLat");
    initLat.innerHTML = "";
    const initLon = document.getElementById("initialLon");
    initLon.innerHTML = "";
    const finLon = document.getElementById("finalLon");
    finLon.innerHTML = "";
    const finLat = document.getElementById("finalLat");
    finLat.innerHTML = "";
    const dep = document.getElementById("departure");
    dep.innerHTML = "";
    const arr = document.getElementById("arrival");
    arr.innerHTML = "";
}