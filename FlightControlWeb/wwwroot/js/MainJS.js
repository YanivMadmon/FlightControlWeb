let thisFlight = "";
function getFlights() {
    let current = getCurrentTime();
    $.getJSON("http://ronyut2.atwebpages.com/ap2/api/Flights?relative_to=" + current, function (data) {
        //console.log(data);
        data.forEach(function (flight) {
            $(flight).each(function (index, value) {
                addFlights(value);
            })
        })
        $('#flight_table tbody').empty();
        $('#flight_table tbody').append(thisFlight);
        thisFlight = "";
        console.log(thisFlight);
    });
 }

function flightsTable() {
    setInterval(function () {
        getFlights();
    }, 4000);
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
    thisFlight += "<tr><td>" + flight.flight_id +
        "</td>" + "<td>" + flight.company_name +
        "</td>" + "<td>" + flight.is_external + "</td>" + "<td>" + butt + "</td></tr>";
}

function removeFlight(flight) {
    console.log(flight);
    let url1 = "http://ronyut2.atwebpages.com/ap2/api/Flights/" + flight.flight_id;
    console.log(url1);
    $.ajax({
        url: url1,
        type: 'DELETE',
        success: function (result) {
            console.log("remove succes");
        }
    });
}