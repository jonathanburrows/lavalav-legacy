import { browser, by, element, protractor } from 'protractor';

describe('e2e AccountMenuComponent', () => {
    const portaitSize = { width: 320, height: 568 };
    const landscapeSize = { width: 568, height: 320 };
    const tabletSize = { width: 768, height: 1024 };

    describe('on mobile devices', () => {
        it('will show a sign in icon when logged out', () => {
            const splashPage = new SplashPage();
            splashPage.setSize(landscapeSize);
            splashPage.navigateTo();
            splashPage.logOut();

            const signInButton = splashPage.getElement('.anonymous-menu__sign-in-icon');

            expect(signInButton.isDisplayed()).toBeTruthy();
        });
    });

    describe('on non-mobile devices', () => {
        it('will show a sign in button when logged out', () => {
            const splashPage = new SplashPage();
            splashPage.setSize(tabletSize);
            splashPage.navigateTo();
            splashPage.logOut();

            const signInButton = splashPage.getElement('.anonymous-menu__sign-in-button');

            expect(signInButton.isDisplayed()).toBeTruthy();
        });

        it('will show a register button when logged out', () => {
            const splashPage = new SplashPage();
            splashPage.setSize(tabletSize);
            splashPage.navigateTo();
            splashPage.logOut();

            const registerButton = splashPage.getElement('.anonymous-menu__register-button');

            expect(registerButton.isDisplayed()).toBeTruthy();
        });
    });

    describe('the account menu', () => {
        it('will not show up when logged out', () => {
            const splashPage = new SplashPage();
            splashPage.navigateTo();
            splashPage.logOut();

            const enu = splashPage.getElement('.menu');

            expect(enu.isPresent()).toBeFalsy();
        });

        it('will show up when logged in', () => {
            const splashPage = new SplashPage();
            splashPage.navigateTo();
            splashPage.signIn();

            const menu = splashPage.getElement('.menu');

            expect(menu.isDisplayed()).toBeTruthy();
        });

        it('will have a signout action', () => {
            const splashPage = new SplashPage();
            splashPage.navigateTo();
            splashPage.signIn();
            const menuButton = splashPage.getElement('.menu__icon');
            menuButton.click();
            const signOutButton = splashPage.getElement('.menu__sign-out');
            // timeout so the menu can open, the wait isnt working for it.
            browser.wait(protractor.ExpectedConditions.elementToBeClickable(signOutButton), 2000);

            signOutButton.click();

            expect(browser.getCurrentUrl()).toContain('oidc/credentials-signin');
        });

        it('will redirect the user to the signin page once the signout button is pressed', () => {
            const splashPage = new SplashPage();
            splashPage.navigateTo();
            splashPage.signIn();
            const menuButton = splashPage.getElement('.menu__icon');
            menuButton.click();

            const signOutButton = splashPage.getElement('.menu__sign-out');
            // timeout so the menu can open, the wait isnt working for it.
            browser.wait(protractor.ExpectedConditions.elementToBeClickable(signOutButton), 2000);

            expect(signOutButton.isDisplayed()).toBeTruthy();
        });
    });
});

class SplashPage {
    private tokenKey = 'oidc:bearer-token';

    navigateTo() {
        return browser.get('/');
    }

    logOut() {
        browser.executeScript(`localStorage.removeItem('${this.tokenKey}');`);
        browser.get('/');
    }

    signIn() {
        browser.get('/oidc/credentials-signin');
        const username = this.getInput('username');
        username.sendKeys('testuser');
        const password = this.getInput('password');
        password.sendKeys('password');
        const signIn = this.getElement('button.card__actions--sign-in');
        signIn.click();
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

    disableAnimations() {
        // Sorry for the hack, the protractor DisableAnimations for angular 2 isnt supported yet.
        // There is a class in index.html which prevents the animations.
        const scriptHack = `document.body.className = document.body.className + ' no-animations';`;
        browser.executeScript(scriptHack);
    }
}
