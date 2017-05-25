import { Injectable } from '@angular/core';

/**
 *  Options used by the common front end services.
 */
@Injectable()
export class FrontEndOptions {
    // Url to the api.
    public resourceServerUrl: string;

    // required by angular cli
    public production: boolean;
}
