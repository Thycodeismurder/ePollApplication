import { Component, OnInit } from '@angular/core';
import { Poll } from 'src/services/pollType';
import { RestApiRequestsService } from 'src/services/rest-api-requests.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  constructor(private apiService: RestApiRequestsService) {}
  polls: Poll[] | undefined;

  ngOnInit(): void {
    this.apiService.getAll().subscribe((data: Poll[]) => {
      this.polls = data;
    });
  }
  PostOption(event: { optionId?: number; pollId?: number }) {
    const optionIdString = event.optionId?.toString();
    const pollIdString = event.pollId?.toString();
    this.apiService
      .postOption(optionIdString, pollIdString)
      .subscribe((data) => {
        console.log(data);
      });
  }
}
