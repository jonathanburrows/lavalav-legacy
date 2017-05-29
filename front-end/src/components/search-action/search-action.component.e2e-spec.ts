import { browser, by, element, protractor } from 'protractor';

describe('e2e SearchActionComponent', () => {
    let splashPage: SplashPage;

    const portaitSize = { width: 320, height: 568 };
    const landscapeSize = { width: 568, height: 320 };
    const tabletSize = { width: 768, height: 1024 };

    describe('on portrait devices', () => {
        beforeEach(() => {
            splashPage = new SplashPage();
            splashPage.setSize(portaitSize);
            splashPage.navigateTo();
            splashPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on landscape devices', () => {
        beforeEach(() => {
            splashPage = new SplashPage();
            splashPage.setSize(landscapeSize);
            splashPage.navigateTo();
            splashPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    describe('on tablet+ devices', () => {
        beforeEach(() => {
            splashPage = new SplashPage();
            splashPage.setSize(tabletSize);
            splashPage.navigateTo();
            splashPage.disableAnimations();
        });

        multiDeviceBehaviour();
    });

    function multiDeviceBehaviour() {
        it('will have an expand button', () => {
            const expandButton = splashPage.getElement('.search__icon--search');

            expect(expandButton.isDisplayed()).toBe(true);
        });

        it('will hide the input by default', () => {
            const expander = splashPage.getElement('.search__expander');

            expect(expander.isDisplayed()).toBe(false);
        });

        it('will show the input after the expand button is pressed', () => {
            const expandButton = splashPage.getElement('.search__icon--search');
            expandButton.click();

            const expander = splashPage.getElement('.search__expander');

            expect(expander.isDisplayed()).toBe(true);
        });

        it('will hide the input after the expand button is pressed', () => {
            const expandButton = splashPage.getElement('.search__icon--search');
            expandButton.click();
            const collapseButton = splashPage.getElement('.search__icon--close');
            collapseButton.click();

            const expander = splashPage.getElement('.search__expander');

            expect(expander.isDisplayed()).toBe(false);
        });
    }
});

class SplashPage {
    navigateTo() {
        return browser.get('/component/lvl-layout');
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
