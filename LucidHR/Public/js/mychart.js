

//============================================
//Chartist
//============================================

if ($("#exampleStackedBar").length > 0) {
    var barLink = $(".mystacked-buttons").find(".active").attr("href")
    $.getJSON("/Home/salary?name=year", function (response) {
        if (response.status == 200) {
            new Chartist.Bar("#exampleStackedBar", {
                labels: ["Q1", "Q2", "Q3", "Q4"],
                series: [
                    [response.first.dev, response.second.dev, response.third.dev, response.fourth.dev],
                    [response.first.marketing, response.second.marketing, response.third.marketing, response.fourth.marketing],
                    [response.first.hr, response.second.hr, response.third.hr, response.fourth.hr],
                ]
            }, {
                    stackBars: !0,
                    axisY: {
                        labelInterpolationFnc: function (value) {
                            return value / 1000 + "k"
                        }
                    },
                    plugins: [
                        Chartist.plugins.tooltip({
                            appendBody: true
                        }),
                        Chartist.plugins.legend({
                            legendNames: ['Developer', 'Marketing', 'Human Resource']
                        })
                    ]
                }).on("draw", function (data) {
                    "bar" === data.type && data.element.attr({
                        style: "stroke-width: 30px"
                    })
                })
        }
    })
    $(".mystacked-buttons").find(".active").click(function (e) {
        e.preventDefault();
        $.getJSON("/Home/salary?name=year", function (response) {
            if (response.status == 200) {
                new Chartist.Bar("#exampleStackedBar", {
                    labels: ["Q1", "Q2", "Q3", "Q4"],
                    series: [
                        [response.first.dev, response.second.dev, response.third.dev, response.fourth.dev],
                        [response.first.marketing, response.second.marketing, response.third.marketing, response.fourth.marketing],
                        [response.first.hr, response.second.hr, response.third.hr, response.fourth.hr],
                    ]
                }, {
                        stackBars: !0,
                        axisY: {
                            labelInterpolationFnc: function (value) {
                                return value / 1000 + "k"
                            }
                        },
                        plugins: [
                            Chartist.plugins.tooltip({
                                appendBody: true
                            }),
                            Chartist.plugins.legend({
                                legendNames: ['Developer', 'Marketing', 'Human Resource']
                            })
                        ]
                    }).on("draw", function (data) {
                        "bar" === data.type && data.element.attr({
                            style: "stroke-width: 30px"
                        })
                    })
            }
        })
    })

}
//line chart
if ($("#LineChart").length > 0) {
   
    $.getJSON("/home/Fullsalary", function (response) {
        var deps = [];
        var depSalary = [];
        var i = 0;
        var months = [];
        $.each(response.data.depart, function (key, value) {
            deps.push(value.name)
            
            depSalary[i] = [value.mon[0].salary, value.mon[1].salary, value.mon[2].salary, value.mon[3].salary, value.mon[4].salary, value.mon[5].salary, value.mon[6].salary, value.mon[7].salary, value.mon[8].salary, value.mon[9].salary, value.mon[10].salary, value.mon[11].salary]
            i++;    
        })
        $.each(response.data.depart[0].mon, function (key, value) {
            months.push(value.month);
        })
        months = months.reverse();
        new Chartist.Line('#LineChart', {
            labels: [months[11], months[10], months[9], months[8], months[7], months[6], months[5], months[4], months[3], months[2], months[1], months[0]],
            series: depSalary
                
        }, {
                high: 50000,
                low: 0,
                // fullWidth: true,
                // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
                axisY: {
                    // onlyInteger: true,
                    // offset: 20
                },
                plugins: [
                    Chartist.plugins.tooltip({
                        appendBody: true
                    }),
                    Chartist.plugins.legend({
                        legendNames: deps
                    })
                ]
            });
    })

    $(".line-chart-buttons .active").click(function (e) {
        e.preventDefault();
        $.getJSON("/home/Fullsalary", function (response) {
            var deps = [];
            var depSalary = [];
            var i = 0;
            var months = [];
            $.each(response.data.depart, function (key, value) {
                deps.push(value.name)

                depSalary[i] = [value.mon[0].salary, value.mon[1].salary, value.mon[2].salary, value.mon[3].salary, value.mon[4].salary, value.mon[5].salary, value.mon[6].salary, value.mon[7].salary, value.mon[8].salary, value.mon[9].salary, value.mon[10].salary, value.mon[11].salary]
                i++;
            })
            $.each(response.data.depart[0].mon, function (key, value) {
                months.push(value.month);
            })
            months = months.reverse();
            new Chartist.Line('#LineChart', {
                labels: [months[11], months[10], months[9], months[8], months[7], months[6], months[5], months[4], months[3], months[2], months[1], months[0]],
                series: depSalary

            }, {
                    high: 50000,
                    low: 0,
                    // fullWidth: true,
                    // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
                    axisY: {
                        // onlyInteger: true,
                        // offset: 20
                    },
                    plugins: [
                        Chartist.plugins.tooltip({
                            appendBody: true
                        }),
                        Chartist.plugins.legend({
                            legendNames: deps
                        })
                    ]
                });
        })
    })
}



  
