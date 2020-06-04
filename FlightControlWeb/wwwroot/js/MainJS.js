let thisFlight = "";
let idCol = -1;
/*function getFlights() {
    let current = getCurrentTime();
    $.getJSON("http://rony10.atwebpages.com/api/Flights?relative_to=" + current, function (data) {
        //console.log(data);
        try {
            IterateAllFlights(data);
            $('#flight_table tbody').empty();
            $('#flight_table tbody').append(thisFlight);
        }
        catch (e) {
            console.log(e);
        }
        thisFlight = "";
    });
 }*/
async function getFlights() {
    try {
        const currentTime = getCurrentTime();
        let url = 'http://rony10.atwebpages.com' + '/api/Flights?relative_to=' + currentTime;
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
    let url1 = "http://rony10.atwebpages.com/api/Flights/" + idFlighet;
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
    //console.log(row);
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
    flightDetailsText = "";
    $.getJSON("http://rony10.atwebpages.com/api/FlightPlan/" + id, function (data) {
        console.log(data);
        flightDetailsText=data
    });
    //document.getElementById("details").value = "Fifth Avenue, New York City";
}