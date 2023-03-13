import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-create-poll',
  templateUrl: './create-poll.component.html',
  styleUrls: ['./create-poll.component.scss'],
})
export class CreatePollComponent implements OnInit {
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
    });
    this.options.push(optionsForm);
  }
  onSubmit() {
    console.log(this.poll.value);
  }
}
