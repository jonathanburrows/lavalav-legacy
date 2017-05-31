import { browser, by, element, protractor } from 'protractor';

describe('e2e ContentComponent', () => {
    let contentPage: ContentPage;

    beforeEach(() => {
        contentPage = new ContentPage();
        contentPage.navigateTo();
    });

    it('will render lvl-content-title directives', () => {
        const title = contentPage.getElement('.lvl-content-title');

        expect(title.isDisplayed()).toBeTruthy();
    });

    it('will render lvl-content-subtitle-title directives', () => {
        const subtitle = contentPage.getElement('.lvl-content-subtitle');

        expect(subtitle.isDisplayed()).toBeTruthy();
    });

    it('will render lvl-content-body directives', () => {
        const contentBody = contentPage.getElement('.lvl-content-body');

        expect(contentBody.isDisplayed()).toBeTruthy();
    });

    it('will render lvl-content-avatar directives', () => {
        const avatar = contentPage.getElement('.lvl-content-avatar');

        expect(avatar.isDisplayed()).toBeTruthy();
    });
});

class ContentPage {
    navigateTo() {
        return browser.get('/component/lvl-content');
    }

    getElement(query: string) {
        return element(by.css(query));
    }
}
