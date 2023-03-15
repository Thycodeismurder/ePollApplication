import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Poll } from './pollType';

const apiEndPoint =
  'https://v7lddz4dpb.execute-api.eu-west-1.amazonaws.com/Prod/';
const httpOptions = {
  headers: new HttpHeaders({ Origin: 'http:://localhost:4200' }),
};
httpOptions.headers.append('Content-Type', 'text/plain');

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
    const response = this.httpClient.post(
      apiEndPoint + 'polls/' + pollId + '/vote/' + optionId,
      JSON.stringify('')
    );
    return response;
  }
  createPoll(poll: Poll): Observable<{}> {
    const response = this.httpClient.post(
      apiEndPoint + 'polls/add',
      JSON.stringify(poll)
    );
    return response;
  }
}
