# Keysential

*Server-side global key manager.*

## Commands

### Start a new KeyManager

    start-key-manager
       --id="<manager-id>"
       --position=<x,y,z>
       --distance=<8>
       --keys="<key1,key2,key3>"
      [--type=<Vendor|DistanceXZ>]
      [--add]

### Stop an existing KeyManager

    stop-key-manager
       --id="<manager-id>"
      [--remove]
