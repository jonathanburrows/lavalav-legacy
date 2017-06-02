# Save Button Component

## Goals
Provide a save button that can be used in forms and has responsive animations.



## Syntax
The lvl-save-button must be decorated with a [formGroup] directive, containing the parent form.
```html
<lvl-save-button [formGroup]="mySpecialForm"></lvl-save-button>
```



## Requirements
If the form has not been touched, then it is expected that the button is hidden.


If the form has been modified by the user to put it into invalid state, then the button is expected to:
1. be visible
2. have a warning icon
3. be unpressable
4. have a white background
5. have an icon color of the warning text


If the form has been modified by the user, and is in a valid state, then the button is expected to:
1. be visible
2. have a save icon
3. be pressable
4. have the background color of the accent color


If the form has been submitted, and the save is in process, then the button is expected to:
1. a spinner appears
2. be unpressable
3. have a white background
4. have the spinner color be the accent color


After a form has been saved, then the button is expected to:
1. show a done icon
2. have an icon color of the accent color
3. be unpressable
4. have a white background
5. appear for a short amount of time, then disappear



### Remarks
In order to get full functionality, it is recommended that the [formGroup] directive be given a ValidatableForm, so the save events can be listened to.