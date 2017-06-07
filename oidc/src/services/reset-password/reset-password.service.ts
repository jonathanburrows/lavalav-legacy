import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { ResetPasswordViewModel } from './reset-password-view-model';
import { HeadersService } from '@lvl/front-end';
import { OidcOptions } from '../oidc-options';

/**
 *  Contacts the server's ResetPassword service.
 */
@Injectable()
export class ResetPasswordService {
    constructor(private oidcOptions: OidcOptions, private http: Http, private headersService: HeadersService) { }

    /**
     *  Sends an email to the user with a link to reset the password.
     *  @param username the name of the user having the email sent.
     *  @throws {Error} username is null.
     */
    requestReset(username: string): Observable<Response> {
        if (!username) {
            throw new Error('username is null.');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/reset-password/request/${username}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers });
    }

    /**
     *  Requests that a user's password be updated.
     *  @param model contains the updated and old password.
     *  @returns Ok if the information is correct, validation errors otherwise.
     */
    reset(model: ResetPasswordViewModel): Observable<Response> {
        if (!model) {
            throw new Error('model is null');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/reset-password`;
        const headers = this.headersService.getHeaders();

        return this.http.put(url, model, { headers: headers });
    }
}
