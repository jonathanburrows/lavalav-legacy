# Personal Details Component

## Goals
Provide a way for users to edit their claims, so they can change their identity.



## Syntax
```html
<lvl-oidc-personal-details></lvl-oidc-personal-details>
```




## Requirements
It is expected that the component:

Has an email input, that will display a validation message if submitted with an invalid email.


Has a first name input.


Has a last name input.


Has a phone number input, that will only accept numbers.


Has a job input.


Has a location input.


Has a save button.


Will show a validation message when the save button is pressed with an invalid email.


Will persist each input's changes when the save button is pressed.



## Roles
Only users with the 'oidc' role can access this component.



## Menu
This will not appear on the menu.



## Remarks
By default, the route /oidc/personal-details maps to this component.