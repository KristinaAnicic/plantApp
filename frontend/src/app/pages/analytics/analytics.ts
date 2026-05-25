import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { NgApexchartsModule } from "ng-apexcharts";
import { AnalyticsDto, HealthPrediction } from '../../models/analytics.interface';
import { AnalyticsService } from '../../services/analytics.service';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import * as ChartConfigs from './analytics-charts.config';

@Component({
  selector: 'app-analytics',
  imports: [NgApexchartsModule, DatePipe, TranslateModule],
  templateUrl: './analytics.html',
  styleUrl: './analytics.css',
})

export class Analytics implements OnInit {
  service = inject(AnalyticsService);
  router = inject(Router);
  translate = inject(TranslateService);
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
    const values = [89, 2, 9]
    return health.map( (health, index) => ({
      label: health.label,
      value: values[index] ?? 0,
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

  chartOptions = ChartConfigs.getRadialBarOptions();

  donutChartOptions = computed(() => 
    ChartConfigs.getDonutChartOptions(this.analytics()?.actionStats || [])
  );

  logLineChartOptions = computed(() => {
    return ChartConfigs.getLogLineChartOptions(
      this.analytics()?.growthLogActivity || [], 
      this.translate.instant('analytics.numOfLogs'));
  });

  seasonalPlantingAreaChartOptions = computed(() =>
    ChartConfigs.getSeasonalPlantingAreaChartOptions(
      this.analytics()?.seasonalPlanting || [],
      this.translate.instant('analytics.numOfPlantedPlants')
  ));

  predictionPlantLineChartOptions = computed(() =>
    ChartConfigs.getPredictionPlantLineChartOptions(
      this.selectedPrediction(),
      this.translate.instant('analytics.plantHealthScore')
  ));
    
  groupSuccessAreaChartOptions = computed(() =>
    ChartConfigs.getGroupSuccessAreaChartOptions(
      this.analytics()?.groupPlantSuccess || [],
      this.translate.instant('analytics.successScore')
  ));
    
  familySuccessAreaChartOptions = computed(() => 
    ChartConfigs.getFamilySuccessAreaChartOptions(
      this.analytics()?.familyPlantSuccess || [],
      this.translate.instant('analytics.successScore')
  ));

  onPlantChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const index = Number(selectElement.value);
    
    const predictions = this.analytics()?.healthPrediction;
    
    if (predictions && predictions[index]) {
      this.selectedPrediction.set(predictions[index]);
    }
  }

  search(query: string){
    this.router.navigate(['/'], {
      queryParams: { search: query, page: 1 }
    })
  }
}

