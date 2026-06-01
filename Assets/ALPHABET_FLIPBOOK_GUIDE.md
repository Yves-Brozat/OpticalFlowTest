# Guide Rapide : Alphabet FlipBook pour VFX Graph

## 🎯 Solution au problème Texture2DArray

Le VFX Graph ne supporte pas directement les Texture2DArray dans MainTexture.
**Solution** : Utiliser un **FlipBook** (atlas de textures) !

---

## 📦 Partie 1 : Générer le FlipBook (2 minutes)

### Étape 1 : Créer le générateur
1. GameObject → Create Empty → Nommez : **"Alphabet FlipBook"**
2. Add Component → **Alphabet Flipbook Generator**

### Étape 2 : Configuration
Dans l'Inspector :

**Alphabet Settings :**
- **Alphabet** : `ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`

**Atlas Settings :**
- **Columns** : `6` (6 colonnes)
- **Rows** : `6` (6 lignes = 36 cases total)
- **Cell Size** : `128` (taille de chaque lettre)

**Font Settings :**
- **Font** : Laissez vide ou assignez une font
- **Font Size** : `100`
- **Font Style** : `Bold`
- **Text Color** : `Blanc`
- **Background Color** : `Transparent` (Alpha = 0)

### Étape 3 : Générer !
1. Clic droit sur le script → **"Generate Flipbook Atlas"**
2. Vérifiez la Console :
   ```
   ✅ Atlas généré : 36 caractères
   ✅ Texture sauvegardée : Assets/AlphabetFlipbook.png
   📊 Configuration FlipBook pour VFX Graph :
	  - Flip Book Size: X=6, Y=6
   ```

3. La texture est **automatiquement sauvegardée** dans `Assets/AlphabetFlipbook.png`

---

## 🎨 Partie 2 : Configuration VFX Graph (1 minute)

### Dans le VFX Graph :

#### 1. **Ajouter la texture**
   - Dans le **Blackboard** : `+` → **Property** → **Texture 2D**
   - Nommez : `AlphabetFlipbook`
   - Assignez `AlphabetFlipbook.png`
   - Cochez **"Exposed"**

#### 2. **Dans Output Particle Quad** :
   - **Main Texture** : Connectez `AlphabetFlipbook` ✅ (ça marche maintenant !)
   - **Flip Book Size** : `X = 6`, `Y = 6`
   - **Flip Book Blend** : ☐ (décoché pour textures nettes)

#### 3. **Dans Initialize Particle** :
   - Ajoutez : **Set Texture Index**
   - Mode : **Random**
   - Min : `0`
   - Max : `36` (nombre de caractères)

#### 4. **Important : Activer FlipBook**
   - Dans **Output Particle Quad** :
   - Cherchez le slot **"Texture Index"**
   - Connectez la valeur de particule `texIndex` (ou le random que vous avez créé)

---

## 🎮 Résultat

Chaque particule affichera maintenant une **lettre aléatoire** de l'alphabet ! 🎉

---

## 🔧 Configuration détaillée VFX Graph

### Structure complète :

```
VFX Graph "Optical Flow Letters"

Blackboard:
└─ AlphabetFlipbook (Texture2D) → AlphabetFlipbook.png

Initialize Particle:
├─ Set Position Sequential
└─ Set Attribute: texIndex
   └─ Random(0, 36)

Output Particle Quad:
├─ Main Texture: AlphabetFlipbook
├─ Flip Book Size: X=6, Y=6
└─ Texture Index: texIndex
```

---

## ⚡ Variantes stylées

### Lettre change pendant la vie de la particule :

**Dans Update Particle :**
```
Set Attribute: texIndex
└─ floor(age * 10) % 36
```
(Change 10 fois par seconde)

### Lettre basée sur la vitesse :

**Dans Update Particle :**
```
Set Attribute: texIndex  
└─ floor(length(velocity) * 5) % 36
```
(Lettre change selon la vitesse)

### Lettre basée sur la position :

```
Set Attribute: texIndex
└─ floor((position.x + position.y) * 2) % 36
```

---

## 🎨 Personnalisation

### Grille différente :

**Plus de caractères** (ex: 64) :
- Columns : `8`
- Rows : `8`
- Cell Size : `128`
- Alphabet : ajoutez minuscules + symboles

**Lettres plus grandes** :
- Cell Size : `256` (meilleure qualité)
- Atlas final : 1536x1536 pixels

**Performance max** :
- Cell Size : `64`
- Columns : `8`
- Rows : `5` (40 cases)

---

## 🐛 Troubleshooting

### "Toutes les particules ont la même lettre"
✅ Vérifiez que **texIndex** est `Random` dans Initialize Particle

### "Lettres floues/pixelisées"
✅ Augmentez **Cell Size** (256 ou 512)
✅ Vérifiez que la texture importée a **Max Size** = 4096

### "Texture non connectée à MainTexture"
✅ Utilisez **Texture 2D** (pas Texture 2D Array)
✅ La texture doit être un FlipBook (grille de lettres)

### "Lettres ne s'affichent pas"
✅ Vérifiez **Flip Book Size** = vos Columns et Rows
✅ Vérifiez **Texture Index** est connecté dans Output
✅ Text Color = Blanc, Background = Transparent

### "Mauvaise lettre affichée"
✅ Vérifiez que Flip Book Size correspond aux vraies dimensions
✅ Max de texIndex doit être <= Columns × Rows

---

## 📊 Formats recommandés selon usage

### Standard (Recommandé) :
```
Columns: 6
Rows: 6
Cell Size: 128
Alphabet: "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
Atlas final: 768x768 pixels
```

### Haute qualité :
```
Columns: 8
Rows: 8  
Cell Size: 256
Alphabet: Lettres + minuscules + symboles
Atlas final: 2048x2048 pixels
```

### Performance :
```
Columns: 8
Rows: 4
Cell Size: 64
Alphabet: "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
Atlas final: 512x256 pixels
```

---

## 🎯 Checklist finale

✅ 1. Généré FlipBook : `AlphabetFlipbook.png`  
✅ 2. VFX Blackboard : Texture2D `AlphabetFlipbook`  
✅ 3. Output Quad : Main Texture connecté  
✅ 4. Output Quad : Flip Book Size = 6x6  
✅ 5. Initialize : Set texIndex = Random(0, 36)  
✅ 6. Output Quad : Texture Index = texIndex  

**Résultat** : Particules avec lettres aléatoires qui suivent l'optical flow ! 🚀

---

## 💡 Astuce Pro

Pour un effet **Matrix/Glitch** :
1. Utilisez une font **monospace** (Courier, VT323)
2. Text Color : `Vert fluo` (#00FF00)
3. Dans Update : Changez texIndex toutes les 0.1s
4. Ajoutez du **motion blur** dans Output

C'est magnifique ! ✨
