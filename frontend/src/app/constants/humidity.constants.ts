export interface LevelInfo {
    titleKey: string;    // promijenjeno iz title u titleKey
    descKey: string;     // promijenjeno iz desc u descKey
    icon: string;
}

export const HUMIDITY_LEVELS: Record<number, LevelInfo> = {
    1: { titleKey: "humidity.level1.title", descKey: "humidity.level1.desc", icon: "🏜️"},
    2: { titleKey: "humidity.level2.title", descKey: "humidity.level2.desc", icon: "🌵"},
    3: { titleKey: "humidity.level3.title", descKey: "humidity.level3.desc", icon: "🍃"},
    4: { titleKey: "humidity.level4.title", descKey: "humidity.level4.desc", icon: "💧"},
    5: { titleKey: "humidity.level5.title", descKey: "humidity.level5.desc", icon: "☁️"}
};