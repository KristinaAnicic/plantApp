import { Component, computed, inject, OnInit, Signal, signal } from '@angular/core';
import { NgApexchartsModule, ApexPlotOptions, ApexChart } from "ng-apexcharts";
import { AnalyticsDto, HealthPrediction } from '../../models/analytics.interface';
import { AnalyticsService } from '../../services/analytics.service';
import { DatePipe } from '@angular/common';

export type ChartOptions = {
  series: any;
  chart: ApexChart;
  plotOptions: ApexPlotOptions;
  legend?: ApexLegend;
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

@Component({
  selector: 'app-analytics',
  imports: [NgApexchartsModule, DatePipe],
  templateUrl: './analytics.html',
  styleUrl: './analytics.css',
})

export class Analytics implements OnInit {
  service = inject(AnalyticsService);
  analytics =  signal<AnalyticsDto | null>(null);
  selectedPrediction = signal<HealthPrediction | null>(null);

  ngOnInit(): void {
    this.service.getAnalytics().subscribe((res) => {
      this.analytics.set(res);
      this.selectedPrediction.set(res.healthPrediction[0]);
    })
  }

  survivalRate = computed(() => {
    const summary = this.analytics()?.summary;
    if (!summary || !summary?.numOfPlants || summary?.numOfPlants === 0) return 0;

    const rate = (summary.numOfCurrentPlants / summary.numOfPlants) * 100;
    return Math.round(rate);
  })

  private readonly statusColors: Record<string, string> ={
    'On Time': '#22c55e',
    'Delayed': '#f97316',
    'Late': '#ef4444'
  }

  performanceData = computed(() => {
    const stats = this.analytics()?.reminderStats || [];

    return stats.map( stat => ({
      label: stat.label,
      value: stat.percentage,
      color: this.statusColors[stat.label] || '#9ca3af'
    }))
  });

  private readonly healthColors: Record<string, string> ={
    'Healthy': '#22c55e',
    'Stressed': '#f97316',
    'Dormant': '#3b82f6'
  }

  healthData = computed(() => {
    const health = this.analytics()?.healthStats || [];

    return health.map( health => ({
      label: health.label,
      value: health.percentage,
      color: this.healthColors[health.label] || '#9ca3af'
    }))
  }); 

  actionData = computed(() => {
    const action = this.analytics()?.actionStats || [];
    const values: number[] = action.map(a => a.count)
    const labels: string[] = action.map(a => a.actionType)
    return {
      lables: labels,
      value: values
    }
  })

  public chartOptions: ChartOptions = {
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
          name: { 
            show: true,
            offsetY: -5
          },
          value: {
            show: true,
            fontSize: '16px',
            fontWeight: 'bold',
            offsetY: 5
          }
        }
      }
    },
  };


  donutChartOptions: Signal<ChartOptions> = computed(() => {
    const stats = this.analytics()?.actionStats || [];   
    return {
      series: stats.map(s => s.count),
      chart: {
        width: "100%",
        type: "donut",
      },
      labels: this.analytics()?.actionStats.map(s => s.actionType) || [],
      dataLabels: {
        enabled: false
      },
      fill: {
        type: "gradient"
      },
      legend: {
        formatter: function(val: any, opts: any) {
          return val + " - " + opts.w.globals.series[opts.seriesIndex];
        }
      },
      plotOptions: { },
      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200
            },
            legend: {
              position: "bottom"
            }
          }
        }
      ]
    }
  });

  logLineChartOptions: Signal<ChartOptions> = computed(() => {
    const activity = this.analytics()?.growthLogActivity || []; 
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    return {
      series: [
        {
          name: "Growth Logs",
          data: activity.map(s => s.count)
        }
      ],
      chart: {
        height: 300,
        type: "line",
        zoom: {
          enabled: false
        }
      },
      colors: ['#8922c5ff'],
      plotOptions: {},
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: "straight",
        width: 4
      },
      markers: {
        size: 4,
        hover: {
          size: 10
        }
      },
      grid: {
        clipMarkers: false,
        borderColor: '#f1f5f9'
      },
      xaxis: {
        type: "category",
        categories: activity.map(s => `${monthNames[s.month - 1]} ${s.year}`),
      },
    }
  });


  seasonalPlantingAreaChartOptions: Signal<ChartOptions> = computed(() => {
    const activity = this.analytics()?.seasonalPlanting || []; 
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    
    return {
      series: [
        {
          name: "Seasonal plantings",
          data: activity.map(s => s.count)
        }
      ],
      chart: {
        height: 300,
        width: "100%",
        type: "area",
        toolbar: { show: false }
      },
      plotOptions: {},
      colors: ['#22c55e'],
      dataLabels: { enabled: false },
      stroke: {
        curve: "smooth",
        width: 3
      },
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
      grid: {
        borderColor: '#f1f5f9'
      }
    }
  });


  predictionPlantLineChartOptions: Signal<ChartOptions> = computed(() => {
    const prediction = this.selectedPrediction(); 
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    return {
      series: [
        {
          name: "Health Score for " + (prediction?.plantName ?? 'Plant'),
          data: prediction?.monthlyPrediction ?? []
        }
      ],
      chart: {
        height: 300,
        type: "bar",
        toolbar: { show: false },
        zoom: { enabled: false },
      },
      colors: ['#68abd8ff'],
      plotOptions: {},
      dataLabels: { enabled: false },
      stroke: {
        curve: "smooth",
        width: 4
      },
      markers: {
        size: 4,
        hover: {
          size: 10
        }
      },
      grid: {
        clipMarkers: false,
        borderColor: '#f1f5f9'
      },
      xaxis: {
        type: "category",
        categories: monthNames,
      },
      yaxis: {
        min: 0,
        max: 100,
        tickAmount: 5,
        labels: {
          formatter: (val: any) => `${val.toFixed(0)}%`,
          style: { colors: '#64748b' }
        }
      },
    }
  });

  onPlantChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const index = Number(selectElement.value);
    
    const predictions = this.analytics()?.healthPrediction;
    
    if (predictions && predictions[index]) {
      this.selectedPrediction.set(predictions[index]);
    }
  }
}

