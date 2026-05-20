import { Component, computed, inject, Input, OnInit, Signal, signal } from '@angular/core';
import { PlantGroupService } from '../../services/plant-group.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { PlantGroupGetDto, UpsertPlantGroupDto } from '../../models/plant-group.interface';
import { UpsertGrowthLogDto } from '../../models/growth-log.interface';
import { UpsertReminderDto } from '../../models/reminder.interface';
import { AddEditLogModal } from "../../components/add-edit-log-modal/add-edit-log-modal";
import { AddEditGroupModal } from "../../components/add-edit-group-modal/add-edit-group-modal";
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantGroupList } from "../../components/plant-group-list/plant-group-list";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";
import { PlantGroupAnalytics } from '../../models/analytics.interface';
import { AnalyticsService } from '../../services/analytics.service';
import { NgApexchartsModule, ApexPlotOptions, ApexChart, ChartComponent } from "ng-apexcharts";
import { TranslateModule, TranslateService } from '@ngx-translate/core';

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
  selector: 'app-plant-group',
  imports: [AddEditLogModal, AddEditGroupModal, PlantedGrowthLog, PlantGroupList, PlantedReminders, ChartComponent, TranslateModule],
  templateUrl: './plant-group.html',
  styleUrl: './plant-group.css',
})
export class PlantGroup implements OnInit {
  @Input() id!: string;
  currentGroupId = signal<number | null>(null);

  service = inject(PlantGroupService);
  analyticsService = inject(AnalyticsService);
  private router = inject(Router);
  public notif = inject(NotificationService);
  private route = inject(ActivatedRoute);
  translate = inject(TranslateService);

  group = signal<PlantGroupGetDto | null>(null);
  isEditGroupModalOpen = signal(false);
  isLogModalOpen = signal(false);
  isReminderModalOpen = signal(false);
  isManagePlantsModalOpen = signal(false);
  groupToEdit = signal<UpsertPlantGroupDto | null>(null);
  logToEdit = signal<UpsertGrowthLogDto | null>(null);
  reminderToEdit = signal<UpsertReminderDto | null>(null);
  showOnlyGroupLogs = signal(false);

  groupAnalytics = signal<PlantGroupAnalytics | null>(null);

  selectedYear = signal<number>(2025);
  availableYears = signal<number[]>([
    2023,
    2024,
    2025,
    2026
  ]);

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.currentGroupId.set(id);
      this.loadGroup();
      this.loadAnalytics();
    })
  }

  loadAnalytics(){
    const id = this.currentGroupId();
    if (!id) return;

    this.analyticsService.getGroupAnalytics(id, this.selectedYear()).subscribe({
      next: (result) => {
        this.groupAnalytics.set(result);
      },
      error: (err) => {
        console.log("Error while fetching analytics: ", err);
      }
    })
  }

  loadGroup(){
    this.isManagePlantsModalOpen.set(false);
    const id = this.currentGroupId();
    if (!id) return;

    this.service.getGroup(id).subscribe({
      next: (result) => {
        this.group.set(result);
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }


  toggleAddGroupModal(){
    this.isEditGroupModalOpen.update(val => !val);
  }

  toggleLogModal(){
    this.isLogModalOpen.update(val => !val);
  }

  toggleReminderModal(){
    this.isReminderModalOpen.update(val => !val);
  }

  togglePlantsModal(){
    this.isManagePlantsModalOpen.update(val => !val);
  }

  toggleShowLogsModal(){
    this.showOnlyGroupLogs.update(val => !val);
  }

  editGroup(){
    const currentPlanted = this.group();
    if (!currentPlanted) return;

    const plant: UpsertPlantGroupDto = {
      ...currentPlanted
    }
    this.groupToEdit.set(plant);
    this.isEditGroupModalOpen.set(true);
  }

  editLog(id: number){
    const log = this.group()?.growthLogs?.find(g => g.id === id);
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
    const reminder = this.group()?.reminders?.find(g => g.id === id);
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

  deleteGroup(){
    this.service.removeGroup(parseInt(this.id)).subscribe({
      next: () => {
        this.router.navigate(['/my-plants']);
        this.notif.showSuccess("Successfully deleted plant")
      },
      error: () => this.notif.showError("Couldn't remove plant, try again later!")
    });
  }

  openPlanted(id: number){
    this.router.navigate(['my-plants', id])
  }

  filteredlogs = computed(() => {
    const logs = this.group()?.growthLogs ?? [];

    if (this.showOnlyGroupLogs()){
      return logs.filter(l => l.plantGroupId !== null)
    }
    return logs;
  })

  onYearChange(event: Event) {
    const year = Number((event.target as HTMLSelectElement).value);
    this.selectedYear.set(year);
    this.loadAnalytics();
  }


  predictionPlantLineChartOptions: Signal<ChartOptions> = computed(() => {
    const analytics = this.groupAnalytics()?.groupLogAnalytics ?? []; 

    return {
      series: [
        {
          name: "Health Score",
          data: analytics.map(a => a.avgHealth)
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
        categories: analytics.map(p => new Date(0, p.month - 1).toLocaleString('en', { month: 'short' })),
      },
      yaxis: {
        min: 0,
        max: 100,
        tickAmount: 5,
        title: {
          text: this.translate.instant('analytics.plantHealthScore'),
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
  });


  plantGrowthLineChartOptions: Signal<ChartOptions> = computed(() => {
    const growthData = this.groupAnalytics()?.growthAnalytics ?? [];

    const firstWithData = growthData.find(g => g.plantGrowthHeight?.length);
    const categories = firstWithData?.plantGrowthHeight.map(h =>
      new Date(0, h.month - 1).toLocaleString('en', { month: 'short' })
    ) ?? ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];

    const series = growthData.map(g => {
      const monthMap = new Map((g.plantGrowthHeight ?? []).map(h => [h.month, Number(h.height.toFixed(2))]));
      const data = Array.from({length: 12}, (_, i) => monthMap.get(i + 1) ?? null);

      return{
        name: g.planted?.plantName ?? "Unknown Plant",
        data
      }
    });

    return {
      chart: {
        type: 'area',
        height: 300,
        width: '100%',
        toolbar: { show: false }
      },
      series: series,
      stroke: {
        curve: "smooth",
        width: 3
      },
      dataLabels: { enabled: false },
      xaxis: {
        categories
      },
      yaxis: {
        title: {
          text: this.translate.instant('analytics.estimatedPlantHeight'),
          style: {
            fontSize: '13px',
            fontWeight: 600,
            color: '#374151'
          }
        },
      },
      tooltip: {
        shared: true,
        intersect: false,
      },
      plotOptions: {},
      grid: {
        clipMarkers: false,
        borderColor: '#f1f5f9'
      },
    }
  });
}
