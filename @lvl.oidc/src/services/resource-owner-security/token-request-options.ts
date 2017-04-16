export interface TokenRequestOptions {
    grant_type: string;
    client_id?: string;
    client_secret?: string;
    scope?: string;
    username?: string;
    password?: string;
    refresh_token?: string;
}
