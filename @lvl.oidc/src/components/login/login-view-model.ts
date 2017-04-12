import { Required } from '@lvl/core';

export class LoginViewModel {
    @Required()
    username: string;

    @Required()
    password: string;

    rememberLogin: boolean;

    returnUrl: string;

    constructor(options: {
        username: string,
        password: string,
        rememberLogin: boolean,
        returnUrl: string
    }) {
        this.username = options.username;
        this.password = options.password;
        this.rememberLogin = options.rememberLogin;
        this.returnUrl = options.returnUrl;
    }
}
