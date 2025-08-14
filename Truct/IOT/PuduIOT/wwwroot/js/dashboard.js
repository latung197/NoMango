const checkboxLoading = document.getElementById('checkbox-loading');
const dayLoading = document.getElementById('day-loading');
const monthLoading = document.getElementById('month-loading');
const yearLoading = document.getElementById('year-loading');
const hourLoading = document.getElementById('hour-loading');
const tableLoading = document.getElementById('table-loading');




function checkboxZoneChecked(zone) {
    var checkboxesZone = document.querySelectorAll(zone);

    for (var i = 0; i < checkboxesZone.length; i++) {
        if (checkboxesZone[i].checked) {
            return true;
        }
    }
    return false;
}

function changeLableChart() {
    let count = 0;
    let name;
    let title;
    let lable;
    if (window.location.pathname === "/AirConditioner/Dashboard") {
        title = languageResource['DAS-LBL-022'];
        lable = languageResource['COM-LBL-006'];
    }
    else if (window.location.pathname === "/Line/Dashboard") {
        title = languageResource['DAS-LBL-021'];
        lable = languageResource['COM-LBL-004'];
    }

    else if (window.location.pathname === "/AirCompressor/Dashboard") {
        title = languageResource['DAS-LBL-023'];
        lable = languageResource['COM-LBL-007'];
    }

    var checkboxes = document.querySelectorAll('.checkbox-child');
    if (window.location.pathname === "/Line/Dashboard") {
        var aZone = document.getElementById('AZone');
        var bZone = document.getElementById('BZone');
        var cZone = document.getElementById('CZone');
        if (aZone.checked && !bZone.checked && !cZone.checked)
            if (!checkboxZoneChecked('.B-zone') && !checkboxZoneChecked('.C-zone'))
                return languageResource['COM-LBL-009'];
        if (!aZone.checked && bZone.checked && !cZone.checked)
            if (!checkboxZoneChecked('.A-zone') && !checkboxZoneChecked('.C-zone'))
                return languageResource['COM-LBL-010'];
        if (!aZone.checked && !bZone.checked && cZone.checked)
            if (!checkboxZoneChecked('.A-zone') && !checkboxZoneChecked('.B-zone'))
                return languageResource['COM-LBL-011'];
    }


    if (selectAll.checked) return languageResource['DAS-LBL-004'] + ' ' + lable;
    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            name = checkbox.id;
            var label = document.querySelector('label[for="' + checkbox.id + '"]');
            name = label.textContent || label.innerText;
            count++;
            if (count === 2) { return ''; }
        }

    });
    if (count > 1) return title;
    else return name ? name.split(':')[0] : '';
}

function formatTwoDigitNumber(number) {
    return (number < 10 ? "0" : "") + number;
}



function getXAxisValueByType(timeType, startDate, endDate) {

    var timeList = [];

    var currentDate = new Date(startDate);
    endDate = new Date(endDate);
    startDate = new Date(startDate);

    let count = 0;


    while (currentDate <= endDate) {
        if (timeType === '0') {
            if (startDate.getDate() === endDate.getDate() && startDate.getMonth() === endDate.getMonth()) {
                var dateStr = formatTwoDigitNumber(currentDate.getDate());
                var timeStr = formatTwoDigitNumber(currentDate.getHours());
                timeList.push(dateStr + " " + timeStr + ":00");
                currentDate.setHours(currentDate.getHours() + 1);
            }
            else {
                var dateStr = currentDate.getFullYear() + '/' + (currentDate.getMonth() + 1).toString().padStart(2, '0') + '/' + formatTwoDigitNumber(currentDate.getDate());
                timeList.push(dateStr);
                currentDate.setDate(currentDate.getDate() + 1);
            }


        }
        else if (timeType === '1') {
            if (startDate.getMonth() === endDate.getMonth() && startDate.getFullYear() === endDate.getFullYear()) {
                var dateStr = currentDate.getFullYear() + '/' + (currentDate.getMonth() + 1).toString().padStart(2, '0') + '/' + formatTwoDigitNumber(currentDate.getDate());
                timeList.push(dateStr);
                currentDate.setDate(currentDate.getDate() + 1);
            }
            else {
                var dateStr = currentDate.getFullYear() + '/' + (currentDate.getMonth() + 1).toString().padStart(2, '0');
                timeList.push(dateStr);
                currentDate.setMonth(currentDate.getMonth() + 1);
            }


        }
        else if (timeType === '2') {
            if (startDate.getFullYear() === endDate.getFullYear()) {
                var dateStr = currentDate.getFullYear() + '/' + (currentDate.getMonth() + 1).toString().padStart(2, '0');
                timeList.push(dateStr);
                currentDate.setMonth(currentDate.getMonth() + 1);
            }
            else {
                var dateStr = currentDate.getFullYear().toString();
                timeList.push(dateStr);
                currentDate.setFullYear(currentDate.getFullYear() + 1);
            }
        }
        else if (timeType === '3') {
            var dateStr = currentDate.getHours().toString().padStart(2, '0') + ':' + (currentDate.getMinutes()).toString().padStart(2, '0');
            if (currentDate.getMinutes() > 0 || currentDate.getHours() == endDate.getHours())
                timeList.push(dateStr);
            currentDate.setMinutes(currentDate.getMinutes() + 10);

        }

        count++;
    }

    return timeList;
}


$(document).ready(function () {
    createBarChart(dayChartId)
    createBarChart(monthChartId)
    createBarChart(yearChartId)
    createBarChart(hourChartId)

});

function updateCurrentData(data) {
    var labels = document.querySelectorAll('.label-checkbox-child .value');
    labels.forEach(label => {
        var value = data.filter(d => d.unit_name === label.id.replace('for-', ''));
        if (value.length)
            label.innerText = ': ' + value[0].unit_value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        else
            label.innerText = ': _';
    });
}

function performence() {
    const svg = d3.select('# svg');
    svg.attr('transform', 'rotate(45)').attr('fill', 'red');
}

function reloadLabelDateInTable(chartId, today) {

    var time = getTimeRange(getTimeType());
    var rs, value;
    var label = document.getElementById('time-range');
    var labelTitle = document.getElementById('title-time-range');
    labelTitle.innerText = languageResource['DAS-LBL-015'];
    if (today !== null && today !== undefined) {
        switch (chartId) {
            case dayChartId:
                value = languageResource['DAS-BTN-001'];
                break;
            case monthChartId:
                value = languageResource['DAS-LBL-008'];
                break;
            case yearChartId:
                value = languageResource['DAS-LBL-009'];
                break;
            case hourChartId:
                value = languageResource['DAS-LBL-027'];
                break;
            default:
        }
        var now = new Date();
        rs = (now.getHours() - 1).toString().padStart(2, '0') + ':00';
        label.innerText = rs;
        labelTitle.innerText = languageResource['EXP-LBL-006'];
    }
    else
        if (cultureName == 'vi-VN') {
            value = convertDateVnToStartDate(time[0]) + ' ~ ' + convertDateVnToStartDate(time[1]);
            if (chartId === hourChartId) {
                value = time[0].split(' ')[1];
                label.innerHTML = `<span>${value}</span>`;
                labelTitle.innerText = languageResource['EXP-LBL-006'];
            }
            else
                label.innerHTML = `<span>${convertDateVnToStartDate(time[0]).replace('23:59:59', '')}</span><span>~</span><span>${convertDateVnToStartDate(time[1]).replace('23:59:59', '')}</span>`;

        }
        else {
            value = time[0] + ' ~ ' + time[1];
            if (chartId === hourChartId) {
                value = time[0].split(' ')[1];
                label.innerHTML = `<span>${value}</span>`;
            }
            else
                label.innerHTML = `<span>${time[0].replace('23:59:59', '')}</span><span>~</span><span>${time[1].replace('23:59:59', '')}</span>`;
        }





    return value;

}



function updateTable(data) {
    var table = document.getElementById("statistical-table");

    var tbody = table.getElementsByTagName('tbody')[0];
    tbody.innerHTML = "";

    data.forEach(function (item) {
        var row = tbody.insertRow();
        row.id = 'row-' + item.id;
        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        var cell4 = row.insertCell(3);
        var cell5 = row.insertCell(4);

        var icon = `<a class="ps-3"><img src="/assets/svg/ArrowRed.svg" id="btn-language-vn" alt="SVG Image"></a>`;
        if (item.performance < 80)
            icon = `<a class="ps-3"><img src="/assets/svg/ArrowGreen.svg" id="btn-language-vn" alt="SVG Image"></a>`;
        else if (item.performance < 100)
            icon = `<a class="ps-3"><img src="/assets/svg/ArrowOrange.svg" id="btn-language-vn" alt="SVG Image"></a>`;

        cell1.innerHTML = `<div class="border-end height-14 d-flex justify-content-start align-items-center "><span class="fw-normal ps-2 pe-2"> ${item.item_code}</span></div>`;
        cell2.innerHTML = `<div class="border-end height-14 d-flex justify-content-end align-items-center pe-2"><span class="fw-normal ps-2 pe-2"> ${item.unit_value ? item.unit_value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : 0}</span> <span class="fs-8 fw-normal">KWh</span></div>`;
        cell3.innerHTML = `<div class="border-end height-14 d-flex justify-content-end align-items-center pe-2"><span class="fw-normal ps-2 pe-2"> ${item.standard_value}</span> <span class="fs-8 fw-normal">KWh</span></div>`;
        cell4.innerHTML = `<div class="border-end height-14 d-flex justify-content-end align-items-center pe-2"><span class="fw-normal ps-2 pe-2"> ${item.performance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}<span class="fs-8 fw-normal">%</span></span > ${icon}</div>`;
        cell5.innerHTML = `<div class="height-14 d-flex justify-content-center align-items-center">
                                        <button class="d-flex justify-content-center align-items-center btn-edit btn-hover" id="btn-${item.id}" onclick="getRowValues(this)">
                                            <span class="pe-2" >Update</span>
                                             <svg fill="#000000" width="20px" height="20px" viewBox="0 0 40 40" xmlns="http://www.w3.org/2000/svg"><path class="st0" d="M12 25l3 3 15-15-3-3-15 15zM11 26l3 3-4 1z" /></svg>

                                        </button>
                                    </div>`;

    });


}

var rowId;






function getRowValues(btn) {

    rowId = document.getElementById('row-' + btn.id.replace('btn-', ''));

    var col0 = rowId.cells[0].innerText;
    var col2 = rowId.cells[2].innerText;

    document.getElementById('unit-title-modal').innerText = col0;
    document.getElementById('numericInput').value = col2.replace(languageResource['DAS-LBL-010'], '');
    document.getElementById('password').value = '';
    $('#loginModal').modal('show');
}

function setInputValue(id) {
    document.getElementById('numericInput').value = initialValue;
}

function handlePaste(event) {

    event.preventDefault();

    const pastedData = event.clipboardData.getData('text');

    const isNegativeNumber = parseFloat(pastedData) < 0;

    if (!isNegativeNumber)
        document.getElementById('numericInput').value = pastedData;

}

function saveChanges() {
    var numericValue = document.getElementById('numericInput').value;


    var mydata = {
        id: rowId.id.replace('row-', ''),
        value: numericValue
    };


    $.ajax({
        url: "update_unit",
        type: "POST",
        data: mydata,
        beforeSend: function () {
        },

        success: function (data) {
            changeUIDatetimePicker();
            loadData();
        },
        error: function (e) {
            loading.style.display = "none";
            console.log(e);
        }
    });
}

function getHtml(id) {
    var html = [];

    var canvas = document.createElement("canvas");

    canvas.width = $(id).width();
    canvas.height = $(id).height();
    var svgElement = document.querySelector(`${id} svg`);

    var resizedContext = canvas.getContext("2d");

    canvas.height = "215";
    canvas.width = "890";


    resizedContext.drawImage(canvas, 0, 0, 890, 215);
    canvg(canvas, svgElement.innerHTML);

    html.push(canvas.toDataURL("image/png"));

    return html;
}

function exportPDF(units, time, type, id) {

    let name;
    switch (type) {
        case '0':
            name = 'datePDF';
            break;
        case '1':
            name = 'monthPDF';
            break;
        case '2':
            name = 'yearPDF';
            break;
        case '3':
            name = 'hourPDF';
            break;
        default:
    }
    let date = new Date();

    $.ajax({
        url: "export-data-PDF",
        type: "POST",
        data: { html: getHtml(id), unitCode: units.join(','), timeType: type, startTime: time[0], endTime: time[1], eDashboard: getTypeDashboardResource() },
        beforeSend: function () {
        },
        xhrFields: {
            responseType: 'arraybuffer'
        },
        success: function (data) {

            const blob = new Blob([data], { type: 'application/pdf' });
            saveAs(blob, `${name}_${date.getDate()}${date.getHours()}.pdf`);
        },
        error: function (e) {
            console.log(e);
        }
    });



}

function exportCSV(units, time, type) {
    let name;
    switch (type) {
        case '0':
            name = 'dateCSV';
            break;
        case '1':
            name = 'monthCSV';
            break;
        case '2':
            name = 'yearCSV';
            break;
        case '3':
            name = 'hourCSV';
            break;

        default:
    }
    let date = new Date();

    $.ajax({
        url: "export-data-CSV",
        type: "POST",
        data: { unitCode: units.join(','), timeType: type, startTime: time[0], endTime: time[1], eDashboard: getTypeDashboardResource() },
        beforeSend: function () {
        },
        xhrFields: {
            responseType: 'arraybuffer'
        },
        success: function (data) {

            const blob = new Blob([data], { type: 'text/csv' });
            saveAs(blob, `${name}_${date.getDate()}${date.getHours()}.csv`);

        },
        error: function (e) {
            console.log(e);
        }


    })
}


var heightChart = 225;
var btnMenuCollapse = document.getElementById('menuToggleCollapse');
var btnMenuExpand = document.getElementById('menuToggleExpand');

btnMenuCollapse.addEventListener('click', function (e) {

    heightChart = 205;
    MemuResize();

});

btnMenuExpand.addEventListener('click', function (e) {

    heightChart = 225;
    MemuResize();

});

