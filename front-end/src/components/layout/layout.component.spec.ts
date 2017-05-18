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

    function getElement(query: string) {
        const debugElement = fixture.debugElement.query(By.css(query));
        if (!debugElement) {
            throw new Error(`Could not element that matches the query '${query}'`);
        }

        return debugElement.nativeElement;
    }
});
