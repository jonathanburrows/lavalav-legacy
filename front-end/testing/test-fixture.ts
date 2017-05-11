import { PlatformRef, Type } from '@angular/core';
import {
    async,
    ComponentFixture,
    getTestBed,
    TestBed
} from '@angular/core/testing';
import { BrowserModule } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';

import { NonResettingTestBed } from './non-resetting-test-bed';

/** Compiles an application for testing purposes, and provides a test bed which doesnt reset after each describe.
 *  @remarks - this was added for performance reasons. Without, it was compiling for each describe block. */
export class TestFixtures {
    private static testBed: TestBed;

    /**
     *  Sets the test bed for the first time. Should be used in test.ts, or a common area before testing begins.
     */
    public static initTestBed<TAppModule>(appModuleType: Type<TAppModule>, ngModule: Type<any>, platform: PlatformRef) {
        if (!appModuleType) {
            throw new Error('appModuleType is null.');
        }
        if (!ngModule) {
            throw new Error('ngModule is null.');
        }
        if (!platform) {
            throw new Error('platform is null.');
        }

        this.testBed = new NonResettingTestBed();

        this.testBed.initTestEnvironment(ngModule, platform);

        this.testBed.configureTestingModule({
            imports: [appModuleType, RouterTestingModule]
        });
        this.testBed.overrideModule(BrowserModule, ngModule);
        this.testBed.ngModule = ngModule;
        this.testBed.compileComponents();
    }

    /**
     *  Returns the test bed which doesnt reset.
     */
    public static getTestBed() {
        if (!this.testBed) {
            throw new Error('initTestBed has not been called');
        }
        return this.testBed;
    }
}
