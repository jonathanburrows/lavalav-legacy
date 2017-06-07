import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { HeadersService } from '@lvl/front-end';
import { OidcOptions } from '../oidc-options';
import { User } from '../../models';

/**
 *  Used to access some api like functionality for users.
 */
@Injectable()
export class UserAdministrationService {
    constructor(private oidcOptions: OidcOptions, private http: Http, private headersService: HeadersService) { }

    /**
     *  Will contact the server with a odata query for users.
     *  @param query An odata query which will be sent to the server.
     *  @throws {Error} query is null.
     */
    query(query: any) {
        if (!query) {
            throw new Error('query is null.');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/user-administration/odata`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers, params: query }).map(response => response.json());
    }

    /**
     *  Will fetch a single user with a matching id.
     *  @param id The identifier of the desired user.
     *  @throws {Error} id is null.
     */
    get(id: number): Observable<User> {
        if (!id) {
            throw new Error('id is null.');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/user-administration/api/${id}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(response => new User(response.json()));
    }

    /**
     *  Will update a user on the api.
     *  @param updating the user to be updated.
     *  @throws {Error} updating is null.
     */
    update(updating: User): Observable<User> {
        if (!updating) {
            throw new Error('updating is null.');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/user-administration/api`;
        const headers = this.headersService.getHeaders();

        return this.http.put(url, updating, { headers: headers }).map(response => new User(response.json));
    }
}
