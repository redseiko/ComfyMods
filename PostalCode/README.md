# PostalCode

*LocationInstance control and customization.*

## Commands

### register-location

Register a new LocationInstance.

    register-location --prefab=<string> --position=<x,y,z> --generated=<bool>
    register-location --p=<string> --pos=<x,y,z> --g=<bool>

### deregister-location

Deregister an existing LocationInstance.

    deregister-location --position=<x,y,z>
    deregister-location --pos=<x,y,z>

### list-location

List all LocationInstances for specified prefab.

    list-location --prefab=<string>
    list-location --p=<string>
