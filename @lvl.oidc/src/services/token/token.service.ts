import { EventEmitter, Injectable } from '@angular/core';
import { Headers } from '@angular/http';

import { StorageService } from '@lvl/core';

import { BearerToken } from './bearer-token';
import { IdToken } from './id-token';

@Injectable()
export class TokenService {
    public bearerTokenKey = 'oidc:bearer-token';
    private _bearerToken: BearerToken;
    public get bearerToken() {
        if (!this._bearerToken) {
            const serialized = this.storageService.getItem(this.bearerTokenKey);
            this._bearerToken = serialized ? JSON.parse(serialized) : null;

            this.resetCountdowns();
        }
        return this._bearerToken;
    }
    public set bearerToken(value: BearerToken) {
        const serialized = value ? JSON.stringify(value) : '';
        this.storageService.setItem(this.bearerTokenKey, serialized);
        this._bearerToken = value;

        this.resetCountdowns();
    }

    public get accessToken() {
        if (this.bearerToken && this.bearerToken.access_token) {
            return this.convertJwtToIdToken(this.bearerToken.access_token);
        } else {
            return null;
        }
    }

    public tokenExpired = new EventEmitter<BearerToken>();
    private tokenExpiryTimer: any;

    public tokenHalflife = new EventEmitter<BearerToken>();
    private tokenHalflifeTimer: any;

    constructor(private storageService: StorageService) { }

    public isTokenExpired() {
        if (!this.bearerToken) {
            return true;
        }

        if (!this.accessToken) {
            throw new Error('No claims defined.');
        }
        return new Date().getTime() > this.accessToken.exp;
    }

    public convertJwtToIdToken(jwt: string): IdToken {
        if (!jwt) {
            return null;
        }

        const subjectIndex = 1;
        const encoded = jwt.split('.')[subjectIndex];
        const decoded = this.urlBase64Decode(encoded);
        return JSON.parse(decoded);
    }

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
            const claims = this.convertJwtToIdToken(this.bearerToken.access_token);

            const secondsToTimeout = claims.exp - currentTime;
            this.tokenExpiryTimer = setTimeout(() => this.tokenExpired.emit(this.bearerToken), secondsToTimeout);

            const halflifeDate = claims.iat + (claims.exp - claims.iat) / 2;
            const secondsToHalflife = halflifeDate - currentTime;
            this.tokenHalflifeTimer = setTimeout(() => this.tokenHalflife.emit(this.bearerToken), secondsToHalflife);
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
