import { browser, by, element, protractor } from 'protractor';

describe('e2e RecoverUsernameComponent', () => {
    let recoverUsernamePage: RecoverUsernamePage;

    const portaitSize = { width: 320, height: 568 };
    const landscapeSize = { width: 568, height: 320 };
    const tabletSize = { width: 768, height: 1024 };

    describe('on portrait devices', () => {
        beforeEach(() => {
            recoverUsernamePage = new RecoverUsernamePage();
            recoverUsernamePage.setSize(portaitSize);
            recoverUsernamePage.navigateTo();
            recoverUsernamePage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on landscape devices', () => {
        beforeEach(() => {
            recoverUsernamePage = new RecoverUsernamePage();
            recoverUsernamePage.setSize(landscapeSize);
            recoverUsernamePage.navigateTo();
            recoverUsernamePage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on tablet+ devices', () => {
        beforeEach(() => {
            recoverUsernamePage = new RecoverUsernamePage();
            recoverUsernamePage.setSize(tabletSize);
            recoverUsernamePage.navigateTo();
            recoverUsernamePage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    function multiDeviceBehaviour() {
        it('will have an email field', () => {
            const email = recoverUsernamePage.getInput('email');

            expect(email.isDisplayed).toBeTruthy();
        });

        it('will have a recover button', () => {
            const recoverButton = recoverUsernamePage.getElement('.card__actions--recover-username');

            expect(recoverButton.isDisplayed).toBeTruthy();
        });

        it('will show a validation message if submitted without an email', () => {
            const recoverButton = recoverUsernamePage.getElement('.card__actions--recover-username');

            recoverButton.click();

            const errorMessages = recoverUsernamePage.getErrorMessagesForInput('email');
            expect(errorMessages.getText()).toBe('Required');
        });

        it('will show a validation message if submitted with an invalid email', () => {
            const recoverButton = recoverUsernamePage.getElement('.card__actions--recover-username');
            const email = recoverUsernamePage.getInput('email');
            email.sendKeys('not-an-email-address');

            recoverButton.click();

            const errorMessages = recoverUsernamePage.getErrorMessagesForInput('email');
            expect(errorMessages.getText()).toBe('Invalid email');
        });

        it('will show a validation message if no user has that email', () => {
            const recoverButton = recoverUsernamePage.getElement('.card__actions--recover-username');
            const email = recoverUsernamePage.getInput('email');
            email.sendKeys('non-existant@user.com');

            recoverButton.click();

            const errorMessage = recoverUsernamePage.getErrorMessagesForInput('email');
            expect(errorMessage.getText()).toBe('No user has this email');
        });

        it('will display success message if a user has the given email', () => {
            const email = recoverUsernamePage.getInput('email');
            email.sendKeys('testuser@lavalav.com');
            const recoverButton = recoverUsernamePage.getElement('.card__actions--recover-username');

            recoverButton.click();

            const confirmation = recoverUsernamePage.getElement('.card__actions__confirmation');
            expect(confirmation.isDisplayed()).toBeTruthy();
        });
    }
});

class RecoverUsernamePage {
    navigateTo() {
        return browser.get('/oidc/recover-username');
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
