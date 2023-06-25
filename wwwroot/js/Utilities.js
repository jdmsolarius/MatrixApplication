function adjustLabel(xLabel) {
    let tick = Array.from(document.getElementsByClassName('tick'))
    for (var i = 0; i < tick.length; i++) {
        if (tick[i]["__data__"] == 0) {
            let tickTransform = tick[i].getAttribute('transform');
            let tickX = tickTransform.match(/translate\((.*),/)[1];
            let text = document.querySelector('.label')
            text.setAttribute('x', tickX);
            text.setAttribute('y', 34);
            text.removeAttribute('transform')
            text.innerHTML = xLabel;
        }
    }
}

function SyncDots(purpleDots) {
    if (!$.fn.dataTable.isDataTable('#GeneTable')) {
        return
    }
    var theKeys = Object.entries(purpleDots)
    for (var i = 0; i < theKeys.length; i++) {
        theKeys[i].value
    }
    var items = $('#GeneTable').DataTable({
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
    }).column(1).data();
    var cleaneditems = []
    for (var l = 0; l < items.length; l++) {
        cleaneditems.push(items[l].split('>')[1].replace("</a",""))
    } 
    var tempDots = document.getElementsByClassName('dot');
    var tempDotsArray = Array.from(tempDots);
    for (var j = 0; j < tempDotsArray.length; j++) {
        for (var k = 0; k < Object.values(cleaneditems).length; k++)
        {
            if (tempDotsArray[j]["__data__"]["G"].toUpperCase() == cleaneditems[k].toUpperCase()) {
                var color = $(tempDotsArray[j]).css('fill');
                if (color != 'rgb(128, 0, 128)') {
                    $(tempDotsArray[j]).css('fill', 'purple')
                }
            }
        }
    }
    return purpleDots;
}

function arrayToCsv(data, exclude = ["Id", "KVPRows", "MatrixId"]) {
    var finalData = [], headerList = [], indexes = [], keys = [], tempList = [];
    //deal with a list of objects
    if (!Array.isArray(data[0])) {
        for (var i = 0; i < Object.keys(data[0]).length; i++) {
            if (exclude.includes(Object.keys(data[0])[i])) {
                indexes.push(i);
            } else {
                headerList.push(Object.keys(data[0])[i]);
            }
        }
        finalData.push(headerList);
        keys = Object.keys(data[0]);
        for (var i = 0; i < data.length; i++) {
            tempList = [];
            for (var j = 0; j < keys.length; j++) {
                if (indexes.includes(j)) {
                    continue;
                } else {
                    tempList.push(data[i][keys[j]]);
                }
            }
            finalData.push(tempList);
        }
    } else
        finalData = data
    return finalData.map(row => row.map(String).map(v => v.replaceAll('"', '""')).map(v => !isNaN(v) ? v : `"${v}"`).join(',')).join('\r\n');
}
function Distinct(arr) {
    var a = [];
    for (var i = 0, l = arr.length; i < l; i++)
        if (a.indexOf(arr[i]) === -1 && arr[i] !== '')
            a.push(arr[i]);
    return a;
}

function fetchFile(file, output) {
    const $file = document.getElementById(file);
    const $output = document.getElementById(output);
    $file.onchange = async e => {
        const [file] = e.target.files;
        const text = await file.text();
        $output.textContent = text;
    };
}

function fetchFiles() {
    fetchFile('file', 'output');
    fetchFile('file2', 'output2');
    fetchFile('file3', 'output3');
    fetchFile('file4', 'output4');
}

function downloadBlob(content, filename, contentType) {
    var blob = new Blob([content], { type: contentType });
    var url = URL.createObjectURL(blob);

    // Create a link to download the blob
    var pom = document.createElement('a');
    pom.href = url;
    pom.setAttribute('download', filename);
    pom.click();
}

function customBox(x1x2y1y2, dat) {
    if (x1x2y1y2 == null || x1x2y1y2?.length != 4) {
        alert("all four fields are required")
        return;
    }
    var dots = document.getElementsByClassName('dot');
    var dotsArray = []
    for (var j = 0; j < dots.length; j++) {
        dotsArray.push(dots[j])
    }
    var x1 = parseFloat(x1x2y1y2[0])
    var x2 = parseFloat(x1x2y1y2[1])
    var y1 = parseFloat(x1x2y1y2[2])
    var y2 = parseFloat(x1x2y1y2[3])
    if (x1 > x2 || y1 > y2) {
        alert("x1 must be less than x2 and y1 must be less than y2")
        return;
    }
    var saved = []
    for (var i = 0; i < dotsArray.length; i++) {
        var foldChange = dotsArray[i]["__data__"]["L"]
        var NegativeLogPValue = dotsArray[i]["__data__"]["P"];
        if ((foldChange > x1 && foldChange < x2) && (NegativeLogPValue > y1 && NegativeLogPValue < y2)) {
            saved.push(dotsArray[i])
        }
    }
    if (saved.length < 200) {
        if (Object.keys(purpleDots).length == 0) {
            purpleDots = DataTableFromBox(saved, purpleDots)
        }
        else if (!Object.keys(purpleDots).length == 0) {
            var interm = DataTableFromBox(saved, purpleDots)
            for (var j = 0; j < interm.length; j++) {
                purpleDots[interm[j]] = interm[j]
            }
        }
    }
    else {
        var setForDownload = downloadDataset(saved, JSON.parse(dat[1]), "customDataset")
        var finalSet = arrayToCsv(setForDownload)
        downloadBlob(finalSet, SampleA + "vs" + SampleB + "_" + selText + ".csv", 'text/csv;charset=utf-8;')
    }
}

function remove(arr, itemToRemove) {
    const index = arr.indexOf(itemToRemove);
    if (index > -1) {
        arr.splice(index, 1);
    }
    return arr;
}

function hideGreyDots(percentage) {
    if (percentage == 0 || percentage == 100) {
        return;
    }
    else {

    }
    var allDots = document.getElementsByClassName('dot')
    var first = true
    for (var i = 0; i < allDots.length; i++) {
        if (first) {
            const duplicates = Array.from(allDots).reduce((acc, dot) => {
                const key = `${Math.round(dot.__data__.L * 100) / 100}:${Math.round(-Math.log10(dot.__data__.P) * 100) / 100}`;
                acc[key] = acc[key] || [];
                acc[key].push(dot);
                return acc;
            }, {});
            const duplicatesArray = Object.values(duplicates).filter(arr => arr.length > 1);
            for (var k = 0; k < duplicatesArray.length; k++) {
                if ($(allDots[k]).hasClass('fold') || $(allDots[k]).hasClass('sigfold')) {
                    continue;
                }
                for (var j = 0; j < duplicatesArray[k].length - 1; j++) {
                    $(duplicatesArray[j]).hide()
                }
            }
            first = false;
        }
        if ($(allDots[i]).hasClass('fold') || $(allDots[i]).hasClass('sigfold') || $(allDots[i]).hasClass('sig')) {
            continue
        }
    }
}

function optionDropdown(id = 'DownloadableData') {
    document.getElementById(id).onchange = function () {
        selText = $("#DownloadableData option:selected").text();
        if (selText == "Select an option") {
            alert("you must select an option")
            return;
        }
        if (selText == "customBox") {
            $('#x1').css('visibility', 'visible');
            $('#x2').css('visibility', 'visible');
            $('#y1').css('visibility', 'visible');
            $('#y2').css('visibility', 'visible');
            $('#optionText').css('visibility', 'visible');
            $('#DataSetDownload').css('visibility', 'visible');
            $('#DownloadText').css('visibility', 'visible');
        }
        else {
            $('#DataSetDownload').css('visibility', 'visible');
            $('#DownloadText').css('visibility', 'visible');
            $('#x1').css('visibility', 'hidden');
            $('#x2').css('visibility', 'hidden');
            $('#y1').css('visibility', 'hidden');
            $('#y2').css('visibility', 'hidden');
            $('#optionText').css('visibility', 'hidden');
            $('#DownloadText').css('visibility', 'hidden');
        }
    }
}

function destroyCreateDataTable(id, destroy = true) {
    if ($.fn.DataTable.isDataTable('#GeneTable')) {
        try {
            $('#GeneTable').DataTable().destroy();
        }
        catch
        {

        }
    }
    try {
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
            select: true
        });
    }
    catch
    {

    }
    TurnOnDeleteButton();
}

function clearTable(id = 'clearTable', purpleDots) {
    document.getElementById(id).onclick = function () {
        $('#GeneTable').DataTable().clear().draw(true)
        var dots = document.getElementsByClassName('dot')
        for (var i = 0; i < dots.length; i++) {
            $(dots[i]).css('fill', '')
        }
        destroyCreateDataTable("#GeneTable")
    }
    TurnOnDeleteButton();
    return []
}

function deleteButton(id = '#deleteButton', purpleDots) {
    $(id).on('click', function () {
        try {
            var row = $('#GeneTable').DataTable().row('.selected')
            var gene = row.data()[1].split('>')[1].replace("</a", "");
            var circle = purpleDots[gene]
            $(circle).css('fill', "")
            $('#GeneTable').DataTable().row('.selected').remove().draw(true);
            if (purpleDots[gene]) {
                delete purpleDots[gene]
            }
        }
        catch
        {
            $('#GeneTable').DataTable().row('.selected').remove().draw(true);
        }
    });
    return purpleDots;
}

function TurnOnDeleteButton() {

    $('#GeneTable_paginate').css('margin-top', '11px')
    var geneTable = $('#GeneTable tbody');
    geneTable.off('click', 'tr', clickHandler);
    geneTable.off('click', 'tr');
    geneTable.off('click');
    var clickHandler = function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            $('#GeneTable tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    }
    geneTable.on('click', 'tr', clickHandler);
}



function makeVisible() {
    $('#downloadDownRegulated').css('visibility', 'visible')
    $('#downloadUpRegulated').css('visibility', 'visible')
    $('#downloadEntireDataset').css('visibility', 'visible')
    $('#HideGreyDots').css('visibility', 'visible')
    $('#clickDots').css('visibility', 'visible')
    $('#click10').css('visibility', 'visible')
    $('#click10Text').css('visibility', 'visible')
    $('#performance').css('visibility', 'visible')
}