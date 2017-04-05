export interface ImplicitAuthorizationOptions {
    response_type: string;
    client_id: string;
    redirect_uri: string;
    scope: string;
    nonce: string;
    state: string;
    id_token_hint?: string;
    prompt?: string;
}
