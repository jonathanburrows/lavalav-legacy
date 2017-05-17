import { TestFixtures } from '../../../testing';
import { LayoutComponent } from './layout.component';
import { ComponentFixture } from '@angular/core/testing';
import { RouterOutlet } from '@angular/router';
import { By } from '@angular/platform-browser';

describe(LayoutComponent.name, () => {
    let fixture: ComponentFixture<LayoutComponent>;
    let component: LayoutComponent;

    beforeEach(() => {
        fixture = TestFixtures.getTestBed().createComponent(LayoutComponent);
        component = fixture.componentInstance;
    });

    it('will show the site title on on the side nav', () => {
        component.siteTitle = 'my site title';
        fixture.detectChanges();

        const sideNavTitle = getElement('.side-nav__header__title');

        expect(sideNavTitle.textContent).toBe('my site title');
    });

    it('will render router-outlet by default', () => {
        fixture.detectChanges();

        const routerOutlet = getElement('router-outlet');

        expect(routerOutlet).not.toBeNull();
    });

    it('will render router-outlet if content type is "router"', () => {
        component.contentType = 'router';
        fixture.detectChanges();

        const routerOutlet = getElement('router-outlet');

        expect(routerOutlet).not.toBeNull();
    });

    it('will render ng-content if content type is "content"', () => {
        component.contentType = 'content';
        fixture.detectChanges();

        const routerOutlet = fixture.debugElement.query(By.css('router-outlet'));

        expect(routerOutlet).toBeNull();
    });

    function getElement(query: string) {
        const debugElement = fixture.debugElement.query(By.css(query));
        if (!debugElement) {
            throw new Error(`Could not element that matches the query '${query}'`);
        }

        return debugElement.nativeElement;
    }
});
