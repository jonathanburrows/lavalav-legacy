import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

import { HeadersService } from '@lvl/core';
import { TokenService } from '../token';

@Injectable()
export class BearerHeadersService extends HeadersService {
    constructor(private tokenService: TokenService) {
        super();
    }

    public getHeaders() {
        const headers = super.getHeaders();

        if (this.tokenService.bearerToken) {
            headers.append('Authorization', `Bearer ${this.tokenService.bearerToken.access_token}`);
        }

        return headers;
    }
}
