// Due to a breaking change with google chrome and jasmine, tests cant be run.
// Once a fix is published, these tests need to be verified.
// I just feel terrible adding code without tests, but the time spent trying to fix jasmine has already been considerable.

import { BearerToken } from './bearer-token';
import { IdToken } from './id-token';
import { TokenService } from './token.service';
import { StorageService } from '@lvl/front-end';

describe(TokenService.name, () => {
    describe('bearerToken', () => {
        it('will load from local storage', () => {
            const storageService = new MockStorageService();
            storageService.setItem(TokenService.bearerTokenKey, 'my-bearer-token');
            const tokenService = new TokenService(storageService);

            const bearerToken: any = tokenService.bearerToken;

            expect(bearerToken).toBe('my-bearer-token');
        });

        it('will be blank if no entry in local storage', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            expect(tokenService.bearerToken).toBeUndefined();
        });
    });

    describe('tokenExpired', () => {
        it('will trigger if the token in local storage is expired', () => {
            const storageService = new MockStorageService();
            const expiredJwt = generateMockJwt({
                iss: 'localhost',
                sub: 'user',
                aud: 'localhost',
                exp: (new Date().getTime() / 1000) - 1,
                iat: (new Date().getTime() / 1000) - 1
            });
            const tokenService = new TokenService(storageService);

            expect(tokenService.tokenExpired.first).not.toBeNull();
        });

        it('will not be triggered if the token in local storage isnt expired', () => {
            const storageService = new MockStorageService();

            const nextYear = new Date();
            nextYear.setFullYear(nextYear.getFullYear() + 1);
            const secondsUntilNextYear = nextYear.getTime() / 1000;

            const expiredJwt = generateMockJwt({
                iss: 'localhost',
                sub: 'user',
                aud: 'localhost',
                exp: secondsUntilNextYear,
                iat: secondsUntilNextYear
            });
            const tokenService = new TokenService(storageService);

            expect(tokenService.tokenExpired.first).toBeNull();
        });

        it('will not be triggered if there is no token in local storage', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            expect(tokenService.tokenExpired.first).toBeNull();
        });

        it('will trigger if the token is set to an expired one', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);
            const expiredJwt = generateMockJwt({
                iss: 'localhost',
                sub: 'user',
                aud: 'localhost',
                exp: (new Date().getTime() / 1000) - 1,
                iat: (new Date().getTime() / 1000) - 1
            });

            let triggered = false;
            tokenService.tokenExpired.subscribe(_ => triggered = true);
            tokenService.bearerToken = {
                access_token: expiredJwt,
                token_type: 'Bearer',
                id_token: expiredJwt,
                state: '123'
            };

            expect(triggered).toBeTruthy();
        });

        it('will not be triggered if the token is set to one which expires on the future', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            const nextYear = new Date();
            nextYear.setFullYear(nextYear.getFullYear() + 1);
            const secondsUntilNextYear = nextYear.getTime() / 1000;
            const notExpiredJwt = generateMockJwt({
                iss: 'localhost',
                sub: 'user',
                aud: 'localhost',
                exp: secondsUntilNextYear,
                iat: secondsUntilNextYear
            });

            let triggered = false;
            tokenService.tokenExpired.subscribe(_ => triggered = true);
            tokenService.bearerToken = {
                access_token: notExpiredJwt,
                token_type: 'Bearer',
                id_token: notExpiredJwt,
                state: '123'
            };

            expect(triggered).toBeFalsy();
        });

        it('will not be triggered if not set to a bearer token', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            let triggered = false;
            tokenService.tokenExpired.subscribe(_ => triggered = true);
            tokenService.bearerToken = null;

            expect(triggered).toBeFalsy();
        });
    });

    describe('getSubjectOfJwt', () => {
        it('will return null if no subject is given', () => {
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            const subject = tokenService.getSubjectOfJwt(null);

            expect(subject).toBeNull();
        });

        it('will decode the subject of a jwt', () => {
            const jwt = generateMockJwt({
                iss: 'my-little-secret',
                sub: '.',
                aud: '.',
                exp: 1,
                iat: 1
            });
            const storageService = new MockStorageService();
            const tokenService = new TokenService(storageService);

            const decoded = tokenService.getSubjectOfJwt(jwt);

            expect(decoded.iss).toBe('my-little-secret');
        });
    });

    function generateMockJwt(idToken: IdToken) {
        // this will have a garbage header and signature, its only to be used for the id token.
        const encodedSubject = btoa(JSON.stringify(idToken));
        return `jiberish.${encodedSubject}.more-jiberish`;
    }
});

class MockStorageService extends StorageService {
    private items: { [key: string]: any } = {};

    get length() {
        return Object.keys(this.items).length;
    }

    clear() {
        this.items = {};
    }

    getItem(key: string) {
        const item = this.items[key];
        return item ? JSON.stringify(this.items[key]) : null;
    }

    key(index: number) {
        const j = Object.keys(this.items)[index];
        const item = this.items[j];
        return item ? JSON.stringify(item) : null;
    }

    removeItem(key: string) {
        this.items[key] = undefined;
    }

    setItem(key: string, data: string) {
        this.items[key] = data;
    }
}
