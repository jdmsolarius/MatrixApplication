function downloadDataset(dataSets, GeneKeyData, key) {

    if (key == "allDots") {
        var csvData = []
        var headers = []
        headers.push("EnsembleId")
        headers.push("GeneName")
        headers.push("PValue")
        headers.push("LogFC")
        headers.push("LogCPM")
        headers.push("FDR")
        csvData.push(headers)
        Object.entries(GeneKeyData).forEach(entry => {
            var miniList = []
            const [key, value] = entry;
            if (key != "Id" && key != "KvpRows" && key != "MatrixId" && key != "kvpRows" && key != "kvprows") {
                miniList.push(entry[1]["EnsembleId"])
                miniList.push(entry[1]["GeneName"])
                miniList.push(entry[1]["PValue"])
                miniList.push(entry[1]["LogFC"])
                miniList.push(entry[1]["LogCPM"])
                miniList.push(entry[1]["FDR"])
            }
            csvData.push(miniList)
        });
        return csvData
    }

    var textData = key
    var csvData = []
    for (var i = 0; i < dataSets[textData].length; i++) {
        var dot = GeneKeyData[dataSets[textData][i]["__data__"]["G"]]
        csvData.push(dot)
    }
    return csvData
}

function ToThreeDecimals(num) {
    let numStr = num.toString();
    let [coefficient, exponent] = numStr.split('e');
    let [integerPart, decimalPart] = coefficient.split('.');
    if (decimalPart && decimalPart.length > 3) {
        let roundedDecimalPart = (Math.round(parseFloat('0.' + decimalPart) * 1000) / 1000).toString().slice(2);
        while (roundedDecimalPart.length < 3) {
            roundedDecimalPart += '0';
        }
        return integerPart + '.' + roundedDecimalPart + (exponent ? 'e' + exponent : '');
    } else if (!decimalPart && !exponent) {
        return numStr + '.000';
    } else {
        return numStr;
    }
}
var tableBody = document.createElement('tbody');
function DataTableFromBox(element, genes, purpleDots, MatrixId, significanceThreshold, foldChangeThreshold) {
    var ran = false
    if (document.getElementById('TableId')) {
        $('#GeneTable').DataTable().destroy();
    }
    if (purpleDots == null || purpleDots == undefined)
        purpleDots = []
    var GeneNames = []
    for (var j = 0; j < genes.length; j++) {
        GeneNames.push(genes[j]["__data__"]["G"])
    }
    var geneValuePairs = false
    var datas = []
    $.ajax({
        url: '/Home/GetVolcanoRows?MatrixId=' + MatrixId + "&GeneList=" + GeneNames + "&ExperimentName=" + $('#SelectExperiment').text() + "&foldChangeThreshold=" + foldChangeThreshold + "&significanceThreshold=" + significanceThreshold,
        type: "GET",
        contentType: "application/x-www-form-urlencoded",
        async: false,
        success: function (dat) {
            data = JSON.parse(dat)
            keys = Object.keys(data[0])
            datas = Array.from(data)
        },
        error: function (err) {

        }
    });

    const result = datas[0].geneValuePairs.Entries.reduce((acc, kvp) => {
        if (!acc[kvp.Key]) {
            acc[kvp.Key] = [];
        }
        acc[kvp.Key].push(kvp.Value);
        return acc;
    }, {});

    for (var j = 0; j < genes.length; j++) {
        if (!$(genes[j]).css('fill', 'purple')) {
            purpleDots[$(genes[j])["__data__"]["G"]] = genes[j]
            $(genes[j]).css('fill', 'purple')
        }

        var table1 = document.getElementById('GeneTable');
        var tableHead = document.createElement('thead');



        var row = document.createElement('tr');
        var exclude = ["Id", "KVPRows", "MatrixId"]

        if (!document.getElementById('GeneTable').innerHTML.includes("<tr>")) {
            var rowOne = document.createElement('tr');
            for (i = 0; i < keys.length; i++) {
                if (exclude.includes(keys[i])) {
                    continue
                }
                if (keys[i] == "sampleValuePairs") {
                    continue;
                }
                if (keys[i] == "geneValuePairs") {
                    var key = datas[0].geneValuePairs.Entries[0].Key;
                    var object = result[key]
                    var values = Object.values(object);
                    var sortedArr = values.sort(function (a, b) {
                        // Get the first key of each object
                        var keyA = Object.keys(a)[0];
                        var keyB = Object.keys(b)[0];

                        // Compare the keys alphabetically
                        if (keyA < keyB) {
                            return -1;
                        }
                        if (keyA > keyB) {
                            return 1;
                        }
                        return 0;
                    });
                    for (var l = 0; l < sortedArr.length; l++) {
                        try {
                            var cell = document.createElement('td');
                            var text = Object.keys(sortedArr[l])[0]
                            if (key != datas[0].geneValuePairs.Entries[l].Key) {
                                break;
                            }
                            cell.appendChild(document.createTextNode(text));
                            rowOne.appendChild(cell);
                        }
                        catch (ex) {
                            var cell = document.createElement('td');
                            var text = "Error" + ex
                            cell.appendChild(document.createTextNode(text));
                            rowOne.appendChild(cell);
                        }

                    }
                    sampleValuePairs = true
                    continue;
                }
                var cell1 = document.createElement('td');
                var text1 = document.createTextNode(keys[i]);

                cell1.appendChild(text1);
                rowOne.appendChild(cell1);

            }
            if (!document.getElementById('TableId')) {

                table1.appendChild(tableHead)
                tableHead.appendChild(rowOne);
            }

        }
        var appendRow = true
        for (i = 0; i < keys.length; i++) {
            if (exclude.includes(keys[i])) {
                continue
            }
            var cell1 = document.createElement('td');
            var text1 = document.createTextNode(datas[j][keys[i]]);
            if (keys[i] == "Gene") {
                var link = document.createElement('a')
                link.href = "https://www.genecards.org/cgi-bin/carddisp.pl?gene=" + datas[j][keys[i]] + "&keywords=u%5D"
                link.target = 'blank'
                link.innerHTML = datas[j][keys[i]]
                cell1.appendChild(link)
                row.appendChild(cell1);
                purpleDots[datas[j][keys[i]]] = genes[j]
            }
            else if (keys[i] == "EnsembleId") {
                var link = document.createElement('a')
                link.href = "https://www.ebi.ac.uk/ebisearch/search?query=" + datas[j][keys[i + 1]] + "&requestFrom=ebi_index&db=allebi"
                link.target = 'blank'
                link.innerHTML = datas[j][keys[i]]
                cell1.appendChild(link)
                row.appendChild(cell1);
                if ($(tableBody).text().includes(datas[j]["EnsembleId"])) {
                    appendRow = false
                }
            }
            else if (keys[i] == "sampleValuePairs") {
                continue;
            }
            else if (keys[i] == "geneValuePairs") {
                for (var k = 0; k < Object.values(result)[i].length; k++) {
                    try {
                        var text = result[genes[j]["__data__"]["G"]].sort()
                        var final = Object.values(Object.values(text)[k])[0]
                        var cell = document.createElement('td');
                        cell.appendChild(document.createTextNode(ToThreeDecimals(final)))
                        row.appendChild(cell);
                    }
                    catch (ex) {
                        var text = "error"
                        var cell = document.createElement('td');
                        cell.appendChild(document.createTextNode(text));
                        row.appendChild(cell);
                    }
                }
                sampleValuePairs = true
                continue;
            }
            else if (keys[i] == "sampleValuePairs") {
                continue;
            }
            else {
                text1.data = ToThreeDecimals(text1.data)
                text1.textContent = text1.data.toString()
                cell1.appendChild(text1);
                row.appendChild(cell1);
            }
        }
        if (ran != false && appendRow == true) {
            if (document.getElementById('GeneTable').innerHTML.includes("TableId")) {
                tableBody = document.getElementById('TableId')
            }
            tableBody.appendChild(row);
        }
        else {
            if (!document.getElementById('GeneTable').innerHTML.includes("TableId")) {
                tableBody = document.createElement('tbody');
                tableBody.id = "TableId"
                tableBody.appendChild(row);

            }
            else {
                tableBody = document.getElementById('TableId')
                tableBody.appendChild(row);
            }
            ran = true

        }


        finished = true

    }

    table1.appendChild(tableBody)
    document.getElementById("clearTable").style.visibility = "visible";
    purpleDots = deleteButton('#deleteButton', purpleDots)

    return purpleDots;
}
function Distance(x1, y1, x2, y2) {
    let xDifference = x2 - x1;
    let yDifference = y2 - y1;

    return Math.sqrt(Math.pow(xDifference, 2) + Math.pow(yDifference, 2));
}
function get10(element, purpleDots, MatrixId) {
    if ($('#click10').is(":checked")) {
        var meetsSigFold = $('.sigfold')
        let distances = $(meetsSigFold).map(el => {
            return {
                el,
                distance: Distance(
                    $(element).attr('cx'),
                    $(element).attr('cy'),
                    meetsSigFold[el].getAttribute('cx'),
                    meetsSigFold[el].getAttribute('cy')
                )
            }
        });
        distances.sort((a, b) => a.distance - b.distance);
        let closest10Elements = distances.slice(0, 10).map(item => item.el);
        var GeneList = []
        for (var i = 0; i < closest10Elements["prevObject"].length; i++) {
            var num = closest10Elements["prevObject"][i]["el"]
            $(num).click();
            var gene = meetsSigFold[num]
            $(gene).click();
            GeneList.push(gene)
        }
        purpleDots = DataTableFromBox(element, GeneList, purpleDots, MatrixId)
    }
    return purpleDots
}
function hasDuplicates(array) {
    return new Set(array).size !== array.length;
}
table1 = document.getElementById('GeneTable');
tableBody.id = "TableId"
function DataTableFromPlot(purpleDots, MatrixId, significanceThreshold, foldChangeThreshold, searchHappening = true, finished = false) {
    TurnOnDeleteButton();
    var finished = finished;
    if (purpleDots == null || purpleDots == undefined)
        purpleDots = []
    document.querySelectorAll('circle').forEach(function (element) {
        element.addEventListener("click", function () {
            if (document.getElementById('TableId') && !searchHappening) {
                $('#GeneTable').DataTable().destroy();
            }
            var got10 = false;
            if ($('#click10').is(":checked")) {
                purpleDots = get10(this, purpleDots, MatrixId)
                got10 = true
            }

            var keys = null
            var datas = null
            $.ajax({
                url: '/Home/GetVolcanoRow?MatrixId=' + MatrixId + "&Gene=" + this["__data__"]["G"] + "&ExperimentName=" + $(SelectExperiment).text() + "&foldChangeThreshold=" + foldChangeThreshold + "&significanceThreshold=" + significanceThreshold,
                type: "GET",
                contentType: "application/x-www-form-urlencoded",
                async: false,
                success: function (dat) {
                    data = JSON.parse(dat)
                    keys = Object.keys(data)
                    datas = data

                },
                error: function (err) {
                    alert(err.statusText);
                }
            });

            var table1 = document.getElementById('GeneTable');
            var tableHead = document.createElement('thead');
            var tableBody = document.createElement('tbody');
            tableBody.id = "TableId"
            var row = document.createElement('tr');
            var exclude = ["Id", "KVPRows", "MatrixId"]
         
            if (!document.getElementById('GeneTable').innerHTML.includes("<tr>")) {
                var rowOne = document.createElement('tr');
                for (i = 0; i < keys.length; i++) {
                    if (exclude.includes(keys[i])) {
                        continue
                    }
                    var cell1 = document.createElement('td');
                    var text1 = document.createTextNode(keys[i]);

                    if (keys[i] == "geneValuePairs") {
                        var obj = datas[keys[i]]
                        var newkeys = Object.keys(obj);
                        newkeys.sort();
                        for (var j = 0; j < Object.keys(obj.Values[0]).length; j++) {
                            try {
                                var cell = document.createElement('td');
                                var key = Object.keys(obj.Values[0])[j]
                                cell.appendChild(document.createTextNode(key));
                                rowOne.appendChild(cell);
                            }
                            catch (ex) {
                                var cell = document.createElement('td');
                                var text = "Error"
                                cell.appendChild(document.createTextNode(text));
                                rowOne.appendChild(cell);
                            }
                        }
                        sampleValuePairs = true
                        continue;
                    }
                    if (keys[i] == "geneValuePairs") {
                        continue;
                    }
                    cell1.appendChild(text1);
                    rowOne.appendChild(cell1);


                }
                table1.appendChild(tableHead)
                tableHead.appendChild(rowOne);

            }
            for (i = 0; i < keys.length; i++) {
                if (exclude.includes(keys[i])) {
                    continue
                }
                var cell1 = document.createElement('td');
                var text1 = document.createTextNode(datas[keys[i]]);

                if (keys[i] == "Gene") {
                    var link = document.createElement('a')
                    link.href = "https://www.genecards.org/cgi-bin/carddisp.pl?gene=" + datas[keys[i]] + "&keywords=u%5D"
                    link.target = 'blank'
                    link.innerHTML = datas[keys[i]]
                    cell1.appendChild(link)
                    row.appendChild(cell1);

                }
                else if (keys[i] == "geneValuePairs") {
                    var obj = datas[keys[i]]
                    var newkeys = Object.keys(obj);
                    newkeys.sort();
                    for (var j = 0; j < Object.keys(obj.Values[0]).length; j++) {
                        try {
                            var cell = document.createElement('td');
                            var value = Object.values(obj.Values[0])[j]
                            var newText = ToThreeDecimals(value.toString());
                            cell.appendChild(document.createTextNode(newText));
                            row.appendChild(cell);

                        }
                        catch (ex) {
                            var cell = document.createElement('td');
                            var text = "Error" + ex
                            cell.appendChild(document.createTextNode(text));
                            row.appendChild(cell);
                        }
                    }
                }
                else if (keys[i] == "sampleValuePairs") {
                    continue;
                }
                else if (keys[i] == "EnsembleId") {
                    var link = document.createElement('a')
                    link.href = "https://www.ebi.ac.uk/ebisearch/search?query=" + datas[keys[i + 1]] + "&requestFrom=ebi_index&db=allebi"
                    link.target = 'blank'
                    link.innerHTML = datas[keys[i]]
                    cell1.appendChild(link)
                    row.appendChild(cell1);
                }
                else {
                    text1.data = ToThreeDecimals(text1.data)
                    text1.textContent = text1.data
                    cell1.appendChild(text1);
                    row.appendChild(cell1);
                }
            }
            if (searchHappening) {
                finished = true;
            }
            if (!finished && !got10) {
                if (document.getElementById('TableId')) {
                    alert(JSON.stringify(row))
                    tableBody = document.getElementById('TableId')
                    tableBody.appendChild(row)
                }
                else {
                    table1.appendChild(tableBody)
                    tableBody.appendChild(row);
                }

                if (!$(this).css('fill', 'purple')) {
                    purpleDots[$(this).attr('sampleID')] = $(this);
                    $(this).css('fill', 'purple')
                }
                destroyCreateDataTable('#GeneTable', true)
                TurnOnDeleteButton('#GeneTable_paginate', '#GeneTable tbody')


                document.getElementById("clearTable").style.visibility = "visible";

                document.getElementById("deleteButton").style.visibility = "visible";

                purpleDots[$(this)[0]["__data__"]["G"]] = this
                purpleDots = deleteButton('#deleteButton', purpleDots)
            }
            else {

                purpleDots = deleteButton('#deleteButton', purpleDots)
                document.getElementById("clearTable").style.visibility = "visible";

                document.getElementById("deleteButton").style.visibility = "visible";
                TurnOnDeleteButton('#GeneTable_paginate', '#GeneTable tbody')
                var DataTable = $('#GeneTable').DataTable();
                DataTable.row.add($(row)).draw(true)
                var alreadyDeleted = false
                if (!$(this).css('fill', 'purple')) {
                    purpleDots[$(this)[0]["__data__"]["G"]] = this
                    $(this).css('fill', 'purple')
                    alreadyDeleted = false
                }
             
                purpleDots[$(this)[0]["__data__"]["G"]] = this
                var data = $('#GeneTable').DataTable().rows().data()
                var map = []
                data.each(function (value, index) {
                    var element = value  // arr[i] is the element in the array at position i
                    if (!map[element]) {
                        map[element] = [index];
                    }
                    else {
                        map[element].push(index);
                    }
                });
                for (var element in map) {
                    if (map[element].length === 1) {
                        delete map[element];
                    }
                }
                if (Object.values(map)?.length > 0 && !got10) {
                    var key = Object.keys(map)
                    var indexTwo = map[key[0]]
                    $(this).css('fill', '')
                    if (purpleDots[$(this)[0]["__data__"]["G"]]) {
                        alreadyDeleted = true
                        delete purpleDots[$(this)[0]["__data__"]["G"]]
                    }
                    $('#GeneTable').DataTable().rows(indexTwo).remove().draw(true)
                }
                if (got10) {
                    var numRows = $('#GeneTable').DataTable().rows().count()
                    $('#GeneTable').DataTable().row(numRows - 1).remove().draw(true)
                    $('#GeneTable').DataTable({
                        dom: 'Bfrtip',
                        buttons: [
                            'colvis',
                            'copy',
                            'csv',
                            'excel',
                            'print'
                        ],
                        pageLength: 25,
                        colReorder: true,
                        rowReorder: true,
                        ordering: false,
                        unique: true,
                        select: true,
                        destroy: true
                    });
                    document.getElementById("click10").checked = false
                }
                else {
                    var currentFillColor = $(this).css('fill')
                    if (currentFillColor == 'rgb(128, 0, 128)') {
                        var rows = $('#GeneTable').DataTable().rows().count()
                        var purpleDotsArray = Object.keys(purpleDots).length;
                        if (rows == purpleDotsArray) {
                            $('#GeneTable').DataTable({
                                dom: 'Bfrtip',
                                buttons: [
                                    'colvis',
                                    'copy',
                                    'csv',
                                    'excel',
                                    'print'
                                ],
                                pageLength: 25,
                                colReorder: true,
                                rowReorder: true,
                                ordering: false,
                                unique: true,
                                select: true,
                                destroy: true
                            });
                            return purpleDots;
                        }
                        finished = true;

                        if (numRows > purpleDotsArray) {
                            var index = Object.keys(purpleDots).indexOf($(this)[0]["__data__"]["G"]);
                            if (index > -1) {
                                delete purpleDots[$(this)[0]["__data__"]["G"]]
                                $('#GeneTable').DataTable().rows(index).remove().draw(true)
                                $('#GeneTable').DataTable().row(numRows - 2).remove().draw(true)
                                $('#GeneTable').DataTable({
                                    dom: 'Bfrtip',
                                    buttons: [
                                        'colvis',
                                        'copy',
                                        'csv',
                                        'excel',
                                        'print'
                                    ],
                                    pageLength: 25,
                                    colReorder: true,
                                    rowReorder: true,
                                    ordering: false,
                                    unique: true,
                                    select: true,
                                    destroy: true
                                });
                                $(this).css('fill', '')
                            }
                            else {
                                tableBody.appendChild(row);
                            }
                        }
                   
                    }
                }
            }
            finished = true

            if (!searchHappening) {
                $('#GeneTable').DataTable({
                    dom: 'Bfrtip',
                    buttons: [
                        'colvis',
                        'copy',
                        'csv',
                        'excel',
                        'print'
                    ],
                    pageLength: 25,
                    colReorder: true,
                    rowReorder: true,
                    ordering: false,
                    unique: true,
                    select: true,
                    destroy: true
                });
                SyncDots(purpleDots)
            }
          

        })
    })
 
    return purpleDots
}
function debounce(func, wait) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            timeout = null;
            func.apply(context, args);
        }, wait);
    };
}
function volcanoPlot(significanceThreshold, foldChangeThreshold) {
    var width = 1200,
        height = 600,
        margin = { top: 20, right: 20, bottom: 40, left: 50 },
        xColumn, // name of the variable to be plotted on the axis
        yColumn,
        xAxisLabel, // label for the axis
        yAxisLabel,
        xAxisLabelOffset, // offset for the label of the axis
        yAxisLabelOffset,
        xTicks, // number of ticks on the axis
        yTicks,
        sampleID = "G",
        significanceThreshold, // significance threshold to colour by
        foldChangeThreshold, // fold change level to colour by
        colorRange, // colour range to use in the plot
        xScale = d3.scaleLinear(), // the values for the axes will be continuous
        yScale = d3.scaleLog();

    function chart(selection) {
        var innerWidth = width - margin.left - margin.right, // set the size of the chart within its container
            innerHeight = height - margin.top - margin.bottom;
        var iterator = 0;
        selection.each(function (data) {
            iterator++
 
            // set up the scaling for the axes based on the inner width/height of the chart and also the range
            // of value for the x and y axis variables. This range is defined by their min and max values as
            // calculated by d3.extent()
            xScale.range([0, innerWidth])
                .domain(d3.extent(data, function (d) { return d[xColumn]; }))
                .nice();

            // normally would set the y-range to [height, 0] but by swapping it I can flip the axis and thus
            // have -log10 scale without having to do extra parsing
            yScale.range([0, innerHeight])
                .domain(d3.extent(data, function (d)
                {
                    return d[yColumn];
                }))
                .nice(); // adds "padding" so the domain extent is exactly the min and max values
            
            var zoom = d3.zoom()
                .scaleExtent([1, 20])
                .translateExtent([[0, 0], [width, height]])
                .on('zoom', zoomFunction);

            // append the svg object to the selection
            var svg = d3.select(this).append('svg')
                .attr('height', height)
                .attr('width', width)
                .append('g')
                .attr('transform', 'translate(' + margin.left + ',' + margin.top + ')')
                .call(zoom);

            svg.append('defs').append('clipPath')
                .attr('id', 'clip')
                .append('rect')
                .attr('height', innerHeight)
                .attr('width', innerWidth);

            // add the axes
            var xAxis = d3.axisBottom(xScale);
            var yAxis = d3.axisLeft(yScale)
                .ticks(5)
                .tickFormat(yTickFormat);

            var gX = svg.append('g')
                .attr('class', 'x axis')
                .attr('transform', 'translate(0,' + innerHeight + ')')
                .call(xAxis);

            gX.append('text')
                .attr('class', 'label')
                .attr('transform', 'translate(' + width / 2 + ',' + (margin.bottom - 6) + ')')
                .attr('text-anchor', 'middle')
                .html(xAxisLabel || xColumn);

            var gY = svg.append('g')
                .attr('class', 'y axis')
                .call(yAxis);

            gY.append('text')
                .attr('class', 'label')
                .attr('transform', 'translate(' + (0 - margin.left / 1.25) + ',' + (height / 2) + ') rotate(-90)')
                .style('text-anchor', 'middle')
                .html(yAxisLabel || yColumn);

            // this rect acts as a layer so that zooming works anywhere in the svg. otherwise, if zoom is called on
            // just svg, zoom functionality will only work when the pointer is over a circle.
            svg.append('rect')
                .attr('class', 'zoom')
                .attr('height', innerHeight)
                .attr('width', innerWidth);

            var circles = svg.append('g').attr('class', 'circlesContainer');

            circles.selectAll(".dot").style = "opacity:0.2"

            var tooltip = d3.select('body')
                .append("div")
                .attr('class', 'tooltip')
                .attr('id', 'originalTooltip');

            function tipEnter(d) {
                try {
                    tooltip.style('visibility', 'visible')
                        .style('font-size', '11px')
                        .attr('id', d.currentTarget["__data__"][sampleID])
                        .html(
                            '<strong>' + "Gene" + '</strong>: ' + d.currentTarget["__data__"][sampleID] + '<br/>' +
                            '<strong>' + $('#XAxisItem').val() + '</strong>: ' + d3.format('.3f')(d.currentTarget["__data__"][xColumn]) + '<br/>' +
                            '<strong>' + "PValue" + '</strong>: ' + ToThreeDecimals(d.currentTarget["__data__"][yColumn].toString())
                        );
                }
                catch (ex) {
                    tooltip.style('visibility', 'visible')
                        .style('font-size', '11px')
                        .attr('id', d[sampleID])
                        .html(
                            '<strong>' + "Gene" + '</strong>: ' + d[sampleID] + '<br/>' +
                            '<strong>' + $('#XAxisItem').val() + '</strong>: ' + d3.format('.3f')(d[xColumn]) + '<br/>' +
                            '<strong>' + "PValue" + '</strong>: ' + ToThreeDecimals(d[yColumn].toString())
                        );
                }
                tipMove(tooltip)
            }


            function tipMove(tooltip) {
                tooltip.style("top", (event.pageY - 5) + "px")
                    .style("left", (event.pageX + 20) + "px");
            }


            circles.selectAll(".dot").data(data).on('mouseenter', tipEnter)
                .on("mousemove", tipMove)
                .enter().append('circle')
                .attr('r', 3)
                .attr('cx', function (d) { return xScale(d[xColumn]); })
                .attr('cy', function (d) {
                    try {
                        return yScale(d[yColumn]);
                    }
                    catch
                    {
                        return -1;
                    }
                })
                .attr('sampleID', function (d) { return d[sampleID]; })
                .attr('class', circleClass)
                .on('mouseenter', tipEnter)
                .on("mousemove", tipMove(tooltip))
                .on('mouseleave', function (d) {
                    return tooltip.style('visibility', 'hidden');
                })



            var thresholdLines = svg.append('g')
                .attr('class', 'thresholdLines');

            // add horizontal line at significance threshold
            thresholdLines.append("svg:line")
                .attr('class', 'threshold')
                .attr("x1", 0)
                .attr("x2", innerWidth)
                .attr("y1", yScale(significanceThreshold))
                .attr("y2", yScale(significanceThreshold));

            // add vertical line(s) at fold-change threshold (and negative fold-change)
            [foldChangeThreshold, -1 * foldChangeThreshold].forEach(function (threshold) {
                thresholdLines.append("svg:line")
                    .attr('class', 'threshold')
                    .attr("x1", xScale(threshold))
                    .attr("x2", xScale(threshold))
                    .attr("y1", 0)
                    .attr("y2", innerHeight);
            });

            var map = { dots: d3.selectAll('.dot'), threshold: svg.selectAll('.threshold'), circles: circles.selectAll(".dot") }


            function yTickFormat(n) {
                return d3.format(".2r")(getBaseLog(10, n));
                function getBaseLog(x, y) {
                    return Math.log(y) / Math.log(x);
                }
            }

            function zoomFunction() {
                var transform = d3.zoomTransform(this);
                map.dots
                    .attr('transform', transform)
                    .attr('r', 3 / Math.sqrt(transform.k));
                try {
                    gX.call(xAxis.scale(d3.event.transform.rescaleX(xScale)));
                    gY.call(yAxis.scale(d3.event.transform.rescaleY(yScale)));
                }
                catch
                {

                }
                map.threshold.attr('transform', transform).attr('stroke-width', 1 / transform.k);
            }

            function circleClass(d) {
                if (d[yColumn] <= significanceThreshold && Math.abs(d[xColumn]) >= foldChangeThreshold) return 'dot sigfold';
                else if (d[yColumn] <= significanceThreshold) return 'dot sig';
                else if (Math.abs(d[xColumn]) >= foldChangeThreshold) return 'dot fold';
                else return 'dot';
            }
            iterator++
        });
    }

    chart.width = function (value) {
        if (!arguments.length) return width;
        width = value;
        return chart;
    };

    chart.height = function (value) {
        if (!arguments.length) return height;
        height = value;
        return chart;
    };

    chart.margin = function (value) {
        if (!arguments.length) return margin;
        margin = value;
        return chart;
    };

    chart.xColumn = function (value) {
        if (!arguments.length) return xColumn;
        xColumn = value;
        return chart;
    };

    chart.yColumn = function (value) {
        if (!arguments.length) return yColumn;
        yColumn = value;
        return chart;
    };

    chart.xAxisLabel = function (value) {
        if (!arguments.length) return xAxisLabel;
        xAxisLabel = value;
        return chart;
    };

    chart.yAxisLabel = function (value) {
        if (!arguments.length) return yAxisLabel;
        yAxisLabel = value;
        return chart;
    };

    chart.xAxisLabelOffset = function (value) {
        if (!arguments.length) return xAxisLabelOffset;
        xAxisLabelOffset = value;
        return chart;
    };

    chart.yAxisLabelOffset = function (value) {
        if (!arguments.length) return yAxisLabelOffset;
        yAxisLabelOffset = value;
        return chart;
    };


    chart.xTicks = function (value) {
        if (!arguments.length) return xTicks;
        xTicks = value;
        return chart;
    };

    chart.yTicks = function (value) {
        if (!arguments.length) return yTicks;
        yTicks = value;
        return chart;
    };

    chart.significanceThreshold = function (value) {
        if (!arguments.length) return significanceThreshold;
        significanceThreshold = value;
        return chart;
    };

    chart.foldChangeThreshold = function (value) {
        if (!arguments.length) return foldChangeThreshold;
        foldChangeThreshold = value;
        return chart;
    };

    chart.colorRange = function (value) {
        if (!arguments.length) return colorRange;
        colorRange = value;
        return chart;
    };

    chart.sampleID = function (value) {
        if (!arguments.length) return sampleID;
        sampleID = value;
        return chart;
    };

    return chart;
}



function popTooltips(allDots, input) {

    //get the genes whose tooltips we need to pop
    var genesToFind = []
    var gene = ""
    if (input.includes(",")) {
        for (var i = 0; i < input.length; i++) {
            if (input[i] == ",") {
                genesToFind.push(gene.trim())
                gene = ""
            }
            else {
                gene += input[i]
            }
        }
        genesToFind.push(gene.trim())
    }
    else {
        genesToFind.push(input.trim())
    }
    const GeneMap = new Map();

    Object.values(allDots).forEach(obj => {
        const id = obj.G.toUpperCase();
        try {
            GeneMap.set(id, obj);
        }
        catch (ex) {
            //there should not be duplicates here but I don't want a total failure if there are one or two.
            console.log(ex)
        }
    });
    var tooltips = document.getElementsByClassName('tooltip')
    for (var j = 0; j < genesToFind.length; j++) {
        for (var i = 0; i < tooltips.length; i++) {

            if ($(tooltips[i]).attr('id', genesToFind[j].toUpperCase())) {
                $(tooltips[i]).remove()
            }
        }
    }
    for (var i = 0; i < genesToFind.length; i++) {
        if (GeneMap.has(genesToFind[i].toUpperCase())) {
            var gene = GeneMap.get(genesToFind[i].toUpperCase())
            var genes = document.getElementsByTagName('circle')
            for (var j = 0; j < genes.length; j++) {
                if (genes[j].getAttribute("sampleID")?.toUpperCase() == gene["G"]?.toUpperCase()) {
                    var newTooltip = document.createElement('div');
                    newTooltip.className = 'tooltip';
                    newTooltip.id = gene['G']?.toUpperCase();
                    document.body.appendChild(newTooltip);
                    newTooltip.style.visibility = 'visible';
                    newTooltip.style.fontSize = '11px';
                    newTooltip.innerHTML =
                        "<strong>'" + "Gene" + '</strong>: ' + gene["G"] + '<br/>' +
                        '<strong>' + $('#XAxisItem').val() + '</strong>: ' + gene["L"].toFixed(2) + '<br/>' +
                        '<strong>' + "PValue)" + '</strong>: ' + gene["P"] + "</b>";
                    var offset = $(genes[j]).offset();
                    var y = offset.top - 5;
                    var x = offset.left + 15;
                    $('#' + gene["G"]?.toUpperCase()).css({
                        position: "absolute",
                        top: y + "px",
                        left: x + "px"
                    });
                }
            }

        }

    }

    return genesToFind;

}
function getDatasets(significanceThreshold, foldChangeThreshold, allTheDots) {
    var datasets = {}
    var meetsDot = allTheDots
    var AllDots = []
    var meetsSigOnlyPositive = []
    var meetsSigOnlyNegative = []
    var meetsFoldOnlyPositive = []
    var meetsFoldOnlyNegative = []
    var meetsSigFoldPositive = []
    var meetsSigFoldNegative = []
    var meetsSigOnly = []
    var MeetsSigFold = []
    var meetsFoldOnly = []
    var MeetsDot = []
    for (var i = 0; i < meetsDot.length; i++) {
        AllDots.push(meetsDot[i])
    }




    var meetsSigFold = document.getElementsByClassName('sigfold')
    for (var i = 0; i < meetsSigFold.length; i++) {
        MeetsSigFold.push(meetsSigFold[i])
    }
    /*
    var meetsSigOnly = document.getElementsByClassName('dot sig')
    var MeetsSigOnly = []
    for (var i = 0; i < meetsSigOnly.length; i++) {
        MeetsSigOnly.push(meetsSigOnly[i])
    }
    var meetsFoldOnly = document.getElementsByClassName('fold')
    var MeetsFoldOnly = []
    for (var i = 0; i < meetsFoldOnly.length; i++) {
        MeetsFoldOnly.push(meetsFoldOnly[i])
    }
    for (var i = 0; i < meetsDot.length; i++) {
        if (!$(meetsDot[i]).hasClass('sigfold') || $(meetsDot[i]).hasClass('fold')  || $(meetsDot[i]).hasClass('sig'))
        {
            MeetsDot.push(meetsDot[i]);
        }
    }

    for (var i = 0; i < MeetsSigOnly.length; i++) {
        if (meetsSigOnly[i]["__data__"]["L"] > 0) {
            meetsSigOnlyPositive.push(MeetsSigOnly[i]);
        }
        else {
            meetsSigOnlyNegative.push(MeetsSigOnly[i])
        }
    }
    for (var i = 0; i < MeetsFoldOnly.length; i++) {
        if (meetsFoldOnly[i]["__data__"]["L"] > 0) {
            meetsFoldOnlyPositive.push(MeetsFoldOnly[i]);
        }
        else {
            meetsFoldOnlyNegative.push(MeetsFoldOnly[i])
        }
    }
    */
    for (var i = 0; i < MeetsSigFold.length; i++) {
        if (meetsSigFold[i]["__data__"]["L"] >= 0) {
            meetsSigFoldPositive.push(MeetsSigFold[i]);
        }
        else {
            meetsSigFoldNegative.push(MeetsSigFold[i])
        }
    }
    datasets["meetsSigFold"] = MeetsSigFold;
    datasets["meetsSigOnly"] = meetsSigOnly
    datasets["meetsFoldOnly"] = meetsFoldOnly
    datasets["meetsSigFoldPositive"] = meetsSigFoldPositive;
    datasets["meetsSigFoldNegative"] = meetsSigFoldNegative;
    datasets["meetsSigOnlyPositive"] = meetsSigOnlyPositive;
    datasets["meetsSigOnlyNegative"] = meetsSigOnlyNegative;
    datasets["meetsFoldOnlyPositive"] = meetsFoldOnlyPositive;
    datasets["meetsFoldOnlyNegative"] = meetsFoldOnlyNegative;
    datasets["meetsDot"] = MeetsDot;
    datasets["allDots"] = allTheDots;
    return datasets;
}