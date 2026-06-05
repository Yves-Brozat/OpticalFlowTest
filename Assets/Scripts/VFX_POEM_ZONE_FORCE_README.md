# VFX Poem Zone Force - Documentation

## Vue d'ensemble

Ce système permet d'activer des forces VFX (Conform to Sphere) individuelles dans le FlowVFX en fonction de l'état de lecture des zones de poèmes. Chaque zone de poème peut contrôler une force distincte.

**Mise à jour immédiate** : Les booléens VFX sont maintenant mis à jour instantanément via une référence directe au VisualEffect component, sans délai d'attente.

## Composants

### 1. PoemReaderPlaylist (modifié)

Le composant principal qui gère les zones de poèmes **et** contrôle directement les forces VFX.

**Nouvelles fonctionnalités** :
- Champ `forceIndex` dans chaque `PoemZone`
- Référence directe au `VisualEffect` component
- Configuration des noms de propriétés VFX
- Mise à jour instantanée des booléens VFX

### 2. VFXPoemZoneForceBinder (optionnel)

Binder VFX qui peut être utilisé comme fallback ou pour synchroniser avec d'autres systèmes.

**Note** : Avec la mise à jour directe dans `PoemReaderPlaylist`, ce binder n'est plus strictement nécessaire mais reste disponible pour compatibilité.

## Configuration

### Étape 1 : Configuration du Visual Effect Graph

Dans votre FlowVFX, vous devez exposer 8 propriétés booléennes :

1. Ouvrez votre Visual Effect Graph
2. Dans le Blackboard, créez 8 propriétés de type **Boolean** :
   - `Force1_Enabled`
   - `Force2_Enabled`
   - `Force3_Enabled`
   - `Force4_Enabled`
   - `Force5_Enabled`
   - `Force6_Enabled`
   - `Force7_Enabled`
   - `Force8_Enabled`

3. Pour chaque force "Conform to Sphere" :
   - Ajoutez un bloc conditionnel ou utilisez le booléen pour multiplier l'intensité
   - Exemple : Multipliez la force par le booléen correspondant

### Étape 2 : Configuration de PoemReaderPlaylist

1. Sélectionnez votre GameObject `PoemReaderPlaylist`
2. Dans la section **VFX Integration** :
   - **Visual Effect** : Glissez-déposez le component VisualEffect de votre FlowVFX
   - **Force Property Names** : Vérifiez que les 8 noms correspondent aux propriétés dans votre VFX Graph (par défaut : Force1_Enabled, Force2_Enabled, etc.)

3. Pour chaque zone (0-7), configurez le champ **Force Index** :
   - Zone 1 → Force Index = 0 (active Force1_Enabled)
   - Zone 2 → Force Index = 1 (active Force2_Enabled)
   - Zone 3 → Force Index = 2 (active Force3_Enabled)
   - ... et ainsi de suite

**Note** : Vous pouvez assigner plusieurs zones au même force index si nécessaire.

### Étape 3 (Optionnelle) : Utiliser le VFXPoemZoneForceBinder

Si vous souhaitez utiliser le binder en plus ou à la place :

1. Ajoutez le component **VFX Property Binder** au GameObject contenant le Visual Effect
2. Ajoutez un nouveau binder : **Poem Zone Force Binder**
3. Configurez les 8 propriétés et assignez le `PoemReaderPlaylist` comme Target

## Comportement

### Activation instantanée

Lorsqu'une zone commence à lire un poème :
- Le booléen VFX correspondant est **immédiatement** mis à `true`
- La force "Conform to Sphere" associée s'active instantanément
- Un log de debug apparaît : `🌀 Force {X} (Force{X}_Enabled) activée`

### Désactivation instantanée

La force se désactive automatiquement et instantanément dans les cas suivants :
1. **Fin du poème** : Le poème arrive à sa dernière ligne
2. **Arrêt manuel** : L'utilisateur réappuie sur la touche de la zone
3. **Arrêt global** : Touche Escape ou Reset

## Exemples d'utilisation

### Exemple 1 : Une force par zone (configuration standard)

```
Zone 1 (Touche 1) → Force Index 0 → Force1_Enabled
Zone 2 (Touche 2) → Force Index 1 → Force2_Enabled
Zone 3 (Touche 3) → Force Index 2 → Force3_Enabled
...
Zone 8 (Touche 8) → Force Index 7 → Force8_Enabled
```

### Exemple 2 : Groupes de zones

```
Zones 1-4 → Force Index 0 → Force1_Enabled (zone gauche)
Zones 5-8 → Force Index 1 → Force2_Enabled (zone droite)
```

### Exemple 3 : Configuration dans le VFX Graph

**Méthode 1 : Activation/Désactivation complète**
```
Conform to Sphere Block
└─ Enable : Force1_Enabled (booléen exposé)
```

**Méthode 2 : Modulation de l'intensité**
```
Conform to Sphere Block
├─ Force Intensity : Multiply
│  ├─ Base Intensity (float)
│  └─ Force1_Enabled ? 1.0 : 0.0
```

## API Publique

### PoemReaderPlaylist.GetForceState(int forceIndex)

Retourne l'état actuel d'une force.

**Paramètres** :
- `forceIndex` : Index de la force (0-7)

**Retour** :
- `true` si la force est activée
- `false` si la force est désactivée ou si l'index est invalide

**Exemple** :
```csharp
bool isForce3Active = poemReaderPlaylist.GetForceState(2);
```

## Debug

Le système affiche des logs de debug si `Show Debug Logs` est activé dans PoemReaderPlaylist :

- `🌀 Force {X} activée` : Une force vient d'être activée
- `🌀 Force {X} désactivée` : Une force vient d'être désactivée

## Troubleshooting

### Les forces ne s'activent pas

**Solution principale (recommandée)** :
1. Vérifiez que le champ `Visual Effect` dans `PoemReaderPlaylist` est bien assigné
2. Vérifiez que les noms de propriétés dans `Force Property Names` correspondent exactement aux propriétés du VFX Graph (sensible à la casse)
3. Vérifiez les logs de debug : `⚠ Propriété VFX '...' introuvable !`
4. Vérifiez que les propriétés sont bien exposées dans le VFX Graph (visible dans le Blackboard)

**Alternative avec le Binder** :
1. Vérifiez que le `VFXPoemZoneForceBinder` est bien ajouté au GameObject du VFX
2. Vérifiez que le champ `Target` pointe vers le bon `PoemReaderPlaylist`
3. Vérifiez que les 8 propriétés booléennes existent dans le VFX Graph

### Mauvaise force activée

- Vérifiez le champ `Force Index` de chaque zone
- Les indices vont de 0 à 7 (pas de 1 à 8)
- Force Index 0 → Force1_Enabled
- Force Index 1 → Force2_Enabled, etc.

### Force reste active après la fin du poème

- Activez `Show Debug Logs` et vérifiez que la désactivation est appelée
- Vérifiez que le component `Visual Effect` est actif
- Vérifiez qu'aucune autre script ne modifie les mêmes propriétés

### Warning "Visual Effect non assigné"

Ce message apparaît si vous n'avez pas assigné le Visual Effect dans l'inspecteur. Les forces continueront à fonctionner via le binder si configuré, mais la mise à jour sera différée d'une frame.

## Avantages de la mise à jour directe

- **Instantané** : Pas d'attente jusqu'à la prochaine frame
- **Fiable** : Pas de dépendance au cycle Update() du binder
- **Simple** : Une seule référence à configurer
- **Debuggable** : Messages de log précis avec nom de propriété

Le système VFXPoemZoneForceBinder reste disponible comme alternative ou backup.
