import { EventEmitter, Injectable } from '@angular/core';
import { Headers } from '@angular/http';

import { StorageService } from '@lvl/front-end';

import { BearerToken } from './bearer-token';
import { IdToken } from './id-token';

/**
 *  Fetches and stores tokens in storage, and notifies of expiry.
 */
@Injectable()
export class TokenService {
    /**
     *  Key to get the bearer token from local storage.
     *  @remarks the key is public because implicit flow needs to access local storage without this service.
     */
    public static bearerTokenKey = 'oidc:bearer-token';

    /** Cached copy of the bearer token. */
    private _bearerToken: BearerToken;

    /** Fetches the cached bearer token, or fetches it from local storage. */
    public get bearerToken(): BearerToken {
        if (!this._bearerToken) {
            const serialized = this.storageService.getItem(TokenService.bearerTokenKey);
            this._bearerToken = serialized ? JSON.parse(serialized) : null;

            this.resetCountdowns();
        }
        return this._bearerToken;
    }

    /** Stores a given token in local storage and cache. */
    public set bearerToken(value: BearerToken) {
        const serialized = value ? JSON.stringify(value) : '';
        this.storageService.setItem(TokenService.bearerTokenKey, serialized);
        this._bearerToken = value;

        this.resetCountdowns();
    }

    /** Deserializes the jwt to an access token. */
    private get accessToken() {
        if (this.bearerToken && this.bearerToken.access_token) {
            return this.getSubjectOfJwt(this.bearerToken.access_token);
        } else {
            return null;
        }
    }

    /** Event which signifies the bearer token cannot be used without refresh. */
    public tokenExpired = new EventEmitter<BearerToken>();
    private tokenExpiryTimer: any;

    /** Event which signifies the bearer token should be refreshed, so there is no disruption to user experience. */
    public tokenHalflife = new EventEmitter<BearerToken>();
    private tokenHalflifeTimer: any;

    constructor(private storageService: StorageService) {
        this.resetCountdowns();
    }

    /**
     *  Splits a jwt into its peices, then decodes the subject.
     *  @param jwt The jwt which will be analysed.
     *  @remarks if jwt is null, then null is returned.
     */
    public getSubjectOfJwt(jwt: string): IdToken {
        if (!jwt) {
            return null;
        }

        const subjectIndex = 1;
        const encoded = jwt.split('.')[subjectIndex];
        const decoded = this.urlBase64Decode(encoded);
        return JSON.parse(decoded);
    }

    /**
     *  Sets the timers for expiry to when the access token expires (or half of that for halflife).
     */
    private resetCountdowns() {
        if (this.tokenExpiryTimer) {
            clearTimeout(this.tokenExpiryTimer);
            delete this.tokenExpiryTimer;
        }
        if (this.tokenHalflifeTimer) {
            clearTimeout(this.tokenHalflifeTimer);
            delete this.tokenHalflifeTimer;
        }

        if (this.bearerToken) {
            const currentTime = Math.floor(new Date().getTime() / 1000);
            const claims = this.getSubjectOfJwt(this.bearerToken.access_token);

            const secondsToTimeout = claims.exp - currentTime;
            this.tokenExpiryTimer = setTimeout(() => this.tokenExpired.emit(this.bearerToken), secondsToTimeout * 1000);

            this.tokenHalflifeTimer = setTimeout(() => this.tokenHalflife.emit(this.bearerToken), secondsToTimeout / 2 * 1000);
        }
    }

    private urlBase64Decode(decoding: string) {
        let output = decoding.replace('_', '/').replace('_', '+');

        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += '==';
                break;
            case 3:
                output += '=';
                break;
            default:
                throw new Error('Illegal base64url string.');
        }

        return window.atob(output);
    }
}
