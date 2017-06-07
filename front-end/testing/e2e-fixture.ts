import { browser, by, element, protractor } from 'protractor';

/**
 *  Collection of functions common to tests.
 */
class E2eFixture {
    private tokenKey = 'oidc:bearer-token';

    constructor(private path: string) { }

    /**
     *  Removes any css animations.
     */
    disableAnimations() {
        // Sorry for the hack, the protractor DisableAnimations for angular 2 isnt supported yet.
        // There is a class in index.html which prevents the animations.
        const scriptHack = `document.body.className = document.body.className + ' no-animations';`;
        browser.executeScript(scriptHack);
    }

    /**
     *  Change the url to the path of the fixture.
     */
    navigateTo() {
        browser.get(this.path);
    }

    /**
     *  Change the size of the screen to a phone held vertically.
     */
    setToPortrait() {
        browser.driver.manage().window().setSize(320, 568);
    }

    /**
     *  Change the size of the screen to a phone held sideways.
     */
    setToLandscape() {
        browser.driver.manage().window().setSize(568, 320);
    }

    /**
     *  Change the size of the screen to a typical tablet.
     */
    setToTablet() {
        browser.driver.manage().window().setSize(768, 1024);
    }

    /**
     *  Increase the size of the screen to the largest it can get.
     *  @remarks - a specific size wasnt used because it was causing issues with the tests when I was RDPing in.
     */
    setToDesktop() {
        browser.driver.manage().window().maximize();
    }

    /**
     *  Gets an element by performing a css query.
     */
    getElement(query: string) {
        return element(by.css(query));
    }

    /**
     *  Returns a reactive form element with the given name.
     *  @param inputName The name of the element given by [formControlName]
     */
    getInput(inputName: string) {
        return element(by.css(`[formControlName="${inputName}"]`));
    }

    /**
     *  Gets the error messages of the form element.
     *  @param inputName The name of the element given by [formControlName] who may have errors.
     */
    getInputErrorMessage(inputName: string) {
        const input = this.getInput(inputName);
        const container = input.element(by.xpath('ancestor::md-input-container'));
        return container.element(by.css('.mat-input-error'));
    }

    /**
     *  Signs the user in using the testuser account.
     *  @remarks This was added in @lvl/front for convenience. It would be complicated otherwise.
     */
    signIn() {
        browser.get('/oidc/sign-in');

        const username = this.getInput('username');
        username.sendKeys('personal-details-testuser');

        const password = this.getInput('password');
        password.sendKeys('password');

        const signIn = this.getElement('button.card__actions--sign-in');
        signIn.click();
    }

    /**
     *  Generates a new user, with the username being a guid, and the password being password.
     *  This was done so the tests dont collide with one another.
     *  @returns the generated username.
     *  @remarks This was added in @lvl/front for convenience. It would be complicated otherwise.
     */
    register(): string {
        // tslint:disable:no-bitwise
        const username = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
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
    /**
     *  Logs the user out, and redirects them to the splash page.
     *  @remarks This was added in @lvl/front for convenience. It would be complicated otherwise.
     */
    signOut() {
        browser.executeScript(`localStorage.removeItem('${this.tokenKey}');`);
        browser.get('/');
    }
}
