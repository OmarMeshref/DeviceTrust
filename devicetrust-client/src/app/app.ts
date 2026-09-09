import { Component, signal } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { NavbarComponent } from './shared/navbar/navbar';
import { FooterComponent } from './shared/footer/footer';

@Component({
  imports: [RouterOutlet, NavbarComponent, FooterComponent],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('devicetrust-client');
  showChrome = signal(true);

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map(() => {
        // Walk to the deepest activated route to read its data, since data
        // lives on the leaf route, not necessarily the top-level one.
        let r = this.route;
        while (r.firstChild) r = r.firstChild;
        return r;
      }),
      mergeMap((r) => r.data)
    ).subscribe((data) => {
      this.showChrome.set(!data['hideChrome']);
    });
  }
}