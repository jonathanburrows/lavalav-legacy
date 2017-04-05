import { Injectable } from '@angular/core';

@Injectable()
export class OidcConfiguration {
    public authorizationServerUrl: string;
    public resourceServerUrl: string;
    public clientUrl: string;
    public clientName: string;
    public clientSecret: string;
    public scopes: string[];
}
