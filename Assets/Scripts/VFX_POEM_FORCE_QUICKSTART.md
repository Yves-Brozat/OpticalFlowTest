# Configuration rapide - VFX Poem Zone Forces

## Résumé en 3 étapes

### 1️⃣ Dans le VFX Graph
Créez 8 propriétés booléennes dans le Blackboard :
```
Force1_Enabled (Boolean)
Force2_Enabled (Boolean)
Force3_Enabled (Boolean)
Force4_Enabled (Boolean)
Force5_Enabled (Boolean)
Force6_Enabled (Boolean)
Force7_Enabled (Boolean)
Force8_Enabled (Boolean)
```

Pour chaque Conform to Sphere :
- Option A : Activez/désactivez le bloc avec le booléen
- Option B : Multipliez l'intensité par le booléen

### 2️⃣ Dans PoemReaderPlaylist (Inspector)
```
VFX Integration
├─ Visual Effect: [Glisser-déposer le VisualEffect component]
└─ Force Property Names: [Vérifier les noms correspondent au VFX]

Zones Configuration
├─ Zone 0 → Force Index: 0
├─ Zone 1 → Force Index: 1
├─ Zone 2 → Force Index: 2
...
└─ Zone 7 → Force Index: 7
```

### 3️⃣ C'est tout ! ✅

Les forces s'activeront automatiquement quand les poèmes commencent à se lire.

## Exemple de structure VFX

```
Context Initialize
└─ ...

Context Update
├─ Force1_Enabled (Property)
├─ Force2_Enabled (Property)
└─ ...

Particle System
├─ Update Particle
│   ├─ Conform to Sphere (Zone 1)
│   │   └─ [Activer seulement si Force1_Enabled = true]
│   ├─ Conform to Sphere (Zone 2)
│   │   └─ [Activer seulement si Force2_Enabled = true]
│   └─ ...
```

## Vérification rapide

Activez `Show Debug Logs` dans PoemReaderPlaylist.
Appuyez sur une touche de zone.
Vous devriez voir :
```
[PoemReaderPlaylist] 🌀 Force 0 (Force1_Enabled) activée
```

Si vous voyez `⚠ Propriété VFX '...' introuvable !`, vérifiez les noms des propriétés.
