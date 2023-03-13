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
  selectedPoll: Poll | undefined;
  selectedTabIndex = 0;

  ngOnInit(): void {
    this.apiService.getAll().subscribe((data: Poll[]) => {
      this.polls = data;
      this.selectedPoll = data[0];
    });
  }
  PostOption(event: { optionId?: number; pollId?: number }) {
    const optionIdString = event.optionId?.toString();
    const pollIdString = event.pollId?.toString();
    this.apiService
      .postOption(pollIdString, optionIdString)
      .subscribe((data) => {
        if (data) {
          this.polls?.map((poll) => {
            +pollIdString! === poll.PollId
              ? poll.Options[+optionIdString!].Votes++
              : null;
          });
        }
      });
  }
  SelectPoll(event: Poll) {
    this.selectedPoll = event;
    this.selectedTabIndex = 1;
  }
}
