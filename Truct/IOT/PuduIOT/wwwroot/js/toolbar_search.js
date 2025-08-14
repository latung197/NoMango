$(document).ready(function () {
    changeUIDatetimePicker();
    loadData();
});

var objDataChart;
var timeListChart = {
    DayChart: [],
    MonthChart: [],
    YearChart: [],
    HourChart: []
}

function loadData() {

    var loading = document.getElementById('loading');
    try {
        var unitsSelected = getUnitsSelected();
        if (unitsSelected.length === 0) {
            alert(languageResource['E-007'], 'danger');
            return;
        }
        checkboxLoading.style.display = 'block';
        tableLoading.style.display = 'block';
        dayLoading.style.display = 'block';
        monthLoading.style.display = 'block';
        yearLoading.style.display = 'block';
       
        const url = `data_real_time?&units=${unitsSelected}&eDashboard=${getTypeDashboardResource()}`;

        const selectedUnits = getUnitsSelected();

        unitChecked.dayUnit = selectedUnits;
        unitChecked.monthUnit = selectedUnits;
        unitChecked.yearUnit = selectedUnits;
        unitChecked.hourUnit = selectedUnits;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                var obj = JSON.parse(data);
                objDataChart = obj;
                timeListChart.DayChart = getDefaultTimeList('0');
                timeListChart.MonthChart = getDefaultTimeList('1');
                timeListChart.YearChart = getDefaultTimeList('2');
                timeListChart.HourChart = getDefaultTimeList('3');

                var [DataUnit, DataChart, StatisticalTable, StatisticalTableToday] = generateData(obj.DayChart);
                var MonthChart = generateDataChart(obj.MonthChart);
                var YearChart = generateDataChart(obj.YearChart);
                var HourChart = generateDataChart(obj.HourChart);

                updateCurrentData(DataUnit);

                updateBarChart(DataChart, dayChartId, timeListChart.DayChart, 1);
                updateBarChart(MonthChart, monthChartId, timeListChart.MonthChart, 1);
                updateBarChart(YearChart, yearChartId, timeListChart.YearChart, 1);
                updateBarChart(HourChart, hourChartId, timeListChart.HourChart, 1);
                objDataChart.DayChart = DataChart;
                objDataChart.MonthChart = MonthChart;
                objDataChart.YearChart = YearChart;
                updateTable(obj.Performance);
            },
            error: function (error) {

                console.error('Error:', error);

            }
        });
    }
    finally {
        loading.style.display = 'none';
    }
}
function getTypeDashboardResource() {
    if (window.location.pathname === "/AirConditioner/Dashboard")
        return 1;
    if (window.location.pathname === "/Line/Dashboard")
        return 0;
    if (window.location.pathname === "/AirCompressor/Dashboard")
        return 2;
}
function generateDataChart(data) {
    var DataChart = []
    var totalByUnit = data.reduce(function (acc, item) {
        var dateTime = item.datetime;
        var totalValue = item.totalvalue;

        if (!acc[dateTime]) {
            acc[dateTime] = 0;
        }

        acc[dateTime] += totalValue;

        return acc;
    }, {});
    for (var dateTime in totalByUnit) {
        if (totalByUnit.hasOwnProperty(dateTime)) {
            var rs = {
                datetime: dateTime,
                totalvalue: totalByUnit[dateTime]
            }
            DataChart.push(rs);
        }
    }
    return DataChart;
}
function generateData(data) {
    var totalByUnit = data.reduce(function (acc, item) {
        var unitName = item.unit_name;
        var totalValue = item.totalvalue;

        if (!acc[unitName]) {
            acc[unitName] = 0;
        }

        acc[unitName] += totalValue;

        return acc;
    }, {});
    var DataUnit = [];
    for (var unitName in totalByUnit) {
        if (totalByUnit.hasOwnProperty(unitName)) {
            var rs = {
                unit_name: unitName,
                unit_value: totalByUnit[unitName]
            }
            DataUnit.push(rs);
        }
    }

    var averageByUnit = data.reduce(function (acc, item) {
        var unitName = item.unit_name;
        var avgValue = item.avg_value;
        var standardValue = item.standard_value;
        var itemCode = item.display_name;
        var id = item.id;

        if (!acc[unitName]) {
            acc[unitName] = {
                sum: 0,
                count: 0,
                standardValue: standardValue !== undefined? standardValue : 0,
                item_code: itemCode,
                id: id
            };
        }

        acc[unitName].sum += avgValue;
        acc[unitName].count++;

        return acc;
    }, {});
    var StatisticalTable = [];
    for (var unitName in averageByUnit) {
        if (averageByUnit.hasOwnProperty(unitName)) { 
            var average = averageByUnit[unitName].sum / averageByUnit[unitName].count;
            var rs = {
                id: averageByUnit[unitName].id,
                item_code: averageByUnit[unitName].item_code,
                unit_value: average, 
                standard_value: averageByUnit[unitName].standardValue,
                performance: averageByUnit[unitName].standardValue !== 0 ?  (average / averageByUnit[unitName].standardValue) * 100 : 0
            }
            StatisticalTable.push(rs);
        }
    }

    var StatisticalTableToday = [];
    data.forEach(function ( item) {
        var unitName = item.unit_name;
        var avgValue = item.avg_value;
        var standardValue = item.standard_value !== undefined ? item.standard_value : 0;
        var itemCode = item.display_name;
        var id = item.id;
        var date = item.datetime;
        var dateNow = new Date().getDate().toString().padStart(2, '0') + ' ' + (new Date().getHours() - 1).toString().padStart(2, '0') + ':00';
        if (!item[unitName] && date === dateNow) {
            var rs = {
                unit_value: avgValue,
                performance: standardValue!== 0 ? avgValue / standardValue * 100 : 0,
                standard_value: standardValue ,
                item_code: itemCode,
                id: id
            };
            StatisticalTableToday.push(rs);

        }

    });


    var DataChart = generateDataChart(data);
    

    return [DataUnit, DataChart, StatisticalTable, StatisticalTableToday];
}

function searchData() {
    try {

        var type = getTimeType();
        var time = getTimeRange(type);

        if (!validateTime(type, time)) return;
        checkboxLoading.style.display = 'block';
        tableLoading.style.display = 'block';
        const url = `search_data?&type=${type}&units=${getUnitsSelected()}&startTime=${time[0]}&endTime=${time[1]}&eDashboard=${getTypeDashboardResource()}`;
        var timelist = getXAxisValueByType(type, time[0], time[1]);
        let chartId;
        switch (type) {
            case '0':
                unitChecked.dayUnit = getUnitsSelected();
                timeRange.dayChart = time;
                chartId = dayChartId;
                dayLoading.style.display = 'block'
                break;
            case '1':
                unitChecked.monthUnit = getUnitsSelected();
                timeRange.monthChart = time;
                chartId = monthChartId;
                monthLoading.style.display = 'block'
                break;
            case '2':
                unitChecked.yearUnit = getUnitsSelected();
                timeRange.yearChart = time;
                chartId = yearChartId;
                yearLoading.style.display = 'block'
                break;
            case '3':
                unitChecked.hourUnit = getUnitsSelected();
                timeRange.hourChart = time;
                chartId = hourChartId;
                hourLoading.style.display = 'block'
                break;
            default:
                chartId = dayChartId;
                break;
        }
        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {


                var obj = JSON.parse(data);

                var [DataUnit, DataChart, StatisticalTable] = generateData(obj.DataChart);
          


                updateTable(StatisticalTable);
                updateBarChart(DataChart, chartId, timelist);
                updateCurrentData(DataUnit);
                switch (type) {

                    case '0':
                        objDataChart.DayChart = DataChart;
                        timeListChart.DayChart = timelist;
                        break;
                    case '1':
                        objDataChart.MonthChart = DataChart;
                        timeListChart.MonthChart = timelist;
                        break;
                    case '2':
                        objDataChart.YearChart = DataChart;
                        timeListChart.YearChart = timelist;
                        break;
                    case '3':
                        objDataChart.HourChart = DataChart;
                        timeListChart.HourChart = timelist;
                        break;
                    default:
                        chartId = dayChartId;
                        break;
                }
            },
            error: function (error) {
                console.error('Error:', error);

            }
        });
    }
    finally {


    }
}

function MemuResize() {

    updateBarChart(objDataChart.DayChart, dayChartId, timeListChart.DayChart, 'NoLoad');
    updateBarChart(objDataChart.MonthChart, monthChartId, timeListChart.MonthChart, 'NoLoad');
    updateBarChart(objDataChart.YearChart, yearChartId, timeListChart.YearChart, 'NoLoad');

}


function closeLoading() {
    checkboxLoading.style.display = 'none';
    tableLoading.style.display = 'none';
    dayLoading.style.display = 'none';
    monthLoading.style.display = 'none';
    yearLoading.style.display = 'none';
    hourLoading.style.display = 'none';
}

/**
 * Validates the time range based on the specified type.
 * @param {string} type - The type of validation to perform (0, 1, 2).
 * @returns {boolean} - True if the time range is valid; otherwise, false.
 */
function validateTime(type, time) {
    try {
        var checkboxes = document.querySelectorAll('.checkbox-child');
        var isCheck = false;
        checkboxes.forEach(function (checkbox) {
            if (checkbox.checked) {
                isCheck = true;
            }
        });
        if (!isCheck) {
            alert(languageResource['E-007'], 'danger');
            return false;
        }

        if (time === null || time[1].includes('yyyy')) {
            if (type === '3') {
                alert(languageResource['E-011'], 'danger');
                return false;
            }
            alert(languageResource['E-001'], 'danger');
            return false;
        }
        const [startTime, endTime] = time;
        const startDate = new Date(startTime);
        const endDate = new Date(endTime);

        if (startDate > endDate) {
            alert(languageResource['E-002'], 'danger'); return false;
        }


        switch (type) {
            case '0':
                if (!validateDifference(startDate, endDate, 31, 'days')) {
                    alert(languageResource['E-004'], 'danger');
                    return false;
                }
                break;
            case '1':
                if (!validateDifference(startDate, endDate, 12, 'months')) {
                    alert(languageResource['E-005'], 'danger');
                    return false;
                }
                break;
            case '2':
                if (!validateDifference(startDate, endDate, 10, 'years')) {
                    alert(languageResource['E-006'], 'danger');
                    return false;
                }
                break;
            case '3':
                if (!validateDifference(startDate, endDate, 1, 'hours')) {
                    alert(languageResource['E-006'], 'danger');
                    return false;
                }
                break;
                return false;
        }
        return true;
    }
    catch {
        return false;
    }
}



function validateDifference(startDate, endDate, maxValue, type) {
    const momentStartDate = moment(startDate);
    const momentEndDate = moment(endDate);

    const monthsDiff = momentEndDate.diff(momentStartDate, type, true);
    return monthsDiff <= maxValue;
}




function changeUIDatetimePicker() {
    var selectedValue = getTimeType();
    var endTimeComponent = document.getElementById('end-time-component');
    var startTimeComponent = document.querySelector('#start-time-component span');

    var [formatTime, formatValue] = getFormatTimeCulture(selectedValue);

    $('#start-time, #end-time').val(formatValue);
    $('#start-time, #end-time').datepicker('destroy');
    $('#start-time, #end-time').datetimepicker('destroy');
    endTimeComponent.style.display = '';
    startTimeComponent.style.display = '';
    switch (selectedValue) {
        case '0':
            $('#start-time, #end-time').datetimepicker({
                format: formatTime,
                datepicker: true,
                timepicker: false,
                showButtonPanel: true
            });
            break;
        case '1':
            $('#start-time, #end-time').datepicker({
                format: formatTime,
                viewMode: "months",
                minViewMode: "months",
                autoclose: true
            });
            break;
        case '2':
            $('#start-time, #end-time').datepicker({
                format: formatTime,
                viewMode: "years",
                minViewMode: "years",
                autoclose: true
            });
            break;
        case '3':
            endTimeComponent.style.cssText = 'display: none !important';
            startTimeComponent.style.cssText = 'display: none !important';
            $('#start-time, #end-time').datetimepicker({
                format: formatTime,
                datepicker: true,
                timepicker: true,
                showButtonPanel: true
            });
            break;
        default:
            break;
    }
}

function getFormatTimeCulture(selectedValue) {
    if (cultureName === 'vi-VN') {
        switch (selectedValue) {
            case '0':
                return ['d/m/Y', 'dd/MM/yyyy'];
            case '1':
                return ['mm/yyyy', 'MM/yyyy'];
            case '2':
                return ['yyyy', 'yyyy'];
            case '3':
                return ['d/m/Y H:00', 'dd/MM/yyyy HH:mm'];
            default:
                break;
        }
    }
    else {
        switch (selectedValue) {
            case '0':
                return ['Y/m/d', 'yyyy/MM/dd'];
            case '1':
                return ['yyyy/mm', 'yyyy/MM'];
            case '2':
                return ['yyyy', 'yyyy'];
            case '3':
                return ['Y/m/d H:00', 'yyyy/MM/dd HH:mm'];
            default:
                break;
        }
        
    }
}

function getDaysInMonth() {
    var date = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);
    return date.getDate();
}

/**
 * Function that returns the current time range based on the selected type.
 * @param {string} type - The type of time (0, 1, or other).
 * @returns {Array} - An array containing the start and end of the time range.
 */
function getCurrentTimeRange(type) {
    switch (type) {
        case '1':
            return getCurrentMonthTimeRange();
        case '2':
            return getCurrentYearTimeRange();
        case '0':
            return getTodayTimeRange();
        case '3':
            return getHourTimeRange();
        default:
            break;
    }
}


function getHourTimeRange() {
    var today = new Date();
    var startOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), today.getHours(),00,00);
    var endOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), today.getHours() + 1, 00, 00);

    return [startOfDay, endOfDay];
}

/**
 * Function that returns the time range for the current day.
 * @returns {Array} - An array containing the start and end of the time range.
 */
function getTodayTimeRange() {
    var today = new Date();
    var startOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var endOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59);

    return [startOfDay, endOfDay];
}

/**
 * Function that returns the time range for the current month.
 * @returns {Array} - An array containing the start and end of the time range.
 */
function getCurrentMonthTimeRange() {
    var today = new Date();
    var firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    var lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    return [firstDayOfMonth, lastDayOfMonth];
}

/**
 * Function that returns the time range for the current year.
 * @returns {Array} - An array containing the start and end of the time range.
 */
function getCurrentYearTimeRange() {
    var today = new Date();
    var firstDayOfYear = new Date(today.getFullYear(), 0, 1);
    var lastDayOfYear = new Date(today.getFullYear(), 11, 31, 23, 59, 59);

    return [firstDayOfYear, lastDayOfYear];
}

function getDefaultTimeList(type) {
    var [startTime, endTime] = getCurrentTimeRange(type);
    return getXAxisValueByType(type, startTime, endTime);

}


function getTimeType() {
    return $('#date-type').val();
}

/**
 * Gets the IDs of selected checkboxes within the specified container.
 * @returns {Array} An array containing the IDs of selected checkboxes.
 */
function getUnitsSelected() {
    var checkboxes = document.querySelectorAll('#checkboxContainer input[type="checkbox"]');
    var selectedCheckboxes = [];

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            selectedCheckboxes.push(checkbox.id);
        }
    });

    return selectedCheckboxes;
}

function convertDateVnToStartDate(dateString) {


    var [datePart, timePart] = dateString.split(' ');

    var [day, month, year] = datePart.split('/');
    if (month === undefined) return `${day}/01/01`;
    if (year === undefined) return `${month}/${day}/01`;


    return `${year}/${month}/${day} ${timePart ? timePart: ''}`;
}

function convertDateJpToStartDate(dateString) {


    var [datePart, timePart] = dateString.split(' ');

    var [year, month, day] = datePart.split('/');
    if (month === undefined) return `${year}/01/01`;
    if (day === undefined) return `${year}/${month}/01`;


    return `${year}/${month}/${day} ${timePart ? timePart: ''}`;
}

function convertDateJpToEndDate(dateString) {
    var [datePart, timePart] = dateString.split(' ');

    var [year, month, day] = datePart.split('/');
    if (month === undefined) return `${year}/12/31 23:59:59`;
    if (day === undefined) return `${year}/${month}/${new Date(year, month, 0).getDate()} 23:59:59`;
    return `${year}/${month}/${day} 23:59:59`;
}

function convertDateVnToEndDate(dateString) {
    var [datePart, timePart] = dateString.split(' ');

    var [day, month, year] = datePart.split('/');
    if (month === undefined) return `${day}/12/31 23:59:59`;
    if (year === undefined) return `${month}/${day}/${new Date(month, day, 0).getDate()} 23:59:59`;
    return `${year}/${month}/${day} 23:59:59`;
}


function getTimeRange(timeType) {

    var startTimeValue = cultureName !== 'vi-VN' ? convertDateJpToStartDate(document.getElementById('start-time').value) : convertDateVnToStartDate(document.getElementById('start-time').value);

    var endTimeValue = cultureName !== 'vi-VN' ? convertDateJpToEndDate(document.getElementById('end-time').value) : convertDateVnToEndDate(document.getElementById('end-time').value);

    if (timeType === '3') {
        var dateObject = new Date(startTimeValue);

        dateObject.setHours(dateObject.getHours() + 1);

        var newTimeString = dateObject.toTimeString().slice(0, 5);
        endTimeValue = startTimeValue.split(' ')[0] + ' ' + newTimeString;

    }

    if (!isValidDate(new Date(startTimeValue)) || !isValidDate(new Date(endTimeValue)) || startTimeValue.includes('yyyy')) {
        return null;
    }

    return [startTimeValue, endTimeValue];
}

function isValidDate(date) {
    return date instanceof Date && !isNaN(date);
}
