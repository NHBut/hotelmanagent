$(document).ready(function () {
    console.log("hiii")
    if (typeof revenueData !== 'undefined' && revenueData.length) {
        var ctx = document.getElementById("revenueChart").getContext("2d");
        var revenueChart = new Chart(ctx, {
            type: "bar",
            data: {
                labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
                datasets: [{
                    label: "Doanh thu (VND)",
                    data: revenueData,
                    backgroundColor: "rgba(75, 192, 192, 0.2)",
                    borderColor: "rgba(75, 192, 192, 1)",
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
});
