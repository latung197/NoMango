const alertPlaceholder = document.getElementById('liveAlertPlaceholder')
const visualLoading = document.getElementById('visual-loading');

const alert = (message, type) => {
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible fade show position-fixed" style="top: 15%; left: 50%; transform: translate(-50%, -50%); z-index: 9999999;" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('')

    alertPlaceholder.append(wrapper)

    setTimeout(function () {
        $(".alert").alert('close');
    }, 3000);
}

var electricCost = 0;

function reloadData(data) {
    const zones = ['Air', 'A', 'B', 'C', 'total', 'avg', 'Pac'];

    var cost = parseFloat(languageResource['ElectricCost'] ? languageResource['ElectricCost']: 0 );

    try {
        zones.forEach(zone => {
            const element = document.getElementById(`index-${zone}`);
            const elementCo2 = document.getElementById(`index-${zone}-co2`);
            const elementCost = document.getElementById(`index-${zone}-cost`);  
         
            const zoneValue = parseFloat(data[zone].replace(',', '.'));

            element.innerText = zoneValue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            if (elementCo2)
                elementCo2.innerText = (zoneValue * 0.455 / 1000).toLocaleString('en-US', { minimumFractionDigits: 4, maximumFractionDigits: 4 });
            if (elementCost)
                elementCost.innerText = (Math.round(zoneValue * cost / 1000) * 1000).toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
        });
        //const chartColumn = ['A', 'B', 'C', 'Air', 'Pac']
        DrawChart(data, '#visualChart', chartColumn)
    }
    catch (ex) {
        console.log(ex);
    }
    finally {
        visualLoading.style.display = 'none';
    }
}

function removeClassPerformance(element) {
    var classesToRemove = ['bg-data-80', 'bg-data-100', 'bg-data-zero'];

    classesToRemove.forEach(function (className) {
        element.parentElement.classList.remove(className);
    });
}

function reloadDataLine(data, totalData) {
    data.forEach(item => {
        var unit = document.getElementById(item.unit_name);
        removeClassPerformance(unit);
        if (parseFloat(item.performance) >= 100)
            unit.parentElement.classList.add('bg-data-100');
        else
            if (parseFloat(item.performance) >= 80)
                unit.parentElement.classList.add('bg-data-80');
   
        unit.innerText = languageResource[item.display_name] + ' : ' + item.unit_value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' ' + languageResource["DAS-LBL-010"];
        if (unit.innerText === '0.00')
            unit.parentElement.classList.add('bg-data-zero');
    });

    var total = document.getElementById('index-total');
    var avg = document.getElementById('index-avg');
   
    total.innerText = parseFloat(totalData.totalLine.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    avg.innerText = parseFloat(totalData.avgLine.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    if (total.innerText === '0.00')
        total.parentElement.parentElement.classList.add('bg-data-zero');
    if (avg.innerText === '0.00')
        avg.parentElement.parentElement.classList.add('bg-data-zero');

    visualLoading.style.display = 'none';
}

function reloadDataAirConditioner(data, totalData) {
    data.forEach(item => {
        var unit = document.getElementById(item.unit_name);
        removeClassPerformance(unit);
        
        if (parseFloat(item.performance) >= 100)
            unit.parentElement.classList.add('bg-data-100');
        else
            if (parseFloat(item.performance) >= 80)
                unit.parentElement.classList.add('bg-data-80');
        
        unit.innerText = item.unit_value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        if (unit.innerText === '0.00')
            unit.parentElement.classList.add('bg-data-zero');
    });

    var total = document.getElementById('index-total');
    var avg = document.getElementById('index-avg');

    total.innerText = parseFloat(totalData.Pac.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    avg.innerText = parseFloat(totalData.avgPac.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    if (total.innerText === '0.00')
        total.parentElement.parentElement.classList.add('bg-data-zero');
    if (avg.innerText === '0.00')
        avg.parentElement.parentElement.classList.add('bg-data-zero');
    
    visualLoading.style.display = 'none';


}

function reloadDataAirCompressor(data, totalData) {
    data.forEach(item => {
        var unit = document.getElementById(item.unit_name);
        removeClassPerformance(unit);
        if (parseFloat(item.performance) >= 100)
            unit.parentElement.classList.add('bg-data-100');
        else
            if (parseFloat(item.performance) >= 80)
                unit.parentElement.classList.add('bg-data-80');
        unit.innerText = languageResource[item.display_name] + ' : ' + item.unit_value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        if (unit.innerText === '0.00')
            unit.parentElement.classList.add('bg-data-zero');
    });
    var total = document.getElementById('index-total');
    var avg = document.getElementById('index-avg');

    total.innerText = parseFloat(totalData.Air.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    avg.innerText = parseFloat(totalData.avgAir.replace(',', '.')).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    if (total.innerText === '0.00')
        total.parentElement.parentElement.classList.add('bg-data-zero');
    if (avg.innerText === '0.00')
        avg.parentElement.parentElement.classList.add('bg-data-zero');

    visualLoading.style.display = 'none';
}



var connection = new signalR.HubConnectionBuilder()
    .withUrl("/realTimeHub")
    .withAutomaticReconnect()
    .build();


connection.on("ReceiveRealTimeData", (data) => {
    var obj = JSON.parse(data);
    if (window.location.pathname === "/") {
        reloadData(obj.Data);
    }
    //else if (window.location.pathname === "/AirConditioner/Visualization") {
    //    reloadDataAirConditioner(obj.Pac, obj.Data);
    //}
    //else if (window.location.pathname === "/Line/Visualization") {
    //    reloadDataLine(obj.Line, obj.Data);
    //}
    //else if (window.location.pathname === "/AirCompressor/Visualization") {
    //    reloadDataAirCompressor(obj.Air, obj.Data)
    //}
})



connection.stop().then(() => {
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});

connection.start().then(() => {
    console.log("Connection established.");
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});

//window.addEventListener('focus', function () {
//    connection.start().then(() => {
//        console.log("Connection established.");
//    }).catch((err) => {
//        console.error("SignalR connection error: " + err.toString());
//    });
//});
//window.addEventListener('blur', function () {
//    connection.stop().then(() => {
//    }).catch((err) => {
//        console.error("SignalR connection error: " + err.toString());
//    });
//});

function convertTime(date, culture) {
    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var day = date.getDate().toString().padStart(2, '0');
    var hours = date.getHours().toString().padStart(2, '0');
    var minutes = date.getMinutes().toString().padStart(2, '0');
    if (culture !== 'vi-VN') {
        return `${year}/${month}/${day} ${hours}:${minutes}`;

    }
    else {
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }


}
var sideBarStorage = {
    electric: false,
    pac: false,
    air: false
}

function createBarChart(id) {

    const svg = d3.select(id)
        .append("svg")
        .attr("preserveAspectRatio", "xMinYMin meet")

    const ctr = svg.append("g").classed('chart-group', true);


    ctr.append('g')
        .classed('axis-group', true)
        .attr('shape-rendering', 'geometricPrecision')
        .classed('x-axis', true);

    ctr.append('g')
        .classed('axis-group', true)
        .classed('y-axis', true)

    ctr.append('g')
        .classed('rect-group', true)


    ctr.append('g')
        .classed('label-groups', true)

    d3.select(id)
        .append("div")
        .attr("class", "tooltip-donut")
        .style("position", "absolute")
        .style("visibility", "hidden")
        .append('span');

}

function updateBarChart(data, id, timelist, today) {
    try {


        var classLabel = 'title-' + id.replace('#', '');
        var labelsChange = document.querySelectorAll(`.${classLabel}  .label-change`);
        var labelsDay = document.querySelectorAll(`.${classLabel}  .label-day`);
        var text = changeLableChart();
        var rs = reloadLabelDateInTable(id, today);
        if (today !== 'NoLoad') {
            labelsChange.forEach(label => {
                label.innerText = text;
            });
            labelsDay.forEach(label => {
                label.innerText = rs.replace(' 23:59:59', '');
            });
        }
        var maxObject = data.reduce(function (prev, current) {
            return (prev.totalvalue > current.totalvalue) ? prev : current;
        }, {});


        const width = 890;
        const height = 225;
        const marginTop = 20;
        const marginRight = 30;
        const marginBottom = 30;
        const marginLeft = 40;

        // Declare the x (horizontal position) scale.
        const x = d3.scaleBand()
            .domain(timelist.map(d => d))
            .range([marginLeft, width - marginRight])
            .padding(0.1);

        // Declare the y (vertical position) scale.
        const y = d3.scaleLinear()
            .domain([0, maxObject.totalvalue || 100])
            .range([height - marginBottom, marginTop])
            .rangeRound([height - marginBottom, marginTop])
            .nice()
            .clamp(true)

        // Create the SVG container.
        const svg = d3.select(id + " svg")
            .attr("width", width)
            .attr("height", height)
            .attr("viewBox", [0, 0, width, height])
            .attr("style", "max-width: 100%; height: auto;");


        var rectGroup = svg.select(".chart-group .rect-group")
            .selectAll("rect")
            .data(data);

        var widthRect = x.bandwidth() < 60 ? x.bandwidth() : 60;

        // Remove any bars that are no longer needed
        rectGroup.exit().remove();

        // Add new bars
        rectGroup.enter()
            .append("rect")
            .merge(rectGroup)

            .attr("fill", "#4472c4")
            .attr("x", (d) => x(d.datetime) + x.bandwidth() / 2 - widthRect / 4)
            .attr("y", (d) => y(d.totalvalue))
            .attr("height", (d) => y(0) - y(d.totalvalue))
            .attr("width", widthRect / 2)
            .attr('id', (d, i) => i)
            .on("mouseover", function (e, d, k) {
                svg.select(`#label_${e.target.id}`).attr('display', 'block')
            })
            .on("mouseout", (e) => {
                svg.select(`#label_${e.target.id}`).attr('display', 'none')
            });


        // Add the x-axis and label.
        const xAxisGroup = d3.select(id + " svg g .x-axis")
            .attr("transform", `translate(0,${height - marginBottom})`)
            .call(d3.axisBottom(x).tickSizeOuter(0));

        xAxisGroup.selectAll('text')
            .attr('text-anchor', 'center')
            .attr('fill', '#212529')
            .text((d) => {

                if (id === dayChartId)

                    return d.split(' ')[1] ? d.split(' ')[1] : new Date(d).getDate();
                else if (id === monthChartId)
                    return d.split('/')[2] ? new Date(d).getDate() : new Date(d).getMonth() + 1;
                else if (id === yearChartId)
                    return d.split('/')[1] ? new Date(d).getMonth() + 1 : new Date(d).getFullYear();
                else return d;
            })

        // Add the y-axis and label, and remove the domain line.
        d3.select(id + " svg g .y-axis")
            .attr("transform", `translate(${marginLeft},0)`)
            .transition().duration(1000)
            .call(d3.axisLeft(y));

        const yAxis = d3.select(id + " svg g .y-axis");
        yAxis.select('.domain').remove();
        yAxis.selectAll('.tick line').remove();

        yAxis.selectAll(" .tick").append("line")
            .attr("class", "vertical-line")
            .attr("x1", 0)
            .attr("y1", (d) => y(d.totalvalue))
            .attr("x2", width - marginRight - marginLeft)
            .attr("y2", (d) => y(d.totalvalue))
            .attr("stroke", "lightgray")
            .attr("stroke-width", 0.5)




        const labelsGroup = svg.select('.label-groups')
            .attr('font-size', `10`)
            .selectAll('text')
            .data(data);

        // Enter phase
        const labelsEnter = labelsGroup
            .enter()
            .append('text')

            .attr('y', (d) => y(d.totalvalue) - 5)
            .attr('id', (d, i) => 'label_' + i)
            .text(function (d) {
                return d.totalvalue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            });

        // Update phase
        labelsGroup
            .merge(labelsEnter)
            .transition().duration(1000)
            .attr('y', (d) => y(d.totalvalue) - 5)
            .text(function (d) {
                return d.totalvalue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            });

        labelsGroup.exit().remove();
        const label = document.querySelector(`${id} .label-groups text`);
        if (label !== null) {
            var labelWidth = label.getBoundingClientRect().width;

            if (window.screen.width > 1280)
                labelWidth = labelWidth / 1.486;
            svg.select('.label-groups')
                .selectAll('text')
                .attr('display', 'block')

            svg.select('.label-groups')
                .selectAll('text')
                .attr('x', (d, i, nodes) => {
                    const labelWidth = d3.select(nodes[i]).node().getBBox().width;
                    return x(d.datetime) + x.bandwidth() / 2 - labelWidth / 2;
                })
                .attr('display', 'none')
        }



    }
    catch (e) {
        console.log(e)
    }
    finally {
        closeLoading();
    }
}