import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Poll } from './pollType';

const apiEndPoint =
  'https://v7lddz4dpb.execute-api.eu-west-1.amazonaws.com/Prod/';
const httpOptions = {
  headers: new HttpHeaders({ Authorization: 'iqRzw+ileNPu1fhspnRs8nOjjIA=' }),
};
httpOptions.headers.append('Content-Type', 'text/plain');
httpOptions.headers.append('Access-Control-Allow-Headers', 'Content-Type');
httpOptions.headers.append('Access-Control-Allow-Origin', '*');
httpOptions.headers.append('Access-Control-Allow-Methods', '*');

@Injectable({
  providedIn: 'root',
})
export class RestApiRequestsService {
  constructor(private httpClient: HttpClient) {}

  getAll(): Observable<Poll[]> {
    const response = this.httpClient.get<Poll[]>(apiEndPoint + 'polls');
    return response;
  }
  postOption(pollId?: string, optionId?: string): Observable<{}> {
    const response = this.httpClient.put(
      apiEndPoint + 'polls/' + pollId + '/vote/' + optionId,
      '',
      httpOptions
    );
    return response;
  }
}
