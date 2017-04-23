import { Injectable } from '@angular/core';

@Injectable()
export class OidcOptions {
    public authorizationServerUrl: string;
    public clientUrl: string;
    public clientId: string;
    public clientSecret: string;
    public scopes: string[];
}
