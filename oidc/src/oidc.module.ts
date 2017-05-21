import { NgModule, ModuleWithProviders } from '@angular/core';

import { FrontEndModule } from '@lvl/front-end';
import { OidcOptions } from './services';

/**
 *  Provides services and components for security.
 */
@NgModule({
    declarations: [
    ],
    exports: [
    ],
    imports: [
        FrontEndModule
    ]
})
export class OidcModule {
    /**
     *  Provides services and components for security using the resource owner flow.
     *  @param oidcOptions Options for how to connect to the authorization server.
     *  @returns A module with services and components registered required for resource owner security.
     */
    static useResourceOwnerFlow(oidcOptions: OidcOptions): ModuleWithProviders {
        // null checks and logic were not put in this method as it causes a compiler error for angular.

        return {
            ngModule: FrontEndModule,
            providers: [
                { provide: OidcOptions, useValue: oidcOptions }
            ]
        };
    }
}
