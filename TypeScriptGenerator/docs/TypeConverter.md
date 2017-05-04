## Goals
To provide a way to generate TypeScript models from the POCOs in a C# assembly.

## Requirements

### When converting a class to TypeScript, the following conditions must be satisfied:
The result must read:
"export class &lt;class-name&gt;"

If the class inherits from a base class, the result must read:
"export class &lt;class-name&gt; extends &lt;base-class&gt;"

If the class implements an interface, the result must read:
"export class &lt;class-name&gt; implements &lt;interface-name&gt;"

If the class implements two interfaces, the result must read:
"export class &lt;class-name&gt; implements &lt;interface1&gt;, &lt;interface2&gt;"

When the class is abstract, the result must reads:
"export abstract class &lt;class-name&gt;"

### When converting a class to TypeScript, the generated properties must satisfy the conditions:
The property must have the public modifier.

The property name must be pascal case.

If there is a CompareAttribute, the result must read:
"@Compare("&lt;comparedProperty&gt;") public &lt;propertyName&gt;"

If there is a CreditCardAttribute, the result must read:
"@CreditCard() public &lt;propertyName&gt;"

If there is an EmailAttribute, the result must read:
"@Email() public &lt;propertyName&gt;"

If there is a MaxLengthAttribute, the result must read:
"@MaxLength(&lt;length&gt;) public &lt;propertyName&gt;"

If there is a MinLengthAttribute, the result must read:
"@MinLength(&lt;length&gt;) public &lt;propertyName&gt;"

If there is a PhoneAttribute, the result must read:
"@Phone() public &lt;propertyName&gt;"

If there is a UrlAttribute, the result must read:
"@Url() public &lt;propertyName&gt;"

If there is a RangeAttribute, the result must read:
"@Range(&lt;lower&gt;, &lt;upper&gt;) public &lt;propertyName&gt;"

If there is a RegularExpressionAttribute, the result must read:
"@RegularExpression(/&lt;regular-expression&gt;/) public &lt;propertyName&gt;"

If there a RequiredAttribute, the result must read:
"@RequiredAttribute() public &lt;propertyName&gt;"

If there is a RequiredAttribute, and a PhoneNumberAttribute, the result must read:
"@RequiredAttribute() @PhoneNumberAttribute() public &lt;propertyName&gt;"

If the property type is a number or nullable number, decimal or nullable decimal, long or nullable long, double or nullable double; the result must read:
"public &lt;propertyName&gt;: number;"

If the property type is a string, the result must read:
"public &lt;propertyName&gt;: string;"

If the property type is a DateTime or a nullable DateTime, the result must read:
"public &lt;propertyName&gt;: Date;"

If the property type is a boolean or a nullable boolean, the result must read:
"public &lt;propertyName&gt;: boolean;"

If the property type is not a structure, then the result must read:
"public &lt;propertyName&gt;: &lt;propertyType&gt;;"

If there is a single property on a base class, and the property type is mapped to a different npm package, then the result must read:
"import { &lt;propertyType&gt; } from '&lt;module-path&gt;';"

If there are two properties on a base class, and the property types are mapped to a single npm package, then the result must read:
"import { &lt;propertyType1&gt;, &lt;propertyType2&gt; } from '&lt;module-path&gt;';"

If there are two properties on a base class, and the property types are mapped to two different npm packages, then the result must read:
"import { &lt;propertyType1&gt; } from '&lt;module-path1&gt;';
 import { &lt;propertyType2&gt; } from '&lt;module-path2&gt;';"

 If there is a single property on a base class, and the property type comes from the same assembly, then the result must read:
 "import { &lt;propertyType&gt; } from './&lt;propertyType&gt;';"

### When converting a class to TypeScript, the import statements must satisfy the conditions:

 If the class inherits from a base class, and the base class is from a different npm package, the result must read:
 "import { &lt;baseClass&gt; } from '&lt;module-path&gt;';"

If the class inherits from a base class, and the base class is from the same npm package, the result must read:
"import { &lt;baseClass&gt; } from './&lt;baseClass&gt;';"

If the class implements a single interface, and the interface is mapped to a different npm package, then the result must read:
"import { &lt;interface&gt; } '&lt;module-path&gt;';"

If the class implements two interfaces, and the interfaces are mapped to different npm packages, then the result must read:
"import { &lt;interface1&gt; } from '&lt;module-path1&gt;';
 import { &lt;interface2&gt; } from '&lt;module-path2&gt;';"

If the class implements two interfaces, and the interfaces are mapped to the same npm package, then the result must read:
"import { &lt;interface1&gt;, &lt;interface2&gt; } from '&lt;module-path&gt;';"

If the class implements an interface from the same npm package, the result must read:
"import { &lt;interface&gt; } from './&lt;interface&gt;';"

If the class has two properties, and the properties have identical names, the result must read:
"import { &lt;property-type&gt; } from '&lt;module-path2&gt;';
 import { &lt;property-type&gt; as &lt;property-type&gt;_2 } from '&lt;module-path2&gt;';
 
 export class &lt;class-name&gt; {
	public &lt;propertyName1&gt;: &lt;property-type&gt;;
	public &lt;propertyName2&gt;: &lt;property-type&gt;_2;
 }"

If the class inherits from a base class, and has a property with the same type name but from differnet npm packages, the result must read:
"import { &lt;base-class&gt; } from '&lt;module-path1&gt;';
 import { &lt;propery-type&gt; as &lt;property-type&gt;_2 } from '&lt;module-path2&gt;';

 export class &lt;class-name&gt; extends &lt;base-class&gt; {
	public &lt;propertyName&gt;: &lt;property-type&gt;_2:
 }"

If the class inherits from two interfaces with identical names, from different npm packages, then the result must read:
"import { &lt;interface1&gt; } from '&lt;module-path1&gt;';
 import { &lt;interface2&gt; as &lt;interface2&gt;_2 } from '&lt;module-path2&gt;';
 
 export class &lt;class-name&gt; implements &lt;interface1&gt;, &lt;interface2&gt;_2 {"

### When converting an interface to TypeScript, the following conditions must be satisfied:
The result must read:
"export interface &lt;interface-name&gt;"

If the interface implements an interface, it must read:
"export interface &lt;interface-name&gt; implements &lt;interface&gt; {"

If the interface implements two interfaces, it must read:
"export interface &lt;interface-name&gt; implements &lt;interface1&gt;, &lt;interface2&gt; {"

### When converting an interface to TypeScript, the generated properties must satisfy the conditions:
If the property type is an integer, decimal, long, or double, then the result must read:
"&lt;propertyName&gt;: number;"

If the property type is a nullable integer, decimal, long, or double, then the result must read:
"&lt;propertyName&gt;?: number;"

If the property type is a string, the result must read:
"public &lt;propertyName&gt;?: string;"

If the property type is a DateTime, the result must read:
"public &lt;propertyName&gt;: Date;"

If the property type is a nullable DateTime, the result must read:
"public &lt;propertyName&gt;?: Date;"

If the property type is a boolean, the result must read:
"public &lt;propertyName&gt;: boolean;"

If the property type is a boolean, the result must read:
"public &lt;propertyName&gt;?: boolean;"

 If there is a single property on an interface, and the property type comes from the same assembly, then the result must read:
 "import { &lt;propertyType&gt; } from './&lt;propertyType&gt;';"

If the property type is not a structure, then the result must read:
"public &lt;propertyName&gt;?: &lt;propertyType&gt;;"

If there is a single property on an interface, and the property type is mapped to a different npm package, then the result must read:
"import { &lt;propertyType&gt; } from '&lt;module-path&gt;';"

If there are two properties on an interface, and the property types are mapped to a single npm package, then the result must read:
"import { &lt;propertyType1&gt;, &lt;propertyType2&gt; } from '&lt;module-path&gt;';"

If there are two properties on an interface, and the property types are mapped to two different npm packages, then the result must read:
"import { &lt;propertyType1&gt; } from '&lt;module-path1&gt;';
 import { &lt;propertyType2&gt; } from '&lt;module-path2&gt;';"