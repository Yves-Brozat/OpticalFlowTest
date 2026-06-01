# 📖 Guide : Lecteur de Poèmes Automatique

## 🎯 Vue d'ensemble

Le **PoemReader** permet d'afficher un poème lettre par lettre avec des particules VFX.
Chaque phrase apparaît progressivement, puis la suivante après un délai configurable.

---

## 🚀 Setup rapide (2 minutes)

### Étape 1 : Prérequis

Avant de commencer, assurez-vous d'avoir :
- ✅ **AlphabetFlipbookGenerator** configuré avec un atlas généré
- ✅ **VFX Graph** avec flipbook texture et support d'événements de spawn

### Étape 2 : Créer le lecteur

1. GameObject → Create Empty → **"Poem Reader"**
2. Add Component → **Poem Reader**

### Étape 3 : Configuration de base

Dans l'Inspector du **PoemReader** :

#### **Input**
- **Start Key** : `Digit1` (touche 1 du clavier)

Autres touches possibles :
- `Digit1`, `Digit2`, `Digit3`, etc. (touches numériques)
- `Space` (barre d'espace)
- `Enter` (touche Entrée)
- `A`, `B`, `C`, etc. (lettres)

#### **Poem Content**
- **Poem Lines** : Cliquez sur le `+` pour ajouter des lignes
  ```
  [0] Les sanglots longs
  [1] Des violons
  [2] De l'automne
  [3] Blessent mon coeur
  [4] D'une langueur
  [5] Monotone
  ```

#### **Timing**
- **Letter Delay** : `0.1` secondes (vitesse d'apparition des lettres)
- **Line Delay** : `5` secondes (pause entre les phrases)

#### **Particle Settings**
- **Start Position** : `(-5, 3)` (coin haut-gauche)
- **Letter Spacing** : `0.5` (espacement horizontal)
- **Line Spacing** : `1.0` (espacement vertical)

#### **VFX References**
- **Vfx** : Glissez votre VisualEffect depuis la scène
- **Alphabet Generator** : Glissez le GameObject avec AlphabetFlipbookGenerator

#### **Debug**
- **Show Debug Logs** : ☑ (pour voir les logs dans la Console)

---

## 🎨 Configuration VFX Graph

Pour que le PoemReader fonctionne, votre **VFX Graph** doit avoir :

### 1. Event de Spawn personnalisé

Dans le VFX Graph :
- Ajoutez un **Spawn Context** nommé `OnSpawn`
- Activez **"Custom Spawn Event"**

### 2. Attributs de spawn

Dans le **Spawn Context**, ajoutez ces attributs :

**Position** (Vector3) :
- Permet au PoemReader de placer chaque lettre

**texIndex** (Float) :
- Index de la texture flipbook pour choisir la lettre

### 3. Initialize Particle

```
Initialize Particle:
├─ Set Position (inherit from spawn)
└─ Set Texture Index (inherit from spawn "texIndex")
```

### 4. Output

```
Output Particle Quad:
├─ Main Texture: AlphabetFlipbook
├─ Flip Book Size: X=6, Y=6
└─ Texture Index: texIndex
```

---

## 🎮 Utilisation

### Méthode 1 : Via clavier

1. Lancez le jeu
2. Appuyez sur **`1`** (ou votre touche configurée)
3. Le poème démarre automatiquement !

> **Note** : Le projet utilise le **nouveau Input System**. Les touches disponibles sont :
> - `Digit1` à `Digit9` (touches numériques)
> - `A` à `Z` (lettres)
> - `Space`, `Enter`, `Escape`, etc.

### Méthode 2 : Via script

```csharp
PoemReader reader = GetComponent<PoemReader>();

// Démarrer
reader.StartPoem();

// Arrêter
reader.StopPoem();

// Réinitialiser
reader.ResetReader();

// État actuel
bool isReading = reader.IsReading;
int currentLine = reader.CurrentLine;
float progress = reader.Progress; // 0.0 à 1.0
```

### Méthode 3 : Via Inspector

Clic droit sur le composant **PoemReader** :
- **Start Poem** : Lance le poème
- **Stop Poem** : Arrête la lecture
- **Reset Reader** : Réinitialise

---

## ⚙️ Paramètres détaillés

### **Letter Delay** (0.01 → 0.5s)
- `0.05` : Très rapide (effet machine à écrire)
- `0.1` : Vitesse normale (recommandé)
- `0.3` : Lent et dramatique

### **Line Delay** (0.5 → 10s)
- `2s` : Enchaînement rapide
- `5s` : Pause contemplative (recommandé)
- `10s` : Pause très longue

### **Letter Spacing**
- `0.3` : Lettres serrées
- `0.5` : Espacement normal (recommandé)
- `1.0` : Lettres espacées

### **Line Spacing**
- `0.8` : Lignes rapprochées
- `1.0` : Espacement normal (recommandé)
- `2.0` : Lignes très espacées

---

## 🎭 Exemples de configurations

### Haiku japonais (court et zen)
```
Letter Delay: 0.15
Line Delay: 8.0
Letter Spacing: 0.6
Line Spacing: 1.5

Poem Lines:
[0] Un vieil étang
[1] Une grenouille qui plonge
[2] Le bruit de l'eau
```

### Prose rapide (dynamique)
```
Letter Delay: 0.05
Line Delay: 2.0
Letter Spacing: 0.4
Line Spacing: 0.8

Poem Lines:
[0] Dans le noir de la nuit
[1] Les étoiles brillent
[2] Et le vent souffle
[3] Entre les arbres
```

### Poème épique (dramatique)
```
Letter Delay: 0.2
Line Delay: 7.0
Letter Spacing: 0.7
Line Spacing: 2.0

Poem Lines:
[0] LIBERTÉ
[1] Sur mes cahiers d'écolier
[2] Sur mon pupitre et les arbres
[3] Sur le sable sur la neige
[4] J'écris ton nom
```

---

## 🐛 Troubleshooting

### "Aucune lettre n'apparaît"
✅ Vérifiez que **VFX** et **Alphabet Generator** sont assignés  
✅ Vérifiez que le VFX Graph a un événement `OnSpawn`  
✅ Vérifiez que les attributs `position` et `texIndex` sont exposés

### "Toutes les lettres apparaissent en même temps"
✅ Le problème est dans le VFX Graph : le Spawn doit être en **Custom Event**, pas **Constant Rate**

### "Lettres au mauvais endroit"
✅ Ajustez **Start Position** dans l'Inspector  
✅ Vérifiez que le VFX Graph utilise bien l'attribut `position` dans Initialize

### "Mauvaises lettres affichées"
✅ Vérifiez que l'alphabet du generator contient toutes les lettres du poème  
✅ Le PoemReader convertit automatiquement en majuscules

### "Console pleine d'erreurs VFXEventAttribute"
✅ Votre VFX Graph n'expose pas les bons attributs  
✅ Vérifiez que `position` (Vector3) et `texIndex` (Float) sont dans le Spawn Context

---

## 🎨 Intégration avec Optical Flow

Pour que les lettres suivent le flux optique :

### Dans le VFX Graph, Update Particle :

```
Update Particle:
├─ Sample Texture2D (Optical Flow)
│  └─ UV: position.xy / screenSize
├─ Add Velocity
│  └─ velocity += flowSample.xy * flowStrength
└─ Turbulence (optionnel pour du bruit)
```

Cela permettra aux lettres de "nager" dans le flux optique tout en restant lisibles ! 🌊

---

## 💡 Idées avancées

### Effet "Matrix" (lettres qui changent)
Dans le VFX Graph, **Update Particle** :
```
Set Attribute: texIndex
└─ Random(0, 36) every 0.1s
```

### Lettres qui s'estompent
Dans **Initialize Particle** :
```
Set Lifetime: 10s (durée avant disparition)
```

Dans **Output Particle Quad** :
```
Alpha over Life: Gradient (1 → 0)
```

### Multi-poèmes
Créez plusieurs **PoemReader** avec des touches différentes :
- Poème 1 : Touche `1`
- Poème 2 : Touche `2`
- Poème 3 : Touche `3`

---

## 📊 API Publique

```csharp
// Contrôle
public void StartPoem()
public void StopPoem()
public void ResetReader()

// État
public bool IsReading { get; }
public int CurrentLine { get; }
public int TotalLines { get; }
public float Progress { get; } // 0.0 à 1.0
```

---

## 🎯 Checklist finale

✅ 1. AlphabetFlipbookGenerator créé et atlas généré  
✅ 2. VFX Graph avec événement `OnSpawn` et attributs `position`/`texIndex`  
✅ 3. PoemReader créé et configuré  
✅ 4. VFX et Generator assignés dans l'Inspector  
✅ 5. Poème saisi dans **Poem Lines**  
✅ 6. Appuyer sur la touche `1` en jeu  

**Résultat** : Poème qui s'affiche lettre par lettre ! 🎉

---

## 🚀 Pour aller plus loin

- Combinez avec **Spout** pour exporter vers Resolume
- Ajoutez des effets de particules (trails, glow)
- Synchronisez avec de l'audio (beat detection)
- Créez des transitions entre poèmes

Bon voyage poétique ! ✨
