import { browser, by, element, protractor } from 'protractor';

describe('e2e RegisterAccountComponent', () => {
    let registerAccountPage: RegisterAccountPage;

    beforeEach(() => {
        registerAccountPage = new RegisterAccountPage();
        registerAccountPage.navigateTo();
        registerAccountPage.disableAnimations();
    });

    it('will have a username field', () => {
        const username = registerAccountPage.getInput('username');

        expect(username.isDisplayed).toBeTruthy();
    });

    it('will have a password field', () => {
        const password = registerAccountPage.getInput('password');

        expect(password.isDisplayed).toBeTruthy();
    });

    it('will have a register button', () => {
        const register = registerAccountPage.getElement('.card__actions--register');

        expect(register.isDisplayed).toBeTruthy();
    });

    it('will show a validation message if the username is unfocused without a value', () => {
        const username = registerAccountPage.getInput('username');
        const card = registerAccountPage.getElement('.card');

        username.click();
        card.click();

        const errorMessage = registerAccountPage.getErrorMessagesForInput('username');
        expect(errorMessage.getText()).toBe('Required');
    });

    it('will show a validation message if the password is unfocused without a value', () => {
        const username = registerAccountPage.getInput('password');
        const card = registerAccountPage.getElement('.card');

        username.click();
        card.click();

        const errorMessage = registerAccountPage.getErrorMessagesForInput('password');
        expect(errorMessage.getText()).toBe('Required');
    });

    it('will show a validation message if submitted without a username', () => {
        const register = registerAccountPage.getElement('.card__actions--register');
        const password = registerAccountPage.getInput('password');
        password.sendKeys('my password');

        register.click();

        const errorMessage = registerAccountPage.getErrorMessagesForInput('username');
        expect(errorMessage.getText()).toBe('Required');
    });

    it('will show a validation message if submitted without a password', () => {
        const register = registerAccountPage.getElement('.card__actions--register');
        const username = registerAccountPage.getInput('username');
        username.sendKeys('my username');

        register.click();

        const errorMessage = registerAccountPage.getErrorMessagesForInput('password');
        expect(errorMessage.getText()).toBe('Required');
    });

    it('will show a validation message if the user already exists', () => {
        const register = registerAccountPage.getElement('.card__actions--register');
        const username = registerAccountPage.getInput('username');
        username.sendKeys('testuser');
        const password = registerAccountPage.getInput('password');
        password.sendKeys('password');

        register.click();

        const errorMessage = registerAccountPage.getErrorMessagesForInput('username');
        expect(errorMessage.getText()).toBe('Already taken, try another');
    });

    it('will redirect the user to the home page after successfully registering', () => {
        const username = registerAccountPage.getInput('username');
        const uniqueName = `redirect-from-home-user${Math.random()}`;
        username.sendKeys(uniqueName);
        const password = registerAccountPage.getInput('password');
        password.sendKeys('password');
        const register = registerAccountPage.getElement('.card__actions--register');

        register.click();

        expect(browser.getCurrentUrl()).not.toContain('oidc/register-account');
    });
});

class RegisterAccountPage {
    navigateTo() {
        return browser.get('/oidc/register-account');
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
