# NeuralNetworkProj

This is my first Unity Engine Project, and first attempt at working on Neural Networks.

Currently the NN is not connected, at the moment it is running off of Hardcoded Behaviours.

Features:
- Neural Network for Behavioural Evolution
- Actors that simulate Physical Evolution (and act based on their Behavioural Evolution)
  - Races (Human, Elf, Ork, Dwarves, Primals)
    - Primals are for random spawning purposes and will become another randomly chosen race on spawn.
    - Races can only mate with races of the same kind
  - Attributes 
    - Strength; Effects Damage, and move speed while grabbing
    - Dexterity; Effects move speed, and attack speed
    - Constitution; Max Health and Energy
    - Wisdom; Perception FOV
    - Intelligence; Perception Distance
    - Charisma; Nothing atm
  - Energy, Health & Maturity
    - Energy works as a hunger bar.
    - If Energy is empty health begins to drain
    - If Health is zero the creature dies.
    - If Maturity is greater than 100 the creatures health begins to drain. (To be set to a set-able lifespan)
    - If Maturity is above 10 an Actor can become aggressive and attack others
    - If Maturity is above 18 (or set mature age) an Actor can reproduce with another. (So long as they have enough energy)
  - Reproduction
    - Actors will seek out other Actors who are ready to reproduce and attempt to reproduce aswell
    - Children are a skewed average towards the most extreme values, in order to promote diversity (Subject to Change)
  - Evolution
    - If an Actor has enough energy, and has recently given birth (Subject to Change) they can attempt to evolve
    - Evolution Attempts have a cool down
    - If an Actor Manages to evolve they gain a random value between 1f-2f given towards their attributes.
      - I have found that the typical number of evolutions in a life is somewhere between 2-4.
  - Aggression
    - If an Actor is hungry enough and old enough they will also look for other Actors to attack as well as food.
    - (Behaviour subject to change)
- Organics
  - Plants that grow and slowly die off
  - They can be combined into larger plants and fed other plants to be kept alive.
- Spawners
  - Used to maintain a minimum population.
