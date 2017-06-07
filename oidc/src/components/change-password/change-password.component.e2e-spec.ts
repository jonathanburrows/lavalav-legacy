import { browser, by, element, protractor } from 'protractor';

describe('e2e ChangePasswordComponent', () => {
    let changePasswordPage: ChangePasswordPage;
    let username: string;

    beforeEach(() => {
        changePasswordPage = new ChangePasswordPage();
        username = changePasswordPage.createUserAndLogin();
        changePasswordPage.disableAnimations();
        changePasswordPage.navigateTo();
    });

    describe('old password input', () => {
        it('will exist', () => {
            const oldPassword = changePasswordPage.getInput('oldPassword');

            expect(oldPassword.isPresent()).toBeTruthy();
        });

        it('will show required message if submitted without a value', () => {
            const newInput = changePasswordPage.getInput('newPassword');
            newInput.sendKeys('required');

            const saveButton = changePasswordPage.getElement('.save-button');
            saveButton.click();

            const oldPasswordValidation = changePasswordPage.getErrorMessagesForInput('oldPassword');

            expect(oldPasswordValidation.getText()).toBe('Required');
        });

        it('will show required message if submitted without mismatched value', () => {
            const oldPassword = changePasswordPage.getInput('oldPassword');
            oldPassword.sendKeys('mismatched');
            const newInput = changePasswordPage.getInput('newPassword');
            newInput.sendKeys('required');

            const saveButton = changePasswordPage.getElement('.save-button');
            saveButton.click();

            const oldPasswordValidation = changePasswordPage.getErrorMessagesForInput('oldPassword');

            expect(oldPasswordValidation.getText()).toBe('Incorrect password, try again');
        });
    });

    describe('new password input', () => {
        it('will exist', () => {
            const newPassword = changePasswordPage.getInput('newPassword');

            expect(newPassword.isPresent()).toBeTruthy();
        });

        it('will show required message if submitted without a value', () => {
            const oldPasswordInput = changePasswordPage.getInput('oldPassword');
            oldPasswordInput.sendKeys('required');

            const saveButton = changePasswordPage.getElement('.save-button');
            saveButton.click();

            const newPasswordValidation = changePasswordPage.getErrorMessagesForInput('newPassword');

            expect(newPasswordValidation.getText()).toBe('Required');
        });
    });
});

class ChangePasswordPage {
    private tokenKey = 'oidc:bearer-token';

    /**
     *  Generates a new user, with the username being a guid, and the password being password.
     *  This was done so the tests dont collide with one another.
     *  @returns the generated username.
     */
    createUserAndLogin(): string {
        // generating guid (copy pasted from https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript)
        const username = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
            // tslint:disable:no-bitwise
            const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });

        browser.get('/oidc/register-account');
        const usernameInput = this.getInput('username');
        usernameInput.sendKeys(username);

        const passwordInput = this.getInput('password');
        passwordInput.sendKeys('password');

        const register = this.getElement('.card__actions--register');
        register.click();

        return username;
    }

    navigateTo() {
        return browser.get('/oidc/change-password');
    }

    getElement(query: string) {
        return element(by.css(query));
    }

    getInput(inputName: string) {
        return element(by.css(`[formcontrolname="${inputName}"]`));
    }

    getErrorMessagesForInput(inputName: string) {
        const input = this.getInput(inputName);
        const container = input.element(by.xpath('ancestor::md-input-container'));
        return container.element(by.css('.mat-input-error'));
    }

    disableAnimations() {
        // Sorry for the hack, the protractor DisableAnimations for angular 2 isnt supported yet.
        // There is a class in index.html which prevents the animations.
        const scriptHack = `document.body.className = document.body.className + ' no-animations';`;
        browser.executeScript(scriptHack);
    }

    signIn() {
        browser.get('/oidc/sign-in');

        const username = this.getInput('username');
        username.sendKeys('personal-details-testuser');

        const password = this.getInput('password');
        password.sendKeys('password');

        const signIn = this.getElement('button.card__actions--sign-in');
        signIn.click();
    }
}
