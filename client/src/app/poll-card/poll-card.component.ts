import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Poll } from 'src/services/pollType';

@Component({
  selector: 'app-poll-card',
  templateUrl: './poll-card.component.html',
  styleUrls: ['./poll-card.component.scss'],
})
export class PollCardComponent implements OnInit, OnChanges {
  @Input() poll?: Poll;
  @Input() showContent: boolean = false;
  @Output() votePollOption = new EventEmitter<{}>();
  @Output() selectPollEmiter = new EventEmitter<Poll>();

  constructor() {}
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['poll']) {
      console.log(this.poll);
    }
  }

  ngOnInit(): void {
    console.log(this.poll);
  }
  VoteOption(optionId?: number, pollId?: string) {
    this.votePollOption.emit({ optionId, pollId });
  }
  selectPoll() {
    this.selectPollEmiter.emit(this.poll);
  }
}
