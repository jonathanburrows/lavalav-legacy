import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { HeadersService } from '@lvl/front-end';
import { OidcOptions } from '../oidc-options';

@Injectable()
export class RecoverUsernameService {
    constructor(private oidcOptions: OidcOptions, private http: Http, private headersService: HeadersService) { }

    recoverUsername(email: string): Observable<Response> {
        if (!email) {
            throw new Error('email is null');
        }

        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/recover-username/${email}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers });
    }
}
