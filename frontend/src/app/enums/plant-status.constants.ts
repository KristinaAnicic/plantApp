export enum PlantStatusCategory {
    Healthy = 'Healthy',
    Stressed = 'Stressed',
    Inactive = 'Inactive'
}

export const PLANT_STATUS_MAP: Record<number, { name: string; category: PlantStatusCategory; color: string}> = {
    1: { name: 'Healthy', category: PlantStatusCategory.Healthy, color: 'bg-green-100 text-green-700 border-green-200' },
    5: { name: 'Growing', category: PlantStatusCategory.Healthy, color: 'bg-emerald-100 text-emerald-700 border-emerald-200' },
    6: { name: 'Flowering', category: PlantStatusCategory.Healthy, color: 'bg-pink-100 text-pink-700 border-pink-200' },
    7: { name: 'Fruiting', category: PlantStatusCategory.Healthy, color: 'bg-lime-100 text-lime-700 border-lime-200' },
    8: { name: 'Seedling', category: PlantStatusCategory.Healthy, color: 'bg-green-50 text-green-600 border-green-100' },
    11: { name: 'Transplanted', category: PlantStatusCategory.Healthy, color: 'bg-green-100 text-teal-700 border-teal-200' },

    2: { name: 'Sick', category: PlantStatusCategory.Stressed, color: 'bg-red-100 text-red-700 border-red-200' },
    4: { name: 'Wilting', category: PlantStatusCategory.Stressed, color: 'bg-orange-100 text-orange-700 border-orange-200' },
    10: { name: 'Stressed', category: PlantStatusCategory.Stressed, color: 'bg-yellow-100 text-yellow-700 border-yellow-200' },
    12: { name: 'Dormant', category: PlantStatusCategory.Stressed, color: 'bg-blue-100 text-blue-700 border-blue-200' },

    3: { name: 'Dead', category: PlantStatusCategory.Inactive, color: 'bg-gray-200 text-gray-700 border-gray-300' },
    9: { name: 'Harvested', category: PlantStatusCategory.Inactive, color: 'bg-purple-100 text-purple-700 border-purple-200' }
}