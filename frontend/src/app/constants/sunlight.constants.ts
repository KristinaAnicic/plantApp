export interface LevelInfo {
    titleKey: string;
    descKey: string;
    icon: string;
}

export const SUN_LEVELS: Record<number, LevelInfo> = {
    1: { titleKey: "sun.level1.title", descKey: "sun.level1.desc", icon: "🌑" },
    2: { titleKey: "sun.level2.title", descKey: "sun.level2.desc", icon: "☁️" },
    3: { titleKey: "sun.level3.title", descKey: "sun.level3.desc", icon: "⛅" },
    4: { titleKey: "sun.level4.title", descKey: "sun.level4.desc", icon: "🌤️" },
    5: { titleKey: "sun.level5.title", descKey: "sun.level5.desc", icon: "☀️" }
};