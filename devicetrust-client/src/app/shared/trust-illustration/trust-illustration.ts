import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-trust-illustration',
  standalone: true,
  imports: [],
  templateUrl: './trust-illustration.html'
})
export class TrustIllustrationComponent {
  @Input() caption = 'Every repair, permanently on the record.';
}