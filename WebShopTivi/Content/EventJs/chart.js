$.getJSON("/Admin/Statistical/Chart", { year: 2019 },
    function (data) {
        chart(data)
    }
);
$('select').on('change', function () {
    $.getJSON("/Admin/Statistical/Chart", { year: this.value },
        function (data) {
            chart(data)
        }
    );
});
function chart(data) {
    Highcharts.chart('container', {
        chart: {
            type: 'line'
        },
        title: {
            text: 'Biểu đồ thống kê'
        },
        subtitle: {
            text: 'Shop Tivi'
        },
        xAxis: {
            categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
        },
        yAxis: {
            title: {
                text: 'VNĐ'
            }
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                },
                enableMouseTracking: false
            }
        },
        series: [{
            name: 'Giá nhập',
            data: data.data.stock
        }, {
            name: 'Bán ra',
            data: data.data.price
        }]
    });
}
$.getJSON("/Admin/Statistical/Chart", { year: 2020 },
    function (data) {
        console.log(data.data.stock)

    }
);