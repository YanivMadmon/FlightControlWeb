let thisFlight = "";
let idCOl = -1;
function getFlights() {
    let current = getCurrentTime();
    $.getJSON("http://rony5.atwebpages.com/api/Flights?relative_to=" + current, function (data) {
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
        //console.log(thisFlight);
    });
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
    if (idCOl === flight.flight_id) {
        thisFlight += "style=\"background-color: red;\" ";
    }
    thisFlight +=" "+"onclick =\"clickRow(this)\"><td>" + flight.flight_id +
        "</td>" + "<td>" + flight.company_name +
        "</td>" + "<td>" + flight.is_external + "</td>" + "<td>" + butt + "</td></tr>";
    console.log(thisFlight);
}



function removeFlight(row) {
    let a = row.parentNode.parentNode;
    let idFlighet = a.cells[0].innerHTML;
    let url1 = "http://rony5.atwebpages.com/api/Flights/" + idFlighet;
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

function clickRow(row) {
    console.log(row);
    let flightId = row.cells[0].innerHTML;
    if (flightId in flightDic) {
        clickFlight(flightId);
        if (idCOl != -1) { //to found if any row is in color
            let rowCol = document.getElementById(idCOl);
            rowCol.style.backgroundColor = "white";
        }
        row.style.backgroundColor = "red";
        idCOl = flightId;
    }
}

function clickFlight(id) {
    clickRow(id);
    airplanClick(id);
    //flightDetails(id);
}