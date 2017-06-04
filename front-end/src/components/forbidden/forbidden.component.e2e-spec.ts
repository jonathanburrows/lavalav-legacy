import { browser, by, element, protractor } from 'protractor';

describe('e2e ForbiddenComponent', () => {
    let forbiddenPage: ForbiddenPage;

    const portaitSize = { width: 320, height: 568 };
    const landscapeSize = { width: 568, height: 320 };
    const tabletSize = { width: 768, height: 1024 };

    beforeEach(() => {
        forbiddenPage = new ForbiddenPage();
        forbiddenPage.navigateTo();
        forbiddenPage.disableAnimations();
    });

    describe('on portrait devices', () => {
        it('will show a warning icon', () => {
            forbiddenPage.setSize(portaitSize);
            forbiddenPage.navigateTo();

            const icon = forbiddenPage.getElement('.forbidden__icon');

            expect(icon.isDisplayed()).toBeTruthy();
        });
    });

    describe('on landscape devices', () => {
        it('will hide the warning icon', () => {
            forbiddenPage.setSize(landscapeSize);
            forbiddenPage.navigateTo();

            const icon = forbiddenPage.getElement('.forbidden__icon');

            expect(icon.isDisplayed()).toBeFalsy();
        });
    });

    describe('on non-mobile devices', () => {
        it('will show the warning icon', () => {
            forbiddenPage.setSize(tabletSize);
            forbiddenPage.navigateTo();

            const icon = forbiddenPage.getElement('.forbidden__icon');

            expect(icon.isDisplayed()).toBeTruthy();
        });
    });
});

class ForbiddenPage {
    navigateTo() {
        return browser.get('/forbidden');
    }

    setSize(size: { width: number, height: number }) {
        return browser.driver.manage().window().setSize(size.width, size.height);
    }

    getElement(query: string) {
        return element(by.css(query));
    }

    disableAnimations() {
        // Sorry for the hack, the protractor DisableAnimations for angular 2 isnt supported yet.
        // There is a class in index.html which prevents the animations.
        const scriptHack = `document.body.className = document.body.className + ' no-animations';`;
        browser.executeScript(scriptHack);
    }
}
