# Dramamist

*Mistlands ParticleMist effect reduction.*

![Splash](https://i.imgur.com/MGP8kyQ.png)

## Features

All configuration options can be toggled in-game and will take effect immediately.

![Configuration](https://i.imgur.com/A96oDo4.png)

### ParticleMist

#### ParticleMistReduceMotion

  * Modifies **all** ParticleMist particle Velocity/Rotation multipliers to 0.
  * Modifies ParticleMist particle StartRotation to 0.
  * Default: `true`

#### ParticleMistUseFlatMistStartColor

  * Modifies ParticleMist particle StartColor to use only the MinColor value instead of a MinColor/MaxColor range.
  * Default: `false`

#### ParticleMistDistantEmissionMaxVelocity

  * Modifies ParticleMist (distant) particle maximum starting Velocity (for Misters outside of player range).
  * Default: `2.0` (vanilla: `2.0`)

### Demister

#### DemisterForceFieldGravity

  * Modifies every Demister's force-field to apply a *negative* gravity effect to incoming ParticleMist particles.
  * **Moves ParticleMist particles away as quick as you move.**
  * Does not increase view distance but relies on particle motion for effect.
  * Default: `-5.0` (vanilla: `-0.08`)

### DemisterBall

#### DemisterTriggerFadeOutParticleMist

  * Adds a Sphere collider to DemisterBall (Wisp) Demisters that trigger a *fade-out* effect to particles inside.
  * **Increases view distance greater than vanilla but uses no particle motion for effect.**
  * Default: `true`

#### DemisterTriggerFadeOutMultiplier

  * Multiplier used for the FadeOutParticleMist option.
  * At `0%` particles will instantly fade-out and at `100%` particles will not fade-out.
  * Default: `85%`

## Notes

  * This is the *good enough* release with more features/options to be added later.
  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/Dramamist).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
