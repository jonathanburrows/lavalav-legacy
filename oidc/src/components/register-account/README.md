# Register Account Component

## Goals
Provide a way for users to create a new account, so they can access the application.



## Syntax
```html
<lvl-oidc-register-account></lvl-oidc-register-account>
```



## Requirements
It is expect that the register account component:

Has a username input, that:
1. Is required
2. Will display a Required validation message if left with no value
3. Will display a Required validation message if submitted with no value
4. Will display an 'Already taken, try another' validation message if submitted with a taken username


Has a password input, that:
1. Is required
2. Will display a Required validation message if left with no value
3. Will display a Required validation message if submitted with no value


It has a Register button.


It will display validation message when the register button is pressed with bad credentials.


It will create a user whent the register button is pressed.


It will provide the user with a token when the register button is pressed and the user is created.


It will redirect the user to the home page after the user pressed the register button.



## Remarks
By default, the route /oidc/register-account maps to this component.