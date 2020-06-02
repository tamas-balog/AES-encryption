import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Round } from './models/Round';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-type': 'application/json' })
}

@Injectable({
  providedIn: 'root'
})
export class EncryptionService {

  baseUrl: string;

  constructor(private Http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
   }

  postText(text: string, encryptionKey: string) {
    let body = { "text": text, "encryptionKey": encryptionKey };
    return this.Http.post<{ Item1: string[], Item2: Round[], Item3: Round[] }>(this.baseUrl + 'api/encryption', body, httpOptions);
  }
}
