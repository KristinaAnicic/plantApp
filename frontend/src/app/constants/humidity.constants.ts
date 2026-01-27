export interface LevelInfo {
    title: string;
    desc: string;
    icon: string;
}

export const HUMIDITY_LEVELS: Record<number, LevelInfo> = {
    1: { title: "Very Dry", desc: "Low humidity (below 30%). Ideal for cacti and succulents.", icon: "🏜️"},
    2: { title: "Dry", desc: "Standard indoor air. Typical for heated rooms in winter.", icon: "🌵"},
    3: { title: "Moderate", desc: "Comfortable levels (40-60%). Suitable for most houseplants.", icon: "🍃"},
    4: { title: "High", desc: "Moist air (60-80%). Perfect for tropical plants and ferns.", icon: "💧"},
    5: { title: "Very High", desc: "Steamy conditions (80%+). Requires a humidifier or greenhouse.", icon: "☁️"}
};