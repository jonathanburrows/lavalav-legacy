## Goals
To provide a way to generate TypeScript models from the POCOs in a C# assembly.

## Requirements

### When converting a class to TypeScript, the following conditions must be satisfied:
The result must read:
"export class <class-name>"

If the class inherits from a base class, the result must read:
"export class <class-name> extends <base-class>"

If the class implements an interface, the result must read:
"export class <class-name> implements <interface-name>"

If the class implements two interfaces, the result must read:
"export class <class-name> implements <interface1>, <interface2>"

When the class is abstract, the result must reads:
"export abstract class <class-name>"

### When converting a class to TypeScript, the generated properties must satisfy the conditions:
The property name must be pascal case.

The property must have the public modifier.

If there is a CompareAttribute, the result must read:
"@Compare("<comparedProperty>") public <propertyName>"

If there is a CreditCardAttribute, the result must read:
"@CreditCard() public <propertyName>"

If there is an EmailAttribute, the result must read:
"@Email() public <propertyName>"

If there is a MaxLengthAttribute, the result must read:
"@MaxLength(<length>) public <propertyName>"

If there is a MinLengthAttribute, the result must read:
"@MinLength(<length>) public <propertyName>"

If there is a PhoneAttribute, the result must read:
"@Phone() public <propertyName>"

If there is a UrlAttribute, the result must read:
"@Url() public <propertyName>"

If there is a RangeAttribute, the result must read:
"@Range(<lower>, <upper>) public <propertyName>"

If there is a RegularExpressionAttribute, the result must read:
"@RegularExpression(/<regular-expression>/) public <propertyName>"

If there a RequiredAttribute, the result must read:
"@RequiredAttribute() public <propertyName>"

If there is a RequiredAttribute, and a PhoneNumberAttribute, the result must read:
"@RequiredAttribute() @PhoneNumberAttribute() public <propertyName>"

If the property type is a number or nullable number, decimal or nullable decimal, long or nullable long, double or nullable double; the result must read:
"public <propertyName>: number;"

If the property type is a string, the result must read:
"public <propertyName>: string;"

If the property type is a DateTime or a nullable DateTime, the result must read:
"public <propertyName>: Date;"

If the property type is a boolean or a nullable boolean, the result must read:
"public <propertyName>: boolean;"

If the property type is not a structure, then the result must read:
"public <propertyName>: <propertyType>;"

If there is a single property on a base class, and the property type is mapped to a different npm package, then the result must read:
"import { <propertyType> } from '<module-path>';"

If there are two properties on a base class, and the property types are mapped to a single npm package, then the result must read:
"import { <propertyType1>, <propertyType2> } from '<module-path>';"

If there are two properties on a base class, and the property types are mapped to two different npm packages, then the result must read:
"import { <propertyType1> } from '<module-path1>';
 import { <propertyType2> } from '<module-path2>';"

 If there is a single property on a base class, and the property type comes from the same assembly, then the result must read:
 "import { <propertyType> } from './<propertyType>';"

### When converting a class to TypeScript, the import statements must satisfy the conditions:

 If the class inherits from a base class, and the base class is from a different npm package, the result must read:
 "import { <baseClass> } from '<module-path>';"

If the class inherits from a base class, and the base class is from the same npm package, the result must read:
"import { <baseClass> } from './<baseClass>';"

If the class implements a single interface, and the interface is mapped to a different npm package, then the result must read:
"import { <interface> } '<module-path>';"

If the class implements two interfaces, and the interfaces are mapped to different npm packages, then the result must read:
"import { <interface1> } from '<module-path1>';
 import { <interface2> } from '<module-path2>';"

If the class implements two interfaces, and the interfaces are mapped to the same npm package, then the result must read:
"import { <interface1>, <interface2> } from '<module-path>';"

If the class implements an interface from the same npm package, the result must read:
"import { <interface> } from './<interface>';"

If the class has two properties, and the properties have identical names, the result must read:
"import { <property-type> } from '<module-path2>';
 import { <property-type> as <property-type>_2 } from '<module-path2>';
 
 export class <class-name> {
	public <propertyName1>: <property-type>;
	public <propertyName2>: <property-type>_2;
 }"

If the class inherits from a base class, and has a property with the same type name but from differnet npm packages, the result must read:
"import { <base-class> } from '<module-path1>';
 import { <propery-type> as <property-type>_2 } from '<module-path2>';

 export class <class-name> extends <base-class> {
	public <propertyName>: <property-type>_2:
 }"

If the class inherits from two interfaces with identical names, from different npm packages, then the result must read:
"import { <interface1> } from '<module-path1>';
 import { <interface2> as <interface2>_2 } from '<module-path2>';
 
 export class <class-name> implements <interface1>, <interface2>_2 {"

### When converting an interface to TypeScript, the following conditions must be satisfied:
The result must read:
"export interface <interface-name>"

If the interface implements an interface, it must read:
"export interface <interface-name> implements <interface> {"

If the interface implements two interfaces, it must read:
"export interface <interface-name> implements <interface1>, <interface2> {"

### When converting an interface to TypeScript, the generated properties must satisfy the conditions:
If the property type is an integer, decimal, long, or double, then the result must read:
"<propertyName>: number;"

If the property type is a nullable integer, decimal, long, or double, then the result must read:
"<propertyName>?: number;"

If the property type is a string, the result must read:
"public <propertyName>?: string;"

If the property type is a DateTime, the result must read:
"public <propertyName>: Date;"

If the property type is a nullable DateTime, the result must read:
"public <propertyName>?: Date;"

If the property type is a boolean, the result must read:
"public <propertyName>: boolean;"

If the property type is a boolean, the result must read:
"public <propertyName>?: boolean;"

If the property type is not a structure, then the result must read:
"public <propertyName>?: <propertyType>;"

If there is a single property on an interface, and the property type is mapped to a different npm package, then the result must read:
"import { <propertyType> } from '<module-path>';"

If there are two properties on an interface, and the property types are mapped to a single npm package, then the result must read:
"import { <propertyType1>, <propertyType2> } from '<module-path>';"

If there are two properties on an interface, and the property types are mapped to two different npm packages, then the result must read:
"import { <propertyType1> } from '<module-path1>';
 import { <propertyType2> } from '<module-path2>';"

 If there is a single property on an interface, and the property type comes from the same assembly, then the result must read:
 "import { <propertyType> } from './<propertyType>';"