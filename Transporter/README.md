# Transporter

*Server-side teleport manager.*

## Commands

### teleport-player

Teleport one or more players to a destination.

    teleport-player --player-id=<id1,id2,id3> --destination=<x,y,z>
    teleport-player --pid=<id1,id2,id3>       --d=<x,y,z>

### cancel-teleport-player

Cancel a pending teleport for one or more players.

    cancel-teleport-player --player-id=<id1,id2,id3>
    cancel-teleport-player --pid=<id1,id2,id3>
