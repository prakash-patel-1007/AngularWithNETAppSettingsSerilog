import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { ComponentService } from '../services/component.service';

@Component({
  standalone: false,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private componentService: ComponentService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
    this.componentService.updateResult(false);
  }

  ngOnInit(): void {
    if (this.route.snapshot.queryParams['expired']) {
      this.errorMessage = 'Session expired — please sign in again.';
    }
  }

  login(): void {
    if (this.form.invalid) return;
    this.isLoading = true;
    this.errorMessage = '';

    const { username, password } = this.form.value;

    this.authService.login(username, password).subscribe({
      next: () => {
        this.isLoading = false;
        this.componentService.updateResult(true);
        this.router.navigate(['/home'], { replaceUrl: true });
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Invalid username or password.';
      }
    });
  }
}
