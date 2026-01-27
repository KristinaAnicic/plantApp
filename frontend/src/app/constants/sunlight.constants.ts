export interface LevelInfo {
    title: string;
    desc: string;
    icon: string;
}

export const SUN_LEVELS: Record<number, LevelInfo> = {
    1: { title: "Full Shade", desc: "No direct sunlight. Ideal for ferns.", icon: "🌑" },
    2: { title: "Low Light", desc: "Indirect light, far from windows.", icon: "☁️" },
    3: { title: "Partial Shade", desc: "Bright indirect light or soft morning sun.", icon: "⛅" },
    4: { title: "Partial Sun", desc: "4-6 hours of sunlight with afternoon shade.", icon: "🌤️" },
    5: { title: "Full Sun", desc: "6+ hours of intense, direct sunlight.", icon: "☀️" }
};