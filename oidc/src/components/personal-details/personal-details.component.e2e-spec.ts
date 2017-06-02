import { browser, by, element, protractor } from 'protractor';

describe('e2e PersonalDetailsComponent', () => {
    let personalDetailsPage: PersonalDetailsPage;

    beforeEach(() => {
        personalDetailsPage = new PersonalDetailsPage();
        personalDetailsPage.signIn();
        personalDetailsPage.navigateTo();
        personalDetailsPage.disableAnimations();
    });

    it('will persist the first name when saved', () => {
        const firstName = personalDetailsPage.getInput('firstName');
        firstName.clear();
        firstName.sendKeys('my-first-name');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedFirstName = personalDetailsPage.getInput('firstName');
        expect(updatedFirstName.getAttribute('value')).toBe('my-first-name');
    });

    it('will persist the last name when saved', () => {
        const lastName = personalDetailsPage.getInput('lastName');
        lastName.clear();
        lastName.sendKeys('my-last-name');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedLastName = personalDetailsPage.getInput('lastName');
        expect(updatedLastName.getAttribute('value')).toBe('my-last-name');
    });

    it('will persist the email when saved', () => {
        const email = personalDetailsPage.getInput('email');
        email.clear();
        email.sendKeys('my-email@email.ca');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedEmail = personalDetailsPage.getInput('email');
        expect(updatedEmail.getAttribute('value')).toBe('my-email@email.ca');
    });

    it('will persist the phone number when saved', () => {
        const phoneNumber = personalDetailsPage.getInput('phoneNumber');
        phoneNumber.clear();
        phoneNumber.sendKeys('1234567');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedPhoneNumber = personalDetailsPage.getInput('phoneNumber');
        expect(updatedPhoneNumber.getAttribute('value')).toBe('1234567');
    });

    it('will persist the phone number when saved', () => {
        const phoneNumber = personalDetailsPage.getInput('phoneNumber');
        phoneNumber.clear();
        phoneNumber.sendKeys('1234567');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedPhoneNumber = personalDetailsPage.getInput('phoneNumber');
        expect(updatedPhoneNumber.getAttribute('value')).toBe('1234567');
    });

    it('will persist the job when saved', () => {
        const job = personalDetailsPage.getInput('job');
        job.clear();
        job.sendKeys('I work hard play hard');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedJob = personalDetailsPage.getInput('job');
        expect(updatedJob.getAttribute('value')).toBe('I work hard play hard');
    });

    it('will persist the location when saved', () => {
        const location = personalDetailsPage.getInput('location');
        location.clear();
        location.sendKeys('Victoria');
        const saveButton = personalDetailsPage.getElement('.save-button');

        saveButton.click();
        browser.refresh();

        const updatedLocation = personalDetailsPage.getInput('location');
        expect(updatedLocation.getAttribute('value')).toBe('Victoria');
    });
});

class PersonalDetailsPage {
    navigateTo() {
        return browser.get('/oidc/personal-details');
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
