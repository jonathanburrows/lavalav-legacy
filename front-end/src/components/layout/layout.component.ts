import {
    Component,
    Input,
    OnInit
} from '@angular/core';
import { Title } from '@angular/platform-browser';
import {
    ActivatedRoute,
    NavigationEnd,
    NavigationError,
    Route,
    Router
} from '@angular/router';

import { NavigatableMetadata } from '../../decorators';
import {
    NavigationItem,
    Navigation,
    Menu
} from '../../services';

/**
 *  Provides a common layout for navigation and the toolbar, so that the same structure can be re-used across projects.
 */
@Component({
    selector: 'lvl-layout',
    templateUrl: 'layout.component.html',
    styleUrls: ['layout.component.scss']
})
export class LayoutComponent implements OnInit {
    /* Will display on the side nav. */
    @Input() siteTitle: string;

    /* If on a smaller device, this will determine if navigation is show. */
    showSideNav = false;

    /* Will display the search on the app bar. */
    showSearch = false;

    /* Determines what will show up in the side navigation panel. */
    menu: Menu;

    constructor(
        private router: Router,
        public activatedRoute: ActivatedRoute,
        public navigationService: Navigation,
        private title: Title) { }

    /**
     *  Will show navigation on smaller devices.
     */
    openSideNav() {
        this.showSideNav = true;
    }

    /**
     *  Will hide navigation on smaller devices.
     */
    closeSideNav() {
        this.showSideNav = false;
    }

    ngOnInit() {
        this.menu = this.navigationService.getMenu();
        this.router.events.subscribe(args => {
            if (args instanceof NavigationError && args.url !== '/not-found') {
                this.router.navigate(['/not-found']);
            }
            if (args instanceof NavigationEnd) {
                const navItem = this.navigationService.getActiveNavigationItem();
                this.title.setTitle(`${navItem.info.title} - ${this.siteTitle}`);
                this.closeSideNav();

                // Sometimes changing the route will update the menu. Make sure it refreshes.
                this.menu = this.navigationService.getMenu();
            }
        });
    }
}
