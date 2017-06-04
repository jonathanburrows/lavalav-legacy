import { Injectable } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    Route,
    Router,
    RouterStateSnapshot
} from '@angular/router';
import { Observable } from 'rxjs/Observable';

import {
    Navigatable,
    NavigatableMetadata,
    Navigation
} from '@lvl/front-end';
import { SecurityService } from '../security';

/**
 *  Provide a way to get routing information about a component, and prevent unauthorized users from viewing them.
 */
@Injectable()
export class OidcNavigationService extends Navigation {
    constructor(router: Router, private securityService: SecurityService) {
        super(router);
    }

    /**
     *  If they cant activate because of the user, they are redirected.
     */
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        if (this.hasPermissions(route.routeConfig)) {
            // The user has permissions, carry on as usual.
            return true;
        } else if (!this.securityService.isAuthorized) {
            // The user isnt signed in, let them try before accessing.
            this.securityService.redirectToLogin(route.routeConfig.path);
            return false;
        } else {
            // The user explicitly does not have permissions, let them know.
            this.router.navigate(['/forbidden']);
            return false;
        }
    }

    /**
     *  Will determine if a user can visit a route based on their permissions.
     *  @param route The route being checked if the user has access to.
     *  @throws {Error} the route is null.
     *  @returns
     *      If the route has no component, then true.
     *
     *      If the route has no Navigatable decorator, then true.
     *
     *      If the route has no roles, then true.
     *
     *      If the user is not logged in, then false.
     *
     *      If the user is an administrator, then true.
     *
     *      If the user has at least one of the required roles, then true.
     *
     *      Otherwise false.
     */
    protected hasPermissions(route: Route): boolean {
        if (!route) {
            throw new Error('route is null.');
        }
        // When theres no component or redirect, then theres not enough information to determine if the user has rights, so allow it.
        if (!route.component) {
            return true;
        }

        // Navigatable decorator contains all the authorization information, if there's none, allow navigation.
        const navigationMetadata: NavigatableMetadata = Reflect.getOwnMetadata(Navigatable.name, route.component);
        if (!navigationMetadata) {
            return true;
        }

        // If theres no defined roles on who can access the page, allow everyone by default.
        if (!navigationMetadata.roles || !navigationMetadata.roles.length) {
            return true;
        }

        // Dont allow people not logged in or without claims to view a component which requires roles.
        if (!this.securityService.isAuthorized || !this.securityService.userInfo) {
            return false;
        }

        // If the user has a role of administrator, they can see anything.
        const userInfoRoles = this.securityService.userInfo['role'] || [];
        // If a user has 1 role, it is not in array, and needs to be converted.
        const usersRoles: string[] = Array.isArray(userInfoRoles) ? userInfoRoles : [userInfoRoles];
        if (usersRoles.indexOf('administrator') > -1) {
            return true;
        }

        // Check if the user has at least one of the roles required by the component.
        const requiredRoles = navigationMetadata.roles;
        return usersRoles.some(userRole => requiredRoles.indexOf(userRole) > -1);
    }
}
