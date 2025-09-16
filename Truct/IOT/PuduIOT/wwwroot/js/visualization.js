function DrawChart(data, id, column) {
    try {

        const columnName = ['COM-LBL-009', 'COM-LBL-010', 'COM-LBL-011', 'COM-LBL-007', 'COM-LBL-016']
        const columnColor = ['#E2E090', '#E2E090', '#E2E090', '#DFBDCF', '#8DE9EF']

        var maxVaue = findMaxValue(data)
        const numericData = {};

        for (const key in data) {
            numericData[key] = parseFloat(data[key].replace(',', '.'));
        }

        const keys = Object.keys(numericData);
        const width = 615;
        const height = 215;
        const marginTop = 20;
        const marginRight = 30;
        const marginBottom = 30;
        const marginLeft = 40;

        // Declare the x (horizontal position) scale.
        const x = d3.scaleBand()
            .domain(column.map(d => d))
            .range([marginLeft, width - marginRight])
            .padding(0.1);

        // Declare the y (vertical position) scale.
        const y = d3.scaleLinear()
            .domain([0, maxVaue || 100])
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
            .data(column);

        var widthRect = x.bandwidth() < 60 ? x.bandwidth() : 60;

        // Remove any bars that are no longer needed
        rectGroup.exit().remove();

        // Add new bars
        rectGroup.enter()
            .append("rect")
            .merge(rectGroup)

            .attr("fill", (d, i) => columnColor[i])
            .attr("x", (d) => x(d) + x.bandwidth() / 2 - widthRect / 4)
            .attr("y", (d) => y(numericData[d]))
            .attr("height", (d) => y(0) - y(numericData[d]))
            .attr("width", widthRect / 2)
            .attr('id', (d, i) => i)
            

        // Add the x-axis and label.
        const xAxisGroup = d3.select(id + " svg g .x-axis")
            .attr("transform", `translate(0,${height - marginBottom})`)
            .call(d3.axisBottom(x).tickSizeOuter(0));

        xAxisGroup.selectAll('text')
            .attr('text-anchor', 'center')
            .attr('fill', '#212529')
            .attr('font-size', `12`)
            .text((d, i) => languageResource[columnName[i]])

        // Add the y-axis and label, and remove the domain line.
        d3.select(id + " svg g .y-axis")
            .attr("transform", `translate(${marginLeft},0)`)
            .transition().duration(1000)
            .call(d3.axisLeft(y).tickFormat(d3.format(".2s")));

        const yAxis = d3.select(id + " svg g .y-axis");
        yAxis.select('.domain').remove();
        yAxis.selectAll('.tick line').remove();

        yAxis.selectAll(" .tick").append("line")
            .attr("class", "vertical-line")
            .attr("x1", 0)
            .attr("y1", (d) => y(numericData[d]))
            .attr("x2", width - marginRight - marginLeft)
            .attr("y2", (d) => y(numericData[d]))
            .attr("stroke", "lightgray")
            .attr("stroke-width", 0.5)

        yAxis.selectAll('text').attr('fill', '#212529').attr('font-size', `12`);

        const labelsGroup = svg.select('.label-groups')
            .attr('font-size', `12`)
            .selectAll('text')
            .data(column);

        // Enter phase
        const labelsEnter = labelsGroup
            .enter()
            .append('text')
            .attr('y', (d) => y(numericData[d]) - 5)
            .attr('id', (d, i) => 'label_' + i)
            .text(function (d) {
                return numericData[d].toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            });

        // Update phase
        labelsGroup
            .merge(labelsEnter)
            .transition().duration(1000)
            .attr('y', (d) => y(numericData[d]) - 5)
            .text(function (d) {
                return numericData[d].toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            });

        labelsGroup.exit().remove();
        const label = document.querySelector(`${id} .label-groups text`);
        if (label !== null) {

            svg.select('.label-groups')
                .selectAll('text')
                .attr('x', (d, i, nodes) => {
                    const labelWidth = d3.select(nodes[i]).node().getBBox().width;
                    return x(d) + x.bandwidth() / 2 - labelWidth / 2;
                })
        }

    }
    catch (e) {
        console.log(e)
    }
    finally {
    }
}
function findMaxValue(obj) {
    let maxValue = -Infinity;

    for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
            const value = parseFloat( obj[key]);
            if (typeof value === 'number' && value > maxValue) {
                maxValue = value;
            }
        }
    }

    return maxValue;
}

