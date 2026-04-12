## N-Tier Architecture
![N-tier Architecture](./diagrams/n-tier.svg)

A layer can only talk to the layer directly beneath it. The UI talks to the Logic. The Logic talks to the Database. The UI never talks directly to the Database