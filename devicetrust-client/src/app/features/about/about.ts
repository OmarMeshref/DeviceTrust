import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [RouterLink],
  styleUrl: './about.scss',
  templateUrl: './about.html',
})
export class About {}