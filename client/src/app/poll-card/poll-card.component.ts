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
  @Output() votePollOption = new EventEmitter<{}>();

  constructor() {}
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['poll']) {
      console.log(this.poll);
    }
  }

  ngOnInit(): void {
    console.log(this.poll);
  }
  VoteOption(optionId?: number, pollId?: number) {
    this.votePollOption.emit({ optionId, pollId });
  }
}
