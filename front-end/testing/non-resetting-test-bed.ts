import { TestBed, TestModuleMetadata } from '@angular/core/testing';

/**
 *  A test bed which prevents angular from resetting it after every describe block.
 *  @remarks - this was done due to performance constraints.
 */
export class NonResettingTestBed extends TestBed {
    /**
     * Returns the test bed without resetting the environment.
     */
    resetTestingEnvironment() {
        return this;
    }

    /**
     * Returns the test bed without resetting the module.
     */
    resetTestingModule() {
        return this;
    }

    /**
     * allows for a module to be set up without resetting the test bed.
     * @param moduleDef the module to be configured.
     */
    configureTestingModule(moduleDef: TestModuleMetadata) {
        super.configureTestingModule(moduleDef);
    }
}
