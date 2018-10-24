$(document).ready(function () {

    $.getJSON("/Currency/GetCurrencies", null, function (data) {
        $("#switches option").remove();
        data.forEach(function (item) {
            $("#switches").append(
                $("<option" + " value=" + item + ">" + item + "</option>")
            )
        });
    });

    $("#currencySubmit").on("click", function () {
        var selectedOptions = $("option").filter(":selected").text();
        createAndRenderGraph(selectedOptions);
    });
});

function createAndRenderGraph(currenciesToSend) {
    $.ajax({
        traditional: true,
        type: "GET",
        url: "/Currency/GetDataOfCurrencies",
        data: {
            currencies: currenciesToSend
        },
        success: function (dataToRender) {
            $("#chart").remove();
            $("#chart_container").append('<div id="chart"></div>');
            $("#legend").remove();
            $("#legend_container").append('<div id="legend"></div>');
            createSeries(dataToRender);
        }
    });
};

function createSeries(seriesData) {
    var renderData = [];

    for (var i = 0; i < seriesData.names.length; i++) {

        var points = []

        for (var j = 0; j < seriesData.x.length; j++) {
            var tempPoint = {
                x: seriesData.x[j],
                y: seriesData.y[(i * seriesData.x.length) + j]
            };
            points[j] = tempPoint;
        }

        var tempData = {
            name: seriesData.names[i],
            data: points,
            color: seriesData.colour[i]
        }

        renderData[i] = tempData;
    }

    renderGraph(renderData, seriesData.min, seriesData.max);
};


function renderGraph(data, minValue, maxValue) {

    var graph = new Rickshaw.Graph({
        element: document.getElementById("chart"),
        height: 800,
        width: 800,
        min: minValue,
        max: maxValue,
        stack: false,
        renderer: 'line',
        series: data,
        tickFormat: function (y) { return y.toPrecision(5) }
    });

    graph.render();

    var hoverDetail = new Rickshaw.Graph.HoverDetail({
        graph: graph,
        yFormatter: function (y) { return y.toPrecision(5) + "€" }
    });

    var legend = new Rickshaw.Graph.Legend({
        graph: graph,
        element: document.getElementById('legend')

    });

    var shelving = new Rickshaw.Graph.Behavior.Series.Toggle({
        graph: graph,
        legend: legend
    });

    var axes = new Rickshaw.Graph.Axis.Time({
        graph: graph
    });
    axes.render();
}
