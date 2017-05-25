import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { OidcOptions } from '../oidc-options';
import { User } from '../../models';

/**
 *  Provides a way to communicate with the UserController.
 */
@Injectable()
export class UserService {
    constructor(private http: Http, private oidcOptions: OidcOptions) { }

    /**
     *  Creates a new user (if valid).
     *  @param username The identifier of the user.
     *  @param password The secret authenticator of the user.
     *  @returns If valid, the created user. Otherwise, model errors.
     */
    create(username: string, password: string): Observable<User> {
        if (!username) {
            throw new Error('username is null.');
        }
        if (!password) {
            throw new Error('password is null.');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/user`;

        const headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded'});

        const body = new FormData();
        body.append('username', username);
        body.append('password', password);

        return this.http.post(url, body, headers).map(response => response.json());
    }
}
