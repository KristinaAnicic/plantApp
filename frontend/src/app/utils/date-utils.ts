export class DateUtils {
    static formatDateForInput(dateString: string | null | undefined): string {
        if (!dateString) return '';
        
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return '';
        
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');

        return `${year}-${month}-${day}T${hours}:${minutes}`;
    }

    static formatForBackend(localDateValue: string | null | undefined): string | null {
        if (!localDateValue) return null;
        
        const date = new Date(localDateValue);
        return date.toISOString();
    }
}