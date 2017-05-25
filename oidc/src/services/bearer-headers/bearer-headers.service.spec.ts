// Due to a breaking change with google chrome and jasmine, tests cant be run.
// Once a fix is published, these tests need to be verified.
// I just feel terrible adding code without tests, but the time spent trying to fix jasmine has already been considerable.
import { NgZone } from '@angular/core';

import { LocalStorageService } from '@lvl/front-end';
import { BearerHeadersService } from './bearer-headers.service';
import { TokenService } from '../token';

describe(BearerHeadersService.name, () => {
    it('will not attach a header if there is no bearer token', () => {
        const storageService = new LocalStorageService();
        storageService.clear();
        const tokenService = new TokenService(storageService, new NgZone(false));
        const headersService = new BearerHeadersService(tokenService);

        const headers = headersService.getHeaders();
        const authorizationHeader = headers.get('Authorization');

        expect(authorizationHeader).toBeNull();
    });

    it('will include the header if there is a bearer token', () => {
        const storageService = new LocalStorageService();

        const token = JSON.stringify({ access_token: 'hello :)' });
        storageService.setItem(TokenService.bearerTokenKey, token);

        const tokenService = new TokenService(storageService, new NgZone(false));
        const headersService = new BearerHeadersService(tokenService);

        const headers = headersService.getHeaders();
        const authorizationHeader = headers.get('Authorization');

        expect(authorizationHeader).toBe('hello :)');
    });
});
