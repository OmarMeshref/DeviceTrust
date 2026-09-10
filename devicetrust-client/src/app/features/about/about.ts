import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TrustIllustrationComponent } from '../../shared/trust-illustration/trust-illustration';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [RouterLink, TrustIllustrationComponent],
  styleUrl: './about.scss',
  templateUrl: './about.html',
})
export class About {}