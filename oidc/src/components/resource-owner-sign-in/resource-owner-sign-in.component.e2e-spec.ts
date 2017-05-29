import { browser, by, element, protractor } from 'protractor';

describe('e2e ResourceOwnerSignIn', () => {
    let signinPage: CredentialsSigninPage;

    const portaitSize = { width: 320, height: 568 };
    const landscapeSize = { width: 568, height: 320 };
    const tabletSize = { width: 768, height: 1024 };

    describe('on portrait devices', () => {
        beforeEach(() => {
            signinPage = new CredentialsSigninPage();
            signinPage.setSize(portaitSize);
            signinPage.navigateTo();
            signinPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on landscape devices', () => {
        beforeEach(() => {
            signinPage = new CredentialsSigninPage();
            signinPage.setSize(landscapeSize);
            signinPage.navigateTo();
            signinPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on tablet+ devices', () => {
        beforeEach(() => {
            signinPage = new CredentialsSigninPage();
            signinPage.setSize(tabletSize);
            signinPage.navigateTo();
            signinPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    function multiDeviceBehaviour() {
        it('will have a username field', () => {
            const username = signinPage.getInput('username');

            expect(username.isDisplayed).toBeTruthy();
        });

        it('will have a password field', () => {
            const password = signinPage.getInput('password');

            expect(password.isDisplayed).toBeTruthy();
        });

        it('will have a signin button', () => {
            const signIn = signinPage.getElement('button.card__actions--sign-in');

            expect(signIn.isDisplayed).toBeTruthy();
        });

        it('will show a validation message if the username is unfocused without a value', () => {
            const username = signinPage.getInput('username');
            const card = signinPage.getElement('.card');

            username.click();
            card.click();

            const errorMessage = signinPage.getErrorMessagesForInput('username');
            expect(errorMessage.getText()).toBe('Required');
        });

        it('will show a validation message if the password is unfocused without a value', () => {
            const username = signinPage.getInput('password');
            const card = signinPage.getElement('.card');

            username.click();
            card.click();

            const errorMessage = signinPage.getErrorMessagesForInput('password');
            expect(errorMessage.getText()).toBe('Required');
        });

        it('will show a validation message if submitted without a username', () => {
            const signIn = signinPage.getElement('button.card__actions--sign-in');
            const password = signinPage.getInput('password');
            password.sendKeys('my password');

            signIn.click();

            const errorMessage = signinPage.getErrorMessagesForInput('username');
            expect(errorMessage.getText()).toBe('Required');
        });

        it('will show a validation message if submitted without a password', () => {
            const signIn = signinPage.getElement('button.card__actions--sign-in');
            const username = signinPage.getInput('username');
            username.sendKeys('my username');

            signIn.click();

            const errorMessage = signinPage.getErrorMessagesForInput('password');
            expect(errorMessage.getText()).toBe('Required');
        });

        it('will show a validation message when submitted if the user does not exist', () => {
            const username = signinPage.getInput('username');
            username.sendKeys('non-existant');
            const password = signinPage.getInput('password');
            password.sendKeys('password');
            const signIn = signinPage.getElement('button.card__actions--sign-in');

            signIn.click();

            const errorMessage = signinPage.getErrorMessagesForInput('username');
            expect(errorMessage.getText()).toBe(`Can't find user`);
        });

        it('will show a validation message when submitted if the password is incorrect', () => {
            const username = signinPage.getInput('username');
            username.sendKeys('testuser');
            const password = signinPage.getInput('password');
            password.sendKeys('fake password');
            const signIn = signinPage.getElement('button.card__actions--sign-in');

            signIn.click();

            const errorMessage = signinPage.getErrorMessagesForInput('password');
            expect(errorMessage.getText()).toBe('Incorrect password, try again');
        });

        it('will redirect the user after a successful login', () => {
            const username = signinPage.getInput('username');
            username.sendKeys('testuser');
            const password = signinPage.getInput('password');
            password.sendKeys('password');
            const signIn = signinPage.getElement('button.card__actions--sign-in');

            signIn.click();

            expect(browser.getCurrentUrl()).not.toContain('oidc/sign-in');
        });

        it('will redirect the user to the recover username page if they hit the forgot username link', () => {
            const recoverUsername = signinPage.getElement('.card__content__forgot-link--recover-username');

            recoverUsername.click();

            expect(browser.getCurrentUrl()).toContain('oidc/recover-username');
        });

        it('will redirect the user to the reset password page if they hit the forgot password link', () => {
            const recoverUsername = signinPage.getElement('.card__content__forgot-link--reset-password');

            recoverUsername.click();

            expect(browser.getCurrentUrl()).toContain('oidc/request-reset-password');
        });

        it('will redirect the user to the register account page if they hit the register button', () => {
            const recoverUsername = signinPage.getElement('.card__actions__button--register');

            recoverUsername.click();

            expect(browser.getCurrentUrl()).toContain('oidc/register-account');
        });
    }
});

class CredentialsSigninPage {
    navigateTo() {
        return browser.get('/oidc/sign-in');
    }

    setSize(size: { width: number, height: number }) {
        return browser.driver.manage().window().setSize(size.width, size.height);
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
}
