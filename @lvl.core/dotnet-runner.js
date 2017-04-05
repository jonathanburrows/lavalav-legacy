#!/usr/bin/env node

// Used to build, link, and run dotnet projects from npm.

const fs = require('fs');
const path = require('path');
const exec = require('child_process').execSync;

// The first two arguments are nodejs parameters, the 3rd is the user entered value.
const args = process.argv.slice(2);
const projectPath = process.argv[2];
if (!projectPath) {
    throw new Error('The path to the project was not provided.');
}

/** @typedef {Object} PackageJson
 *  @property {string} name
 *  @property {string} bin */
const packageJson = parsePackageJson(projectPath);

compileProject(packageJson, projectPath);

linkProject(packageJson, projectPath);

// Process.argv includes node.js arguments in addition to the ones provided, 
// we want to ignore these, and the project name argument.
const projectArguments = process.argv.slice(3);
runProject(packageJson, projectPath, projectArguments);

/** Fetches the project json from the project path, and validates the information.
 *  @param {string} projectPath the directory to the project.json and package.json
 *  @returns {PackageJson} 
 *  @throws Project directory does not exist.
 *  @throws package.json does not exist.
 *  @throws package.json is missing name property.
 *  @throws package.json is missing bin property. */
function parsePackageJson(projectPath) {
    if (!fs.existsSync(projectPath)) {
        throw new Error(`Project directory ${projectPath} does not exists.`);
    }

    const packageJsonPath = path.join(projectPath, 'package.json');
    if (!fs.existsSync(packageJsonPath)) {
        throw new Error(`Package ${packageJsonPath} does not exist.`);
    }

    // @type {PackageJson}
    const packageJson = require(packageJsonPath);

    if (!packageJson.name) {
        throw new Error(`${packageJsonPath} is missing the required "name" property.`);
    }

    if (!packageJson.bin) {
        throw new Error(`${packageJsonPath} is missing the required "bin" property.`);
    }

    return packageJson;
}

/** Links a project as an npm package if not already linked. 
 *  @param {PackageJson} packageJson
 *  @param {string} projectPath
 *  @param {string[]} arguments */
function compileProject(packageJson, projectPath, arguments) {
    const executablePath = path.join(projectPath, packageJson.bin)
    if (fs.existsSync(executablePath)) {
        //we dont need to recompile it, already good to go.
        return;
    }
    exec(`dotnet build ${projectPath}`);
}

/** Links a project as an npm package if not already linked. 
 *  @param {PackageJson} packageJson
 *  @param {string} projectPath */
function linkProject(packageJson, projectPath) {
    // Using resolve with try catch is cleanest way to check if dependency already installed, sorry
    try {
        require.resolve(packageJson.name);
    }
    catch (e) {
        console.log(`linking package ${packageJson.name}`);
        exec(`npm link ${projectPath}`);
    }
}

/** Runs the command with the given arguments.
 *  @property {PackageJson} packageJson
 *  @property {string} projectPath
 *  @property {string[]} arguments */
function runProject(packageJson, projectPath, arguments) {
    const executablePath = path.join(projectPath, packageJson.bin);
    const command = `${executablePath} ${arguments.join(' ')}`;
    console.log(`Running command ${command}`);
    exec(command);
}
