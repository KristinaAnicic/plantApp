export interface ImageDto {
    id: number;
    url: string;
    copyright?: string;
}

export interface ImageForm {
    url: string;
    file?: File;
}