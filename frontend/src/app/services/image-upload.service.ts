import { Injectable } from '@angular/core';
import { Client, Storage, ID } from 'appwrite';
import { environment } from '../../environments/environment';
import { ImageForm, UploadMapping } from '../models/image.interface';

@Injectable({
  providedIn: 'root',
})

export class ImageUploadService {
  private client: Client;
  private storage: Storage;
  
  constructor() {
    this.client = new Client()
      .setEndpoint(environment.appwriteEndpoint)
      .setProject(environment.appwriteProjectId);
    this.storage = new Storage(this.client);
  }
  
  async uploadImages(imageObjects: ImageForm[]): Promise<UploadMapping[]>{
    const results: UploadMapping[] = [];

    for (const img of imageObjects) {
      if (!img.file) continue;

      try{
        const response = await this.storage.createFile({
          bucketId: environment.appwriteBucketId,
          fileId: ID.unique(),              
          file: img.file
        });
        const fileUrl = this.storage.getFileView({
          bucketId: environment.appwriteBucketId,
          fileId: response.$id
      });

        results.push({
          tempUrl: img.url, 
          serverUrl: fileUrl.toString() 
        });
      }
      catch (error) {
        console.error("Error while uploading files to firebase", error);
      }
    }
    return results;
  }
}
