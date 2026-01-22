export interface ImageDto {
    id: number;
    url: string;
    copyright?: string;
}

export interface ImageForm {
    url: string;
    file?: File;
}

export interface UploadMapping {
    tempUrl: string;
    serverUrl: string;
}