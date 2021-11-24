# Rules

## RULE: no-hardcoded-resource-location

A resource's location should not use a hard-coded string or variable value. It should use a parameter, an expression (but not `resourceGroup().location` or `deployment().location`) or the string 'global'.

Best practice suggests that to set your resources' locations, your template should have a string parameter named `location`. This parameter may default to the resource group or deployment location.

Template users may have limited access to regions where they can create resources. A hard-coded resource location might block users from creating a resource, thus preventing them from using the template. By providing a location parameter that defaults to the resource group location, users can use the default value when convenient but also specify a different location.

### AUTO-FIXES AVAILABLE
* Change variable '{variable}' into a parameter
* Place '{literalValue}' into a new parameter
* Change '{literalValue}' to '{existing-parameter}

### Examples: Variable should be a parameter

FAIL:
  var location = 'westus'
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
      location: location
  }

  Error: A resource's location should not use a hard-coded string or variable value. It should use a parameter value, an expression or the string 'global'. Recommendation: Change variable 'location' into a parameter.
  Auto-fixes:
    * Change variable 'location' into a parameter.

FAIL:
  var v1 = 'westus'
  var v2 = v1
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
      location: storageLocation
  }

  Error: A resource's location should not use a hard-coded string or variable value. It should use a parameter value, an expression or the string 'global'. Recommendation: Change variable 'v1' into a Error: "A resource location should be a parameter value, an expression or the string '{0}'. It should not be a hard-coded string or variable value. Found '{1}'", //asdff
  Auto-fixes:
    * Change variable 'location' into a parameter.

### Examples: Should use a parameter instead of literal string

FAIL:
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: 'westus'
  }

  Error: A resource's location should not use a hard-coded string or variable value. It should use a parameter value, an expression or the string 'global'. Recommendation: Use a new or existing parameter.
  Auto-fixes:
    * Create a new parameter for 'westus' with default value 'westus'
    * Use existing parameter '<existing-parameter>' which has default value '<value>'

### Examples: Passing

PASS:
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: 'global'
  }

PASS:
  var location = 'GLOBAL'
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: location
  }

PASS:
  param p1 string = resourceGroup().location
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: p1
  }

PASS (expression):
  param condition bool
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: condition ? 'WestUS' : 'WestUS2'
  }

PASS (expression):
  var locations = [
    'WestUS'
    'EastUS'
  ]
  resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = [for i in range(0, 10): {
    name: 'name${i}'
    location: locations[i]
    kind: 'StorageV2'
    sku: {
      name: 'Premium_LRS'
    }
  }]

## RULE: no-calls-to-resourcegroup-or-deployment-location-outside-parameters

`resourceGroup().location` and `deployment().location` should only be used as the default value of a parameter.

Template users may have limited access to regions where they can create resources. The expressions `resourceGroup().location` or `deployment().location` could block users if the resource group or deployment was created in a region the user can't access, thus preventing them from using the template.

Best practice suggests that to set your resources' locations, your template should have a string parameter named `location`. By providing a location parameter that defaults to the resource group or deployment location instead of calling these directly elsewhere in the template, users can use the default value when convenient but also specify a different location.

### AUTO-FIXES AVAILABLE
* Change variable '{variable}' into a parameter
  (for scenario `var location = resourceGroup().location`)
* Create a new parameter 'location' with default value '{resourceGroup()/deployment().location}'
* Use existing '{existing-parameter-with-same-default-value}'

### Examples

PASS:
  param location string = resourceGroup().location

FAIL:
  var v1 = anyexpression(resourceGroup().location)
  
FAIL:
  resource ... {
    location: anyexpression(resourceGroup().location)
  }

## RULE: use-explicit-values-for-location-params-in-modules

  If a parameter in a module uses resourceGroup().location or deployment().location in its default value, it must be assigned an explicit value when the module is consumed.
  **ISSUE:** What is the exact rule?
    Maybe: When consuming a module, any location-related parameters that have a default value must be assigned an explicit values.

  Location-related parameters include parameters that have a default referencing resourceGroup().location or deployment().location and any parameter that is referenced from a resource's location property.
  **ISSUE:** Should it only be paramers referenced from a resource's location property?  Or only those referencing resourceGroup().location or deployment().location?


  In the main template, azuredeploy.json or mainTemplate.json, this parameter can default to the resource group location. In linked or nested templates, the location parameter shouldn't have a default location.

A 
Template users may have limited access to regions where they can create resources. 
A hard-coded resource location might block users from creating a resource. The "[resourceGroup().location]" expression could block users if the resource group was created in a region the user can't access. Users who are blocked are unable to use the template.

By providing a location parameter that defaults to the resource group location, users can use the default value when convenient but also specify a different location.

### Examples

Error: 
Fixes:
  1) Pass in '{param-with-resourceGroupOrDeploymentLocation-in-default-value}' 
FAIL:
   module1.bicep:
      param p1 string = resourceGroup().location  //p1  has a default value referencing resourceGroup().location
      param p2 string = 'westus' //p2 is referenced inside stg's location property

      resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
        name: 'stg'
        location: p2
        kind: 'StorageV2'
        sku: {
          name: 'Premium_LRS'
        }
      }
   main.bicep:
     param p1 string
     param p2 string
     param p3 string = resourceGroup().location
     param rgLocation string

     resource ... {
       location: p2
     }
     module m1 "module1.bicep" {
       params: {
         // FAILS: "someParam" isn't being given an explicit value, so it will default to resourceGroup().location, possibly causing
         //   unintended behavior
         // AUTO-FIXES:
         //   * Create new parameter `location`, add `someParam: location`
         //   * Add `someParam: p2`   (because p2 is used as a resource's location)
         //   * Add `someParam: p3`   (because p3 defaults to resourceGroup().location)
         //   * Add `someParam: rgLocation`   (because rgLocation has 'location' in the name)
       }
     }

PASS:
   main.bicep:
     param p1 string
     module m1 "module1.bicep" {
       params: {
         someParam: p1
       }
     }




asdfg module to json file?




ISSUE: How far data flow analysis (essentially type inference?)

E.g. 
  param location1 string = resourceGroup().location   <== counts as location parameter because references resourceGroup.location
  param location2 string  =             <== counts as location parameter becaused used in assignment of resource "location" property
  var myLocation = condition1 ? location1 : location2
  resource ... {
    location: myLocation
  }
  ISSUE: What about "condition1"?
ISSUE: What are actual usage patterns?

ALLOW:
var map = [

]




GOLD STANDARD BP:
param location = resourceGroup().location
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: location
}



BAD:

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: 'westus'
}

BAD?
var myparam = 'westus'
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

BAD?
var myparam = resourceGroup().location
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

BETTER?
param myparam = 'westus'
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

FINE:
param randomvar string = 'westus'

until used in 'location' property



