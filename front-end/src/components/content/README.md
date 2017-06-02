# Content Component

## Goals
Provide a consitent format and structure to content and forms.



## Syntax
```html
<lvl-content>
	<lvl-content-title>My Page</lvl-content-title>
	<lvl-content-subtitle>My Special Section</lvl-content-subtitle>

	<lvl-content-body>
		<md-icon lvl-content-avatar>mail</md-icon> Words!!
	</lvl-content-body>
</lvl-content>
```



## lvl-content-title

### Goals
Denote the main title of a form/page, providing the highest visual hiearchy.



## lvl-content-subtitle

### Goals
Denote a description of a group of content, providing higher visual hiearchy than the content itself.



## lvl-content-body

### Goals
Denote the main content of the page, with the lowest visual hierarchy.



## [lvl-content-avatar]

### Goals
Have a icon which aligns vertically with the menu icon in the app bar.


### Remarks
In order to use it in an md-input, also decorate it with the mdPrefix attribute, and put it in the input container:
```html
<md-input-container>
	<md-icon mdPrefix lvl-content-avatar>search</md-icon>
	<input mdInput />
</md-input-container>
