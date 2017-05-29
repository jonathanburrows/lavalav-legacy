import { browser, by, element, protractor } from 'protractor';

describe('e2e LayoutComponent', () => {
    let layoutPage: LayoutPage;

    describe('when in portrait mode', () => {
        beforeEach(() => {
            layoutPage = new LayoutPage();
            layoutPage.setToPortrait();
            layoutPage.navigateTo();
            layoutPage.disableAnimations();
        });

        it('will have a side nav title with the same height as the app bar', () => {
            const toolbar = layoutPage.getAppToolbar();
            const toolbarHeight = toolbar.getCssValue('height');
            const navHeader = layoutPage.getSideNavHeader();
            const navHeaderHeight = navHeader.getCssValue('height');

            expect(navHeaderHeight).toBe(toolbarHeight);
        });

        it('will hide the side nav on load', () => {
            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeFalsy();
        });

        it('will have a menu button', () => {
            const menuButton = layoutPage.getMenuButton();

            expect(menuButton.isDisplayed()).toBeTruthy();
        });

        it('will expand the side nav when the menu button is pressed', () => {
            const menuButton = layoutPage.getMenuButton();
            menuButton.click();

            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeTruthy();
        });
    });

    describe('when in landscape mode', () => {
        beforeEach(() => {
            layoutPage = new LayoutPage();
            layoutPage.setToLandscape();
            layoutPage.navigateTo();
            layoutPage.disableAnimations();
        });

        it('will have a side nav title with the same height as the app bar', () => {
            const toolbar = layoutPage.getAppToolbar();
            const toolbarHeight = toolbar.getCssValue('height');
            const navHeader = layoutPage.getSideNavHeader();
            const navHeaderHeight = navHeader.getCssValue('height');

            expect(navHeaderHeight).toBe(toolbarHeight);
        });

        it('will hide the side nav on load', () => {
            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeFalsy();
        });

        it('will have a menu button', () => {
            const menuButton = layoutPage.getMenuButton();

            expect(menuButton.isDisplayed()).toBeTruthy();
        });

        it('will expand the side nav when the menu button is pressed', () => {
            const menuButton = layoutPage.getMenuButton();
            menuButton.click();

            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeTruthy();
        });

        it('will collapse the side nav when another area of the screen is clicked', () => {
            const sideNav = layoutPage.getSideNav();
            const menuButton = layoutPage.getMenuButton();
            menuButton.click();

            const overlay = layoutPage.getOverlay();
            browser.wait(protractor.ExpectedConditions.elementToBeClickable(overlay), 1001);
            overlay.click();

            expect(sideNav.isDisplayed()).toBeFalsy();
        });
    });

    describe('when viewing on a tablet', () => {
        beforeEach(() => {
            layoutPage = new LayoutPage();
            layoutPage.setToTablet();
            layoutPage.navigateTo();
            layoutPage.disableAnimations();
        });

        it('will have a side nav title with the same height as the app bar', () => {
            const toolbar = layoutPage.getAppToolbar();
            const toolbarHeight = toolbar.getCssValue('height');
            const navHeader = layoutPage.getSideNavHeader();
            const navHeaderHeight = navHeader.getCssValue('height');

            expect(navHeaderHeight).toBe(toolbarHeight);
        });

        it('will hide the side nav on load', () => {
            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeFalsy();
        });

        it('will have a menu button', () => {
            const menuButton = layoutPage.getMenuButton();

            expect(menuButton.isDisplayed()).toBeTruthy();
        });

        it('will expand the side nav when the menu button is pressed', () => {
            const menuButton = layoutPage.getMenuButton();
            menuButton.click();

            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeTruthy();
        });

        it('will collapse the side nav when another area of the screen is clicked', () => {
            const sideNav = layoutPage.getSideNav();
            const menuButton = layoutPage.getMenuButton();
            menuButton.click();

            const overlay = layoutPage.getOverlay();
            browser.wait(protractor.ExpectedConditions.elementToBeClickable(overlay), 401);
            overlay.click();

            expect(sideNav.isDisplayed()).toBeFalsy();
        });
    });

    describe('when viewing on a desktop', () => {
        beforeEach(() => {
            layoutPage = new LayoutPage();
            layoutPage.setToDesktop();
            layoutPage.navigateTo();
            layoutPage.disableAnimations();
        });

        it('will have a side nav title with the same height as the app bar', () => {
            const toolbar = layoutPage.getAppToolbar();
            const toolbarHeight = toolbar.getCssValue('height');
            const navHeader = layoutPage.getSideNavHeader();
            const navHeaderHeight = navHeader.getCssValue('height');

            expect(navHeaderHeight).toBe(toolbarHeight);
        });

        it('will show the side nav on load', () => {
            const sideNav = layoutPage.getSideNav();

            expect(sideNav.isDisplayed()).toBeTruthy();
        });

        it('will not have a menu button', () => {
            const menuButton = layoutPage.getMenuButton();

            expect(menuButton.isDisplayed()).toBeFalsy();
        });
    });

    describe('side navigation', () => {
        beforeEach(() => {
            layoutPage = new LayoutPage();
            layoutPage.setToDesktop();
            layoutPage.navigateTo();
            layoutPage.disableAnimations();
        });

        it('will have a title for each group', () => {
            const groupTitle = layoutPage.getFirstNavigationGroup();

            expect(groupTitle.getText()).toBe('Front End');
        });

        it('will have a link for each navigation item', () => {
            const groupTitle = layoutPage.getFirstNavigationItem();

            expect(groupTitle.getText()).toBe('Layout');
        });

        it('will have a toolbar with the same text as the selected navigation item', () => {
            const selectedNavigation = layoutPage.getSelectedNavigationItem();
            const toolbarTitle = layoutPage.getToolbarTitle();

            expect(selectedNavigation.getText()).toBe(toolbarTitle.getText());
        });
    });
});

class LayoutPage {
    setToPortrait() {
        browser.driver.manage().window().setSize(320, 568);
    }

    setToLandscape() {
        browser.driver.manage().window().setSize(568, 320);
    }

    setToTablet() {
        browser.driver.manage().window().setSize(768, 1024);
    }

    setToDesktop() {
        browser.driver.manage().window().maximize();
    }

    navigateTo() {
        return browser.get('/component/lvl-layout');
    }

    getSideNavHeader() {
        return element(by.css('.side-nav__header'));
    }

    getAppToolbar() {
        return element(by.css('.toolbar'));
    }

    getSideNav() {
        return element(by.css('.side-nav'));
    }

    getMenuButton() {
        return element(by.css('.toolbar__menu-icon'));
    }

    getToolbarTitle() {
        return element(by.css('.toolbar__title'));
    }

    getOverlay() {
        return element(by.css('.side-nav__overlay'));
    }

    getFirstNavigationGroup() {
        return element.all(by.css('.side-nav__group__title')).get(0);
    }

    getFirstNavigationItem() {
        return element.all(by.css('.side-nav__group__item__text')).get(0);
    }

    getSelectedNavigationItem() {
        return element(by.css('.side-nav__group__item__title--selected'));
    }

    getSearchExpander() {
        return element(by.css('.toolbar__search__expander'));
    }

    getSearchIcon() {
        return element(by.css('.toolbar__search__icon:not(.toolbar__search__icon--close)'));
    }

    getCloseSearchIcon() {
        return element(by.css('.toolbar__search__icon--close'));
    }

    disableAnimations() {
        // Sorry for the hack, the protractor DisableAnimations for angular 2 isnt supported yet.
        // There is a class in index.html which prevents the animations.
        const scriptHack = `document.body.className = document.body.className + ' no-animations';`;
        browser.executeScript(scriptHack);
    }
}
