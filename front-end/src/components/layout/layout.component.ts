import { Component, Input } from '@angular/core';
import { Route, Router } from '@angular/router';

/**
 *  Provides a common layout for navigation and the toolbar, so that the same structure can be re-used across projects.
 */
@Component({
    selector: 'lvl-layout',
    templateUrl: 'layout.component.html',
    styleUrls: ['layout.component.scss']
})
export class LayoutComponent {
    /* Will display on the side nav. */
    @Input() siteTitle: string;

    /* If on a smaller device, this will determine if navigation is show. */
    showSideNav = false;

    /* Will display the search on the app bar. */
    showSearch = false;

    /**
     *  This will determine if a <router-outlet> or a <ng-content> is rendered in the page.
     *  By default, it renders a router.
     *  @remarks - a getter/setter was used for throwing an error if an invalid value is provided.
     */
    @Input()
    get contentType(): 'router' | 'content' {
        return this._contentType;
    }
    set contentType(value: 'router' | 'content') {
        if (value !== 'router' && value !== 'content') {
            throw new Error(`[content] attribute of lvl-layout set to ${value}, but the only allowed values are 'router' and 'content'`);
        }
        this._contentType = value;
    }
    private _contentType: 'router' | 'content' = 'router';

    /* Determines what will show up in the side navigation panel. */
    get routeGroups(): { group: string, routes: Route[] }[] {
        const groups: { group: string, routes: Route[] }[] = [];

        const navigatableRoutes = this.router.config.filter(r => r.data && r.data['showInNavigation']);

        // get the unique group names.
        const groupNames = navigatableRoutes.map(r => <string>r.data['group']).filter((value, i, self) => self.indexOf(value) === i);

        return groupNames.map(groupName => {
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

    /* Used to control the highlighting and title. */
    selectedRoute: Route;

    constructor(private router: Router) {
        this.router.events.subscribe(this.updateSelectedRoute.bind(this));
    }

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
    }
}
