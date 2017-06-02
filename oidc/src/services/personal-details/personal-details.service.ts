import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { HeadersService } from '@lvl/front-end';
import { PersonalDetailsViewModel } from './personal-details-view-model';
import { OidcOptions } from '../oidc-options';

/**
 *  Contacts the server's Personal Details service.
 */
@Injectable()
export class PersonalDetailsService {
    constructor(private oidcOptions: OidcOptions, private http: Http, private headersService: HeadersService) { }

    /**
     *  Fetches a model containing claims which describe a person.
     */
    get(): Observable<PersonalDetailsViewModel> {
        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/personal-details`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(response => response.json());
    }

    /**
     *  Updates a user's claims based on the contents of the model.
     *  @param model Flat model mapping to a user's claims.
     */
    update(model: PersonalDetailsViewModel): Observable<PersonalDetailsViewModel> {
        const url = `${this.oidcOptions.authorizationServerUrl}/oidc/personal-details`;
        const headers = this.headersService.getHeaders();
        const payload = JSON.stringify(model);

        return <any>this.http.put(url, model, { headers: headers });
    }
}
