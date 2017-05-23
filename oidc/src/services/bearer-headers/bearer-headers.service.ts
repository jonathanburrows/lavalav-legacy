import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

import { HeadersService } from '@lvl/front-end';
import { TokenService } from '../token';

/**
 *  Generates headers with the bearer token attached.
 */
@Injectable()
export class BearerHeadersService extends HeadersService {
    constructor(private tokenService: TokenService) {
        super();
    }

    /**
     *  Generate a new set of headers with the bearer token attached.
     */
    public getHeaders() {
        const headers = super.getHeaders();

        if (this.tokenService.bearerToken) {
            headers.append('Authorization', `Bearer ${this.tokenService.bearerToken.access_token}`);
        }

        return headers;
    }
}
