import { browser, by, element, protractor } from 'protractor';

describe('e2e RecoverUsernameComponent', () => {
    let page: RequestPasswordResetPage;

    beforeEach(() => {
        page = new RequestPasswordResetPage();
        page.navigateTo();
        page.disableAnimations();
    });

    it('will have an username field', () => {
        const username = page.getInput('username');

        expect(username.isDisplayed).toBeTruthy();
    });

    it('will have a request reset button', () => {
        const resetButton = page.getElement('.card__actions--request-reset');

        expect(resetButton.isDisplayed).toBeTruthy();
    });

    it('will show a validation message if submitted without a username', () => {
        const resetButton = page.getElement('.card__actions--request-reset');

        resetButton.click();

        const errorMessages = page.getErrorMessagesForInput('username');
        expect(errorMessages.getText()).toBe('Required');
    });

    it('will show a validation message if submitted with a user that does not exist', () => {
        const resetButton = page.getElement('.card__actions--request-reset');
        const username = page.getInput('username');
        username.sendKeys('not-a-user');

        resetButton.click();

        const errorMessages = page.getErrorMessagesForInput('username');
        expect(errorMessages.getText()).toBe('User not found');
    });

    it('will show a validation message if user does not have an email', () => {
        const resetButton = page.getElement('.card__actions--request-reset');
        const username = page.getInput('username');
        username.sendKeys('emailless-user');

        resetButton.click();

        const errorMessage = page.getErrorMessagesForInput('username');
        expect(errorMessage.getText()).toBe('No email for this user');
    });

    it('will display success message after the user is sent a reset link', () => {
        const username = page.getInput('username');
        username.sendKeys('testuser');
        const resetButton = page.getElement('.card__actions--request-reset');

        resetButton.click();

        const confirmation = page.getElement('.card__actions__confirmation');
        expect(confirmation.isDisplayed()).toBeTruthy();
    });
});

class RequestPasswordResetPage {
    navigateTo() {
        return browser.get('/oidc/request-reset-password');
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
