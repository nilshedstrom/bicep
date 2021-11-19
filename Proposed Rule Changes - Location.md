# Rules

## no-hardcoded-resource-location

A resource's location should not use a hard-coded string or variable value. It should use a parameter value, an expression or the string 'global'.

### AUTO-FIXES AVAILABLE
* Change variable '{variable}' into a parameter
* Place '{literalValue}' into a new parameter
* Change '{literalValue}' to '{existing-parameter-with-same-value}

### Examples

FAIL:
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: 'uswest'
  }
  Error: Do not use a hard-coded **string** for a resource's location property (except for 'global')
  Auto-fixes:
    * Create a new parameter for 'uswest'
      ['location' by default]
    * Change 'uswest' to '{existing-parameter-with-same-value}'
      asdfg: Which parameters exactly do we show?


FAIL:
  var location = 'westus'
  resource storageaccount... {
      location: location
  Error: Do not use a hard-coded **variable** for a resource's location property (except for 'global')
  Auto-fix: Turn 'location' into a parameter

FAIL:
  var location = 'westus'
  var storageLocation = location
  resource storageaccount... {
      location: storageLocation
  Error: Do not use a hard-coded **string** for a resource's location property (except for 'global')
  Error: "A resource location should be a parameter value, an expression or the string '{0}'. It should not be a hard-coded string or variable value. Found '{1}'", //asdff
  Auto-fix: Turn 'location' into a parameter **????** asdff

PASS:
  resource storageaccountt 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: 'global'
  }

PASS:
  var location = 'GLOBAL'
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: location
  }

PASS:
  param myLocationParameter = resourceGroup().location
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: myLocationParameter
  }

### Expressions are okay
PASS:
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: condition ? 'West US' : 'West US 2'
  }

PASS:
  var locations = [
    'West US'
    'East US'
  ]
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    location: locations[i]
  }

  var location = anyexpression()
  resource storageaccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
    location: location
  }

## no-calls-to-resourcegroup-or-deployment-location-outside-params

`resourceGroup().location` and `deployment().location` should only be used in the default value of a parameter.

asdfg ask Brian: param p2 string = 'westus'

### AUTO-FIXES AVAILABLE
* Change variable '{variable}' into a parameter.
  (for scenario `var location = resourceGroup().location`)
* Place '{resourceGroup()/deployment().location}' into a new parameter
* Change '{resourceGroup()/deployment().location}' to '{existing-parameter-with-same-value}'

### Examples

FAIL:
  var v1 = anyexpression(resourceGroup().location)

PASS:
  param p1 string location = resourceGroup().location

FAIL:
  resource ... {
    location: anyexpression(resourceGroup().location)
  }

## use-explicit-values-for-location-params-in-modules

If a parameter in a module uses resourceGroup().location or deployment().location in its default value, an explicit value must be passed in when consuming the module. asdfg explain

### Examples

Error: 
Fixes:
  1) Pass in '{param-with-resourceGroupOrDeploymentLocation-in-default-value}' 
FAIL:
   module1.bicep:
     param someParam string = resourceGroup().location   <<< has a default value referencing resourceGroup().location
     resource ... {
       location: someParam
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



