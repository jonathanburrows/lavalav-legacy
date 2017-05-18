import { Component, Input, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';

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
    routeGroups: { group: string, routes: Route[] }[];

    /* Used to control the highlighting and title. */
    selectedRoute: Route;

    constructor(private router: Router) {}

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

    /**
     *  Will show the search input in the application bar.
     */
    openSearch() {
        this.showSearch = true;
    }

    /**
     *  Will hide the search input in the application bar.
     */
    closeSearch() {
        this.showSearch = false;
    }

    private updateSelectedRoute(route: any) {
        const fullRoute: string = route.urlAfterRedirect || route.url;
        const cleanRoute = fullRoute.startsWith('/') ? fullRoute.substr(1) : fullRoute;
        this.selectedRoute = this.router.config.filter(r => r.path === cleanRoute)[0];
        this.closeSideNav();
    }

    ngOnInit() {
        this.router.events.subscribe(this.updateSelectedRoute.bind(this));
        this.updateRouteGroups();
    }

    /**
     *  This was placed into a seperate method because having it called directly from a getter was causing an infinite loop.
     *  @remarks - please fix the infinite loop issue, then remove this method.
     */
    private updateRouteGroups() {
        const navigatableRoutes = this.router.config.filter(r => r.data && r.data['showInNavigation']);

        const uniqueGroupNames = navigatableRoutes.map(r => <string>r.data['group']).filter((value, i, self) => self.indexOf(value) === i);

        this.routeGroups = uniqueGroupNames.map(groupName => {
            const routes = navigatableRoutes.filter(r => r.data['group'] === groupName);

            if (!groupName) {
                const routeDescriptions = routes.map(r => r.path).join(', ');
                throw new Error(`The following urls were marked as navigatable, but have no group: ${routeDescriptions}`);
            }

            return {
                group: groupName,
                routes: routes
            };
        });
    }
}
