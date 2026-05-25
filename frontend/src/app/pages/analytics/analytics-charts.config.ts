import { ApexPlotOptions, ApexChart } from "ng-apexcharts";

export type ChartOptions = {
    series: any;
    chart: ApexChart;
    plotOptions: ApexPlotOptions;
    legend?: any;
    dataLabels?: any;
    fill?: any;
    responsive?: ApexResponsive[];
    labels?: any;
    colors?: any;
    stroke?: any;
    xaxis?: any;
    yaxis?: any;
    grid?: any;
    markers?: any;
};

const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

export const getRadialBarOptions = () : ChartOptions => ({
    series: [],
    chart: {
        height: "100%",
        type: "radialBar",
        sparkline: { enabled: true }
    },
    plotOptions: {
        radialBar: {
            hollow: { size: "65%" },
            dataLabels: {
                show: true,
                name: { show: true, offsetY: -5 },
                value: {
                    show: true,
                    fontSize: '16px',
                    fontWeight: 'bold',
                    offsetY: 5
                }
            }
        }
    },
});

export const getDonutChartOptions = (stats: any[]) : ChartOptions => ({
    series: stats.map(s => s.count),
    chart: {
        width: "100%",
        type: "donut",
    },
    labels: stats.map(s => s.actionType) || [],
    dataLabels: { enabled: false },
    colors: ['#75c76a', '#dfa946', '#6a8bc7', '#6e50c0', '#cc7db9', '#733948'],
    fill: { type: "gradient" },
    legend: {
        formatter: function(val: any, opts: any) {
            return val + " - " + opts.w.globals.series[opts.seriesIndex];
        }
    },
    plotOptions: { },
    responsive: [{
        breakpoint: 480,
        options: {
            chart: { width: 200 },
            legend: { position: "bottom" }
        }
        }
    ]
});

export const getLogLineChartOptions = (activity: any[], translateTitle: string) : ChartOptions => ({
    series: [{
        name: "Growth Logs",
        data: activity.map(s => s.count)
    }],
    chart: {
        height: 300,
        type: "line",
        zoom: { enabled: false }
    },
    colors: ['rgb(247, 173, 37)'],
    plotOptions: {},
    dataLabels: { enabled: false },
    stroke: { curve: "straight", width: 4 },
    grid: { clipMarkers: false, borderColor: '#f1f5f9' },
    xaxis: {
        type: "category",
        categories: activity.map(s => `${monthNames[s.month - 1]} ${s.year}`),
    },
    yaxis: {
        title: {
            text: translateTitle,
            style: { fontSize: '13px', fontWeight: 600, color: '#374151' }
        }
    }
});


export const getSeasonalPlantingAreaChartOptions = (activity: any[], translateTitle: string) : ChartOptions => ({
    series: [{
        name: "Seasonal plantings",
        data: activity.map(s => s.count)
    }],
    chart: {
        height: 300,
        width: "100%",
        type: "area",
        toolbar: { show: false }
    },
    plotOptions: {},
    colors: ['#22c55e'],
    dataLabels: { enabled: false },
    stroke: { curve: "smooth", width: 3 },
    fill: {
        type: "gradient",
        gradient: {
            shadeIntensity: 1,
            opacityFrom: 0.5,
            opacityTo: 0.1,
            stops: [0, 90, 100]
        }
    },
    xaxis: {
        type: "category",
        categories: activity.map(s => `${monthNames[s.month - 1]} ${s.year}`),
    },
    yaxis: {
        title: {
            text: translateTitle,
            style: { fontSize: '13px', fontWeight: 600, color: '#374151' }
        },
        min: 0
    },
    grid: { borderColor: '#f1f5f9' }
});

export const getPredictionPlantLineChartOptions = (prediction: any, translateTitle: string) : ChartOptions => {
    const startMonth = new Date().getMonth();
    const listedMonthNames = [
        ...monthNames.slice(startMonth),
        ...monthNames.slice(0, startMonth)
    ];

    return {
        series: [{
            name: "Health Score for " + (prediction?.plantName ?? 'Plant'),
            data: prediction?.monthlyPrediction ?? []
        }],
        chart: {
            height: 300,
            type: "bar",
            toolbar: { show: false },
            zoom: { enabled: false },
        },
        colors: ['#39735a'],
        plotOptions: {
            bar:{ horizontal: false, columnWidth: '90%' }
        },
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 4 },
        markers: { size: 4, hover: { size: 10 }},
        grid: { clipMarkers: false, borderColor: '#f1f5f9' },
        xaxis: {
            type: "category",
            categories: listedMonthNames,
        },
        yaxis: {
            min: 0,
            max: 100,
            tickAmount: 5,
            title: {
                text: translateTitle,
                style: {
                    fontSize: '13px',
                    fontWeight: 600,
                    color: '#374151'
                }
            },
            labels: {
                formatter: (val: any) => `${val.toFixed(0)}%`,
                style: { colors: '#64748b' }
            }
        },
    }
};

export const getGroupSuccessAreaChartOptions = (groupSuccess: any[], translateTitle: string) : ChartOptions => {
    const colors = groupSuccess.map((_, i) => {
        const lightness = 25 + i * 15;
        return `hsl(140, 45%, ${lightness}%)`;
    });
    
    return {
        series: [{ name: "", data: groupSuccess.map(s => s.percentage)}],
        chart: {
            height: 250,
            width: "100%",
            type: "bar",
            toolbar: { show: false }
        },
        plotOptions: {
            bar: { horizontal: true, distributed: true }
        },
        colors: colors,
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 3 },
        legend: { show: false },
        xaxis: {
            type: "category",
            categories: groupSuccess.map(s => s.label),
            title: {
                text: translateTitle,
                style: {fontSize: '13px', fontWeight: 600, color: '#374151'}
            }
        },
        yaxis: { min: 0, max: 100, tickAmount: 10, },
        grid: { borderColor: '#f1f5f9' }
    }
};

export const getFamilySuccessAreaChartOptions = (familySuccess: any[], translateTitle: string): ChartOptions => {
    /*const colors = familySuccess.map((_, i) => {
        const lightness = 25 + i * 15;
        return `hsl(210, 45%, ${lightness}%)`;
    });*/
    const colors = ['#39735a', '#1a8a72', '#395973', '#483973', '#733965', '#733948'];
    
    return {
        series: [{ data: familySuccess.map(s => s.percentage) }],
        chart: {
            height: 250,
            width: "100%",
            type: "bar",
            toolbar: { show: false }
        },
        plotOptions: {
            bar: { horizontal: true, distributed: true }
        },
        colors: colors,
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 3 },
        legend: { show: false },
        xaxis: {
            type: "category",
            categories: familySuccess.map(s => s.label),
            title: {
                text: translateTitle,
                style: { fontSize: '13px', fontWeight: 600, color: '#374151' }
            }
        },
        yaxis: { min: 0, max: 100 },
        grid: { borderColor: '#f1f5f9' }
    }
};



