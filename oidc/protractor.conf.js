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
        jasmine.getEnv().addReporter(new SpecReporter({ spec: { displayStacktrace: true } }));
    }
};