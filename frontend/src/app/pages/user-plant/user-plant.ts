import { Component, computed, inject, Input, OnInit, Signal, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedGetDto, UpsertPlantedDto } from '../../models/planted.interface';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";
import { AddEditPlantedModal } from "../../components/add-edit-planted-modal/add-edit-planted-modal";
import { AddEditLogModal } from "../../components/add-edit-log-modal/add-edit-log-modal";
import { UpsertGrowthLogDto } from '../../models/growth-log.interface';
import { UpsertReminderDto } from '../../models/reminder.interface';
import { AddEditReminderModal } from "../../components/add-edit-reminder-modal/add-edit-reminder-modal";
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { AnalyticsService } from '../../services/analytics.service';
import { PlantedAnalyticsDto } from '../../models/analytics.interface';
import { NgApexchartsModule, ApexPlotOptions, ApexChart } from "ng-apexcharts";

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
  tooltip?: any;
};

@Component({
  selector: 'app-user-plant',
  imports: [PlantedGrowthLog, PlantedReminders, AddEditPlantedModal, AddEditLogModal, AddEditReminderModal, NgApexchartsModule],
  templateUrl: './user-plant.html',
  styleUrl: './user-plant.css',
})
export class UserPlant implements OnInit {
  @Input() id!: string;
  currentPlantId = signal<number | null>(null);

  service = inject(PlantedService);
  analyticsService = inject(AnalyticsService);
  private router = inject(Router);
  public notif = inject(NotificationService);
  private route = inject(ActivatedRoute);

  planted = signal<PlantedGetDto | null>(null);
  plantedAnalytics = signal<PlantedAnalyticsDto | null>(null);

  isEditPlantedModalOpen = signal(false);
  isLogModalOpen = signal(false);
  isReminderModalOpen = signal(false);
  plantedToEdit = signal<UpsertPlantedDto | null>(null);
  logToEdit = signal<UpsertGrowthLogDto | null>(null);
  reminderToEdit = signal<UpsertReminderDto | null>(null);

  displayImages = computed(() => {
    const all = this.planted()?.images?.filter(im => im.url !== this.planted()?.image) ?? [];
    return all.length > 4 ? all.slice(0, 3) : all.slice(0, 4);
  })

  hasMoreImages = computed(() => (this.planted()?.images?.length ?? 0) > 4);

  plantName = computed(() => {
    const planted = this.planted();
    if (!planted) return;

    return [
      planted.plant.commonName,
      planted.plant.botanicalName
    ]
    .filter(val => !!val)
    .join(' • ');
  });

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.currentPlantId.set(id);
      this.loadPlanted();
      this.loadAnalytics();
    })
  }

  loadAnalytics(){
    const id = this.currentPlantId();
    if (!id) return;

    this.analyticsService.getPlantedAnalytics(id).subscribe({
      next: (result) => {
        this.plantedAnalytics.set(result);
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }

  loadPlanted(){
    const id = this.currentPlantId();
    if (!id) return;

    this.service.getPlanted(id).subscribe({
      next: (result) => {
        this.planted.set(result);
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }

  statusInfo = computed(() => {
    const statusId = this.planted()?.plantStatus?.id;

    if (statusId && PLANT_STATUS_MAP[statusId]) {
      return PLANT_STATUS_MAP[statusId];
    }

    return { 
      name: 'Not specified', 
      category: PlantStatusCategory.Inactive, 
      color: 'bg-gray-100 text-gray-500 border-gray-200' 
    };
  });

  toggleAddPlantedModal(){
    this.isEditPlantedModalOpen.update(val => !val);
  }

  toggleLogModal(){
    this.isLogModalOpen.update(val => !val);
  }

  toggleReminderModal(){
    this.isReminderModalOpen.update(val => !val);
  }

  editPlanted(){
    const currentPlanted = this.planted();
    if (!currentPlanted) return;

    const plant: UpsertPlantedDto = {
      ...currentPlanted,
      plantId: currentPlanted.plant.plantId,
      placeId: currentPlanted.place.id,
      plantGroupId: currentPlanted.plantGroup?.id,
      plantStatusId: currentPlanted.plantStatus?.id ?? 0,
      datePlanted: currentPlanted.datePlanted.split('T')[0],
      images: currentPlanted.images?.map(im => im.url) ?? []
    }
    this.plantedToEdit.set(plant);
    this.isEditPlantedModalOpen.set(true);
  }

  editLog(id: number){
    const log = this.planted()?.growthLogs?.find(g => g.id === id);
    if (!log) return;

    const editLog: UpsertGrowthLogDto = {
      ...log,
      plantStatusId: log.plantStatus?.id ?? 0,
      images: log.images?.map(im => im.url) ?? []
    }
    this.logToEdit.set(editLog);
    this.isLogModalOpen.set(true);
  }

  addLog(){
    this.logToEdit.set(null);
    this.isLogModalOpen.set(true);
  }

  editReminder(id: number){
    const reminder = this.planted()?.nextReminders?.find(g => g.id === id);
    if (!reminder) return;

    const editReminder: UpsertReminderDto = {
      ...reminder,
      frequencyTypeId: reminder.frequencyType?.id ?? 0,
      reminderTypeId: reminder.reminderType?.id ?? 0
    }
    this.reminderToEdit.set(editReminder);
    this.isReminderModalOpen.set(true);
  }

  addReminder(){
    this.reminderToEdit.set(null);
    this.isReminderModalOpen.set(true);
  }

  deletePlanted(){
    this.service.removePlanted(parseInt(this.id)).subscribe({
      next: () => {
        this.router.navigate(['/my-plants']);
        this.notif.showSuccess("Successfully deleted plant")
      },
      error: () => this.notif.showError("Couldn't remove plant, try again later!")
    });
  }


  predictionPlantLineChartOptions: Signal<ChartOptions> = computed(() => {
    const prediction = this.plantedAnalytics()?.monthlyHealthPrediction ?? []; 
    //const prediction = [52.123028, 95.62226, 95.40444, 93.167946, 56.04227, 53.11005, 52.71085, 50.555016, 51.05584, 50.635567, 51.635567, 49.123028]; 
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const startMonth = new Date().getMonth();
    const listedMonthNames = [
      ...monthNames.slice(startMonth),
      ...monthNames.slice(0, startMonth)
    ];

    return {
      series: [
        {
          name: "Health Score",
          data: prediction
        }
      ],
      chart: {
        height: 300,
        width: '100%',
        type: "bar",
        toolbar: { show: false },
        zoom: { enabled: false },
      },
      colors: ['#39735a'],
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
        categories: listedMonthNames,
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


  plantGrowthLineChartOptions: Signal<ChartOptions> = computed(() => {
    const plantGrowthHeight = this.plantedAnalytics()?.plantGrowthHeight ?? []; 
    const allAttributes = ['Foliage', 'Stem', 'Fruit', 'Flower'];

    const attributeSeries = allAttributes.map((attr, index) => ({
      name: attr,
      type: 'scatter',
      data: plantGrowthHeight.map(p => {
        if (!p.activeAttributes.includes(attr)) 
          return null;

        const visualIndex = p.activeAttributes.indexOf(attr);
        return (p.height + visualIndex * 0.05).toFixed(2);
      }),
      color: attr?.toLowerCase() === 'foliage' ? '#3dd374' : undefined
    }));

    return {
      chart: {
        type: 'area',
        height: 300,
        width: '100%',
        toolbar: { show: false }
      },
      series: [{
        name: 'Height',
        data: plantGrowthHeight.map(p => p.height.toFixed(2)),
        color: '#14532d',
      },
      ...attributeSeries
      ],
      stroke: {
        curve: "smooth",
        width: 3
      },
      dataLabels: { enabled: false },
      xaxis: {
        type: "Month",
        categories: plantGrowthHeight.map(p => new Date(0, p.month - 1).toLocaleString('en', { month: 'short' }))
      },
      tooltip: {
        shared: false,
        intersect: false,
      },
      fill: {
        type: 'solid',
        opacity: 0.12
      },
      plotOptions: {},
      grid: {
        clipMarkers: false,
        borderColor: '#f1f5f9'
      },
    }
  });

}
