import { Component } from '@angular/core';
import { async, TestBed } from '@angular/core/testing';
import { Http, HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/forkJoin';

import { ApiService } from './api.service';
import { FrontEndOptions } from '../front-end-options';
import { HeadersService } from '../headers';

import { Moon, Planet } from '../../../demo-app/models';

describe(ApiService.name, () => {
    let apiService: ApiService;

    // The following allows the api service to be initialized using dependency injection.
    beforeAll(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                ApiService,
                { provide: FrontEndOptions, useValue: { resourceServerUrl: 'http://localhost:5000' } },
                HeadersService
            ],
            declarations: [ServiceProviderWorkAroundComponent]
        });

        const fixture = TestBed.createComponent(ServiceProviderWorkAroundComponent);
        apiService = fixture.componentInstance.apiService;
    });

    describe('getCollection', () => {
        it('will throw an Error when the type is null', () => {
            expect(() => apiService.getCollection(null)).toThrowError();
        });

        it('will return an observable of all the items of that type from the server', async(() => {
            debugger;

            const moonCreations = [new Moon(), new Moon()].map(moon => apiService.create(Moon, moon));

            debugger;
            Observable.forkJoin(...moonCreations).subscribe(() => {
                debugger;
                apiService.getCollection(Moon).subscribe(moons => {
                    expect(moons.length).toBeGreaterThanOrEqual(2);
                });
            });
        }));

        it('will cast the returned items as the entity type', async(() => {
            const moonCreations = [new Moon()].map(moon => apiService.create(Moon, moon));

            Observable.forkJoin(...moonCreations).subscribe(() => {
                apiService.getCollection(Moon).subscribe(moons => {
                    expect(moons[0] instanceof Moon);
                });
            });
        }));

        it('will cast the 1-N children as their type', async(() => {
            const moon = new Moon();
            moon.planet = new Planet();

            apiService.create(Moon, moon).subscribe(_ => {
                apiService.getCollection(Moon).subscribe(moons => {
                    const moonWithPlanet = moons.filter(m => m.planet)[0];

                    expect(moonWithPlanet instanceof Planet);
                });
            });
        }));

        it('will cast the 1-N children as their type', async(() => {
            const planet = new Planet();
            planet.moons = [new Moon()];

            apiService.create(Planet, planet).subscribe(() => {
                apiService.getCollection(Planet).subscribe(planets => {
                    const planetWithMoon = planets.filter(p => p.moons && p.moons.length)[0];

                    expect(planetWithMoon.moons[0] instanceof Moon);
                });
            });
        }));
    });

    describe('get', () => {
        it('will throw an Error when entityType is null', () => {
            expect(() => apiService.get(null, 1)).toThrowError();
        });

        it('will throw an Error when id is null', () => {
            expect(() => apiService.get(Moon, null)).toThrowError();
        });

        it('will fetch an item from the resource server url', () => {
            const moon = new Moon();
            moon.name = 'my moon';

            apiService.create(Moon, moon).subscribe(created => {
                apiService.get(Moon, created.id).subscribe(fetched => {
                    expect(fetched.name).toBe(moon.name);
                });
            });
        });

        it('will cast the fetched item as the given entity type', () => {
            apiService.create(Moon, new Moon()).subscribe(created => {
                apiService.get(Moon, created.id).subscribe(fetched => {
                    expect(fetched instanceof Moon).toBeTruthy();
                });
            });
        });

        it('will cast the fetched items 1-1 child as the given entity type', () => {
            const moon = new Moon();
            moon.planet = new Planet();

            apiService.create(Moon, new Moon()).subscribe(created => {
                apiService.get(Moon, created.id).subscribe(fetched => {
                    expect(fetched.planet instanceof Planet).toBeTruthy();
                });
            });
        });

        it('will cast the fetched items 1-N child as the given entity type', () => {
            const planet = new Planet();
            planet.moons = [new Moon()];

            apiService.create(Planet, planet).subscribe(created => {
                apiService.get(Planet, created.id).subscribe(fetched => {
                    expect(fetched.moons[0] instanceof Moon).toBeTruthy();
                });
            });
        });
    });

    describe('create', () => {
        it('will throw an Error when entityType is null', () => {
            const creating = new Moon();
            expect(() => apiService.create(null, creating)).toThrowError();
        });

        it('will throw an Error when creating entity is null', () => {
            expect(() => apiService.create(Moon, null)).toThrowError();
        });

        it('will post the item to the server', async(() => {
            apiService.getCollection(Moon).subscribe(moonsBefore => {

                const creating = new Moon();
                apiService.create(Moon, creating).subscribe(_ => {
                    apiService.getCollection(Moon).subscribe(moonsAfter => {

                        expect(moonsBefore.length).toBeLessThan(moonsAfter.length);
                    });
                });
            });
        }));

        it('will resolve the observable with the created entity', async(() => {
            apiService.create(Moon, new Moon()).subscribe(created => {
                expect(created.id).toBeGreaterThan(0);
            });
        }));

        it('will return an instance of the entity type', async(() => {
            apiService.create(Moon, new Moon()).subscribe(created => {
                expect(created instanceof Moon).toBeTruthy();
            });
        }));

        it('will cast the 1-1 object as a type', async(() => {
            const moon = new Moon();
            moon.planet = new Planet();

            apiService.create(Moon, moon).subscribe(created => {
                expect(created.planet instanceof Planet).toBeTruthy();
            });
        }));

        it('will cast the 1-N object as a type', async(() => {
            const planet = new Planet();
            planet.moons = [new Moon()];

            apiService.create(Planet, planet).subscribe(created => {
                expect(created.moons[0].planet instanceof Planet).toBeTruthy();
            });
        }));
    });

    describe('update', () => {
        it('will throw an Error when entityType is null', () => {
            const updating = new Moon();
            expect(() => apiService.update(null, updating)).toThrowError();
        });

        it('will throw an Error when entity is null', () => {
            expect(() => apiService.update(Moon, null)).toThrowError();
        });

        it('will update the item to the server', async(() => {
            const moon = new Moon();
            moon.name = 'before update';

            apiService.create(Moon, moon).subscribe(updating => {
                updating.name = 'after update';

                apiService.update(Moon, updating).subscribe(_ => {
                    apiService.get(Moon, updating.id).subscribe(updated => {
                        expect(updated.name).toBe('after update');
                    });
                });
            });
        }));

        it('will return an instance of the entity type', async(() => {
            apiService.create(Moon, new Moon()).subscribe(created => {
                apiService.update(Moon, created).subscribe(updating => {
                    expect(updating instanceof Moon).toBeTruthy();
                });
            });
        }));

        it('will cast the 1-1 object as a type', async(() => {
            const moon = new Moon();
            moon.planet = new Planet();

            apiService.create(Moon, moon).subscribe(created => {
                apiService.update(Moon, created).subscribe(updating => {
                    expect(updating.planet instanceof Planet).toBeTruthy();
                });
            });
        }));

        it('will cast the 1-N object as a type', async(() => {
            const planet = new Planet();
            planet.moons = [new Moon()];

            apiService.create(Planet, planet).subscribe(created => {
                apiService.update(Planet, created).subscribe(updating => {
                    expect(updating.moons[0] instanceof Moon).toBeTruthy();
                });
            });
        }));
    });

    describe('delete', () => {
        it('will throw an Error when entityType is null', () => {
            const deleting = new Moon();
            expect(() => apiService.delete(null, deleting)).toThrowError();
        });

        it('will throw an Error when entity is null', () => {
            expect(() => apiService.delete(Moon, null)).toThrowError();
        });

        it('will remove an item from the server', () => {
            apiService.create(Moon, new Moon()).subscribe(created => {
                apiService.delete(Moon, created).subscribe(_ => {
                    apiService.getCollection(Moon).subscribe(remainingMoons => {
                        const matchingMoons = remainingMoons.filter(m => m.id === created.id);
                        expect(matchingMoons).toBe(0);
                    });
                });
            });
        });
    });
});

/**
 *  This component is used so that we can access the ApiService using dependency injection. It wasnt the first choice :(
 */
@Component({
    template: ''
})
class ServiceProviderWorkAroundComponent {
    constructor(public apiService: ApiService) { }
}

class UnmappedClass {
    id: number;
}
