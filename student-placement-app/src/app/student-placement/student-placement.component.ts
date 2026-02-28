import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

interface Student {
  id?: number | null;
  name?: string;
  course?: string;
  college?: string;
  placement?: boolean;
  comment?: string;
}

@Component({
  selector: 'app-student-placement',
  templateUrl: './student-placement.component.html',
  styleUrls: ['./student-placement.component.css']
})
export class StudentPlacementComponent implements OnInit {
  courses = ['B.Tech', 'MCA', 'MBA', 'BSc', 'MSc'];
  model: Student = { id: null, name: '', course: '', college: '', placement: false, comment: '' };

  reactiveForm: FormGroup;
  submittedTemplate = false;
  submittedReactive = false;
  templateResult: any = null;
  reactiveResult: any = null;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.reactiveForm = this.fb.group({
      id: [null, [Validators.required]],
      name: ['', [Validators.required]],
      course: ['', Validators.required],
      college: ['', Validators.required],
      placement: [false, Validators.required],
      comment: ['']
    });
  }

  onSubmitTemplate(form: any) {
    this.submittedTemplate = true;
    if (form.valid) {
      this.templateResult = form.value;
    }
  }

  onSubmitReactive() {
    this.submittedReactive = true;
    if (this.reactiveForm.valid) {
      this.reactiveResult = this.reactiveForm.value;
    }
  }

  resetTemplate(form: any) {
    form.resetForm();
    this.templateResult = null;
    this.submittedTemplate = false;
  }

  resetReactive() {
    this.reactiveForm.reset({ placement: false });
    this.reactiveResult = null;
    this.submittedReactive = false;
  }
}
