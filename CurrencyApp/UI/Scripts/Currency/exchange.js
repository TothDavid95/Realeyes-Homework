$(document).ready(function () {

    $.getJSON("/Currency/GetCurrencies", null, function (data) {
        $(".currList option").remove();
        data.forEach(function (item) {
            $(".currList").append(
                $("<option>" + item + "</option>")
            )
        });
    });

    $(".currList").change(function () {
        $(".currList").each(function () {
            let currName = this.value;
            let currItem = $("." + $(this).attr("id"));

            $.ajax({
                traditional: true,
                type: "GET",
                url: "/Currency/GetLatestValue",
                data: {
                    currency: currName
                },
                dataType: "text",
                success: function (data) {
                    currItem.attr("data-value", parseFloat(data));
                    calculateExchangeRate();
                }
            });
        });
    });

    $("input[type=text]").on("input", function () {
        $(this).attr("value", parseFloat(this.value));
        calculateExchangeRate();
    })
});

function calculateExchangeRate() {
    $("#amountTo").attr("value", parseFloat($("#amountFrom").attr("value") * ($("#amountTo").attr("data-value") / $("#amountFrom").attr("data-value"))));
}

