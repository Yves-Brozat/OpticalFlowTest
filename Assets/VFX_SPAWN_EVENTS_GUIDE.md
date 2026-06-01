# 🎯 Guide : Configuration des Événements de Spawn VFX Graph

## 📋 Vue d'ensemble

Pour que le **PoemReader** puisse spawner des particules à la demande avec des lettres spécifiques, le VFX Graph doit :
1. Écouter un événement personnalisé `OnSpawn`
2. Recevoir des attributs (position, lettre)
3. Créer une particule avec ces données

---

## 🚀 Configuration pas-à-pas

### Étape 1 : Ouvrir votre VFX Graph

1. Double-cliquez sur votre **VFX Graph** (ex: `Visualizer.vfx`)
2. Vous devriez voir les contextes : **Spawn**, **Initialize**, **Update**, **Output**

---

### Étape 2 : Configurer le Spawn Context

#### Option A : Modifier le Spawn existant

Si vous avez déjà un **Spawn Context** :

1. **Cliquez sur le bloc "Spawn"** (titre orange)
2. Dans l'Inspector (à droite), trouvez **"Spawn Mode"**
3. Changez-le en : **Custom Spawn Event**

#### Option B : Créer un nouveau Spawn Context

Si vous n'avez pas de Spawn ou voulez en ajouter un :

1. **Clic droit** dans le graphe → **Create Node** → **Context** → **Spawn**
2. Connectez-le à votre **Initialize** existant
3. Dans l'Inspector, changez **Spawn Mode** en **Custom Spawn Event**

---

### Étape 3 : Nommer l'événement

1. Avec le **Spawn Context** sélectionné
2. Dans l'Inspector, trouvez **"Event Name"**
3. Tapez exactement : `OnSpawn`

⚠️ **Important** : Le nom doit être **exactement** `OnSpawn` (sensible à la casse) !

---

### Étape 4 : Ajouter les attributs de spawn

Les attributs permettent de passer des données du script C# au VFX Graph.

#### 4.1 Ajouter l'attribut Position

1. Dans le **Spawn Context**, cliquez sur le **petit (+)** en haut à droite du bloc
2. Cherchez : **"Set Spawn Event Attribute"** → **Position** (ou **Set Position**)
3. Dans l'Inspector de ce bloc :
   - **Source** : `Custom`
   - **Attribute** : `position`
   - **Type** : `Vector3`

Ou plus simplement :

1. Cliquez sur le **Spawn Context**
2. Dans l'Inspector, section **"Spawn Attributes"**
3. Cliquez sur **"+"**
4. Ajoutez : `position` (Vector3)

#### 4.2 Ajouter l'attribut texIndex

De la même manière :

1. Dans le **Spawn Context**, ajoutez un autre attribut
2. **Attribute** : `texIndex`
3. **Type** : `Float`

---

### Étape 5 : Utiliser les attributs dans Initialize

Maintenant que les attributs sont définis, utilisez-les dans **Initialize Particle** :

#### 5.1 Position

1. Dans le bloc **Initialize Particle**
2. Ajoutez : **Set Position** (clic droit → **Create Block** → **Set Position**)
3. Dans le bloc Set Position :
   - **Compose** : `Position (Position)`
   - **Source** : Connectez depuis l'attribut `position` du Spawn

Plus simple :

1. Dans **Initialize Particle**
2. Ajoutez un bloc **Inherit Source Position**

#### 5.2 Texture Index

1. Dans **Initialize Particle**
2. Ajoutez : **Set Texture Index**
3. Dans le bloc :
   - **Source** : `Custom`
   - Créez un nœud **Get Attribute: texIndex** (cliquez dans le graphe → **Create Node** → **Attribute** → **Get Attribute** → tapez `texIndex`)
   - Connectez `texIndex` à l'entrée du **Set Texture Index**

---

### Étape 6 : Configuration de l'Output

Votre **Output Particle Quad** doit être configuré pour le flipbook :

1. **Main Texture** : Assignez `AlphabetFlipbook.png`
2. **Flip Book Size** : `X = 6`, `Y = 6` (selon votre atlas)
3. **Texture Index** : Connectez l'attribut `texIndex` (il devrait être automatiquement lié si vous l'avez défini dans Initialize)

---

## 📊 Structure finale du VFX Graph

Voici à quoi devrait ressembler votre graphe :

```
┌─────────────────────────────────────────┐
│ SPAWN CONTEXT                           │
│ Mode: Custom Spawn Event                │
│ Event Name: OnSpawn                     │
│                                         │
│ Spawn Attributes:                       │
│   • position (Vector3)                  │
│   • texIndex (Float)                    │
│                                         │
│ Blocks:                                 │
│   • Set Spawn Event Attribute (position)│
│   • Set Spawn Event Attribute (texIndex)│
└─────────────────────────────────────────┘
			↓
┌─────────────────────────────────────────┐
│ INITIALIZE PARTICLE                     │
│                                         │
│ Blocks:                                 │
│   • Inherit Source Position             │
│   • Set Attribute: texIndex             │
│     └─ Get Attribute: texIndex (spawn) │
│   • Set Lifetime: 10                    │
│   • Set Size: 1                         │
│   • Set Color: White                    │
└─────────────────────────────────────────┘
			↓
┌─────────────────────────────────────────┐
│ UPDATE PARTICLE (Optionnel)             │
│                                         │
│ Blocks:                                 │
│   • Gravity (optionnel)                 │
│   • Turbulence (optionnel)              │
│   • Conform to Flow (optionnel)         │
└─────────────────────────────────────────┘
			↓
┌─────────────────────────────────────────┐
│ OUTPUT PARTICLE QUAD                    │
│                                         │
│ Settings:                               │
│   • Main Texture: AlphabetFlipbook      │
│   • Flip Book Size: X=6, Y=6            │
│   • Use Flip Book: ✓                    │
│                                         │
│ Blocks:                                 │
│   • Set Texture Index: texIndex         │
└─────────────────────────────────────────┘
```

---

## 🔧 Configuration détaillée étape par étape

### Configuration du Spawn Context (images)

Voici les paramètres exacts à régler :

#### Dans le Spawn Context (Inspector) :
```
╔═══════════════════════════════════════╗
║ Spawn                                 ║
╠═══════════════════════════════════════╣
║ Mode:                                 ║
║   [Custom Spawn Event ▼]              ║
║                                       ║
║ Event Name:                           ║
║   [OnSpawn                    ]       ║
║                                       ║
║ Rate:                                 ║
║   [1                          ]       ║
║                                       ║
║ Spawn Attributes:                     ║
║   [+] position    (Vector3)           ║
║   [+] texIndex    (Float)             ║
╚═══════════════════════════════════════╝
```

---

### Configuration de Initialize Particle

Dans le bloc **Initialize Particle**, ajoutez ces blocs dans cet ordre :

```
1. Set Position
   └─ Source: Inherit from Spawn
   └─ Attribute: position

2. Set Attribute: texIndex
   └─ Value: Get Attribute (texIndex) from spawn

3. Set Lifetime
   └─ Mode: Random
   └─ Min: 8
   └─ Max: 12

4. Set Size
   └─ Size: 0.5

5. Set Color
   └─ Color: White (1, 1, 1, 1)
```

---

### Configuration de Output Particle Quad

```
╔═══════════════════════════════════════╗
║ Output Particle Quad                  ║
╠═══════════════════════════════════════╣
║ Main Texture:                         ║
║   [AlphabetFlipbook          ] 📁     ║
║                                       ║
║ Flip Book:                            ║
║   ☑ Use Flip Book                     ║
║                                       ║
║ Flip Book Size:                       ║
║   X: [6 ]  Y: [6 ]                    ║
║                                       ║
║ Flip Book Blend:                      ║
║   ☐ Blend                             ║
║                                       ║
║ Texture Index:                        ║
║   Source: Attribute (texIndex)        ║
╚═══════════════════════════════════════╝
```

---

## 🧪 Test de l'événement

### Test 1 : Dans l'Editor

1. Sélectionnez votre **GameObject** avec le VFX
2. Dans l'Inspector du **Visual Effect** component
3. Section **"Custom Events"**, vous devriez voir : `OnSpawn`
4. Cliquez sur **"Send Event"** → `OnSpawn`
5. Une particule devrait apparaître !

### Test 2 : Via script de test

Créez un petit script de test :

```csharp
using UnityEngine;
using UnityEngine.VFX;

public class VFXSpawnTest : MonoBehaviour
{
	[SerializeField] VisualEffect _vfx;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			VFXEventAttribute attr = _vfx.CreateVFXEventAttribute();
			attr.SetVector3("position", new Vector3(0, 0, 0));
			attr.SetFloat("texIndex", Random.Range(0, 36));
			_vfx.SendEvent("OnSpawn", attr);

			Debug.Log("Particule spawnée à (0,0,0)");
		}
	}
}
```

Attachez ce script, lancez le jeu, appuyez sur **Space** → une particule avec une lettre aléatoire apparaît !

---

## 🐛 Troubleshooting

### "L'événement OnSpawn n'apparaît pas dans l'Inspector"
✅ Vérifiez que **Spawn Mode** = `Custom Spawn Event`  
✅ Sauvegardez le VFX Graph (Ctrl+S)  
✅ Recompilez le VFX Graph (clic droit → **Recompile**)

### "Les particules apparaissent mais toutes avec la même lettre"
✅ Vérifiez que `texIndex` est bien défini dans le **Spawn Context**  
✅ Vérifiez que `Set Attribute: texIndex` est dans **Initialize Particle**  
✅ Dans Output, vérifiez que **Texture Index** pointe vers l'attribut `texIndex`

### "Les particules apparaissent au mauvais endroit"
✅ Vérifiez que **Set Position** dans Initialize utilise l'attribut `position`  
✅ Testez avec une position fixe d'abord (ex: `Vector3(0, 0, 0)`)

### "Erreur : VFXEventAttribute null"
✅ Le VFX Graph n'est pas correctement configuré  
✅ Les attributs `position` et `texIndex` ne sont pas exposés dans le Spawn Context

### "Les lettres ne s'affichent pas correctement"
✅ Vérifiez que **Flip Book Size** correspond à votre atlas (6x6)  
✅ Vérifiez que `texIndex` est un entier entre 0 et 35  
✅ Vérifiez que **AlphabetFlipbook** est bien assigné dans Main Texture

---

## 📝 Checklist finale

Avant de tester avec le PoemReader :

✅ **Spawn Context** en mode `Custom Spawn Event`  
✅ Nom de l'événement : `OnSpawn`  
✅ Attribut `position` (Vector3) défini  
✅ Attribut `texIndex` (Float) défini  
✅ **Initialize Particle** hérite de `position`  
✅ **Initialize Particle** utilise `texIndex`  
✅ **Output Quad** a `AlphabetFlipbook` assigné  
✅ **Output Quad** a `Flip Book Size = 6x6`  
✅ **Output Quad** utilise l'attribut `texIndex`  
✅ Test manuel fonctionne (Send Event dans l'Inspector)  

---

## 🎯 Prochaine étape

Une fois que votre VFX Graph est configuré :

1. **Assignez-le** au **PoemReader** dans l'Inspector
2. **Assignez** aussi l'**AlphabetFlipbookGenerator**
3. Remplissez **Poem Lines**
4. **Play** et appuyez sur `1` !

Le poème devrait apparaître lettre par lettre ! ✨

---

## 💡 Astuces avancées

### Ajouter d'autres attributs

Vous pouvez ajouter d'autres attributs pour plus de contrôle :

```
• color (Vector3) : Couleur de la lettre
• size (Float) : Taille de la lettre
• velocity (Vector3) : Vitesse initiale
• lifetime (Float) : Durée de vie
```

Dans le PoemReader, modifiez `SpawnLetter()` :

```csharp
eventAttribute.SetVector3("color", new Vector3(1, 0, 0)); // Rouge
eventAttribute.SetFloat("size", 2.0f); // Taille double
```

### Combiner avec l'Optical Flow

Dans **Update Particle**, ajoutez :

```
• Sample Texture2D (OpticalFlow)
• Add Velocity (flowSample.xy * strength)
```

Les lettres "nageront" dans le flux optique ! 🌊

---

Besoin d'aide pour une étape spécifique ? 🚀
