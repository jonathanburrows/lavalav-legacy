const { SpecReporter } = require('jasmine-spec-reporter');

exports.config = {
    allScriptsTimeout: 11000,
    specs: [
        './src/**/*.e2e-spec.ts'
    ],
    multiCapabilities: [{
        'browserName': 'chrome', chromeOptions: { args: ["--headless", "--disable-gpu"] }
    }],
    directConnect: true,
    baseUrl: 'http://localhost:5007/',
    framework: 'jasmine',
    jasmineNodeOpts: {
        showColors: true,
        defaultTimeoutInterval: 60000,
        print: function () { }
    },
    beforeLaunch: function () {
        require('ts-node').register({
            project: 'tsconfig.e2e.json'
        });
    },
    onPrepare() {
        //require("zone.js/dist/zone-node");
        //require('zone.js/dist/long-stack-trace-zone');
        //require('zone.js/dist/async-test');
        //require('zone.js/dist/fake-async-test');
        //require('zone.js/dist/sync-test');
        //require('zone.js/dist/proxy');
        //require('zone.js/dist/jasmine-patch');

        jasmine.getEnv().addReporter(new SpecReporter({ spec: { displayStacktrace: true } }));
    }
};