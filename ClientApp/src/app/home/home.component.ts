import { Component } from '@angular/core';
import { ComponentService } from '../services/component.service';

@Component({
  standalone: false,
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(
    private componentService: ComponentService,
  ) { 
    this.componentService.updateResult(true);
  }
  
}
