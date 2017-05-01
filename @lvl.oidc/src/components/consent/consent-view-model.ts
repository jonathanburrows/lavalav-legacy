import { ScopeViewModel } from './scope-view-model';

export class ConsentViewModel {
    public clientName: string;
    public clientUrl: string;
    public clientLogoUrl: string;
    public returnUrl: string;
    public rememberConsent: boolean;
    public confirmed: boolean;

    public scopesConsented: string[];
    public identityScopes: ScopeViewModel[];
    public resourceScopes: ScopeViewModel[];
}
