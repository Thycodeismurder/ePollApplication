import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { Poll } from 'src/services/pollType';

@Component({
  selector: 'app-create-poll',
  templateUrl: './create-poll.component.html',
  styleUrls: ['./create-poll.component.scss'],
})
export class CreatePollComponent implements OnInit {
  @Output() createPoll = new EventEmitter<Poll>();
  poll = this.formBuilder.group({
    title: ['', [Validators.required]],
    options: this.formBuilder.array([]),
  });
  constructor(private formBuilder: FormBuilder) {}
  get options() {
    return this.poll.controls['options'] as FormArray;
  }
  ngOnInit(): void {}
  addOption() {
    const optionsForm = this.formBuilder.group({
      title: ['', [Validators.required]],
      id: [this.options.controls.length],
      votes: [0],
    });
    this.options.push(optionsForm);
  }
  onSubmit() {
    if (this.poll.valid) {
      console.log(this.poll.value);
      const poll: Poll = {
        Id: 0,
        Title: this.poll.value['title']!,
        PollId: '',
        Options: this.options.value,
      };
      this.createPoll.emit(poll);
    }
  }
}
