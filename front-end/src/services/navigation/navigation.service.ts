import { Injectable, Type } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    CanActivate,
    Route,
    Router,
    RouterStateSnapshot
} from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { Navigatable, NavigatableMetadata } from '../../decorators';
import 'reflect-metadata';

/**
 *  Provides route guards, and generates menus.
 */
@Injectable()
export class Navigation implements CanActivate {
    constructor(protected router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        return this.hasPermissions(route.routeConfig);
    }

    protected hasPermissions(route: Route): boolean {
        if (!route) {
            throw new Error('route is null.');
        }

        return true;
    }

    /**
     *  Returns all menu items that arent flagged as hidden, and the user has permissions to.
     */
    getMenu(): Menu {
        const navigatableRoutes = this.router.config.filter(route => {
            if (!this.hasPermissions(route)) {
                return false;
            }

            if (this.componentAnnotation(route.component).hideInMenu) {
                return false;
            }

            // if the route requires parameters, it should not show on the menu.
            if (route.path && route.path.indexOf(':') > -1) {
                return false;
            }

            return true;
        });

        const navigationItems = navigatableRoutes.map(route => {
            const info = this.componentAnnotation(route.component);
            return <NavigationItem>{ route: route, info: info };
        });

        const uniqueGroupNames = navigationItems.map(nav => nav.info.group).filter((value, i, self) => self.indexOf(value) === i);

        return uniqueGroupNames.reduce((menu, groupName) => {
            menu[groupName] = navigationItems.filter(nav => nav.info.group === groupName);
            return menu;
        }, {});
    }

    getActiveNavigationItem(): NavigationItem {
        const routes = this.router.config;
        const activeRoute = routes.find(r => this.router.isActive(r.path, true)) || routes.find(r => this.router.isActive(r.path, false));

        if (!activeRoute) {
            return null;
        }

        const annotation = this.componentAnnotation(activeRoute.component);
        return { route: activeRoute, info: annotation };
    }

    private componentAnnotation(component: Type<any>): NavigatableMetadata {
        if (!component) {
            return { hideInMenu: true };
        }

        return Reflect.getOwnMetadata(Navigatable.name, component) || { hideInMenu: true };
    }
}

export interface Menu {
    [group: string]: NavigationItem[];
}

export interface NavigationItem {
    route: Route;
    info: NavigatableMetadata;
}
