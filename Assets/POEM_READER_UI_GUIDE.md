# 📖 Guide Simple : Lecteur de Poèmes UI

## 🎯 Solution simple avec TextMeshPro UI

Affiche un poème lettre par lettre dans un texte UI. **Beaucoup plus simple** que la version VFX !

---

## 🚀 Setup ultra-rapide (1 minute)

### Étape 1 : Créer le texte UI

1. **Clic droit** dans la Hierarchy → **UI** → **Text - TextMeshPro**
2. Si Unity demande d'importer TMP Essentials → Cliquez **"Import TMP Essentials"**
3. Renommez le TextMeshPro : **"Poem Text"**

### Étape 2 : Positionner et styliser le texte

Sélectionnez **"Poem Text"** dans la Hierarchy :

#### **Rect Transform** :
- **Anchor Preset** : Cliquez sur le carré en haut à gauche → **Stretch** (Alt+clic pour position ET taille)
- **Left/Right/Top/Bottom** : `50` (marges)

#### **TextMeshPro - Text (UI)** :
- **Font Size** : `48`
- **Color** : Blanc (ou votre choix)
- **Alignment** : Centré horizontalement, haut verticalement
- **Wrapping** : ☑ Enabled
- **Overflow** : Truncate

### Étape 3 : Créer le lecteur

1. **GameObject** → **Create Empty** → Renommez : **"Poem Reader"**
2. **Add Component** → **Poem Reader UI**

### Étape 4 : Connecter

Dans l'Inspector du **Poem Reader UI** :

#### **Input** :
- **Start Key** : `Digit1` (touche 1)

#### **Poem Content** :
- **Poem Lines** → Size : `6`
- Remplissez avec votre poème :
  ```
  [0] Les sanglots longs
  [1] Des violons
  [2] De l'automne
  [3] Blessent mon coeur
  [4] D'une langueur
  [5] Monotone
  ```

#### **Timing** :
- **Letter Delay** : `0.1` (vitesse d'écriture)
- **Line Delay** : `5` (pause entre phrases)

#### **UI References** :
- **Text Display** : Glissez **"Poem Text"** depuis la Hierarchy

#### **Display Options** :
- **Clear Between Lines** : ☐ (décoché = tout le poème s'accumule)
- **Add Line Breaks** : ☑ (coché = saut de ligne après chaque phrase)

---

## 🎮 Utilisation

1. **Play**
2. Appuyez sur **`1`**
3. Le poème apparaît lettre par lettre ! ✨

**Arrêter** : Appuyez sur **`Escape`**

---

## ⚙️ Options de configuration

### **Clear Between Lines**

**☐ Décoché (recommandé)** :
```
Les sanglots longs
Des violons
De l'automne
```
→ Tout le poème s'affiche progressivement

**☑ Coché** :
```
De l'automne
```
→ Chaque ligne efface la précédente (effet "téléscripteur")

### **Add Line Breaks**

**☑ Coché (recommandé)** :
```
Les sanglots longs
Des violons
De l'automne
```

**☐ Décoché** :
```
Les sanglots longsDes violonsDe l'automne
```

---

## 🎨 Personnalisation du texte

### Style élégant (poésie)
```
Font: Liberation Sans (ou votre choix)
Font Size: 48
Color: Blanc
Alignment: Centré
Letter Spacing: 2
Line Spacing: 1.2
```

### Style machine à écrire
```
Font: Courier New (monospace)
Font Size: 36
Color: Vert fluo (#00FF00)
Alignment: Gauche
Letter Delay: 0.05 (rapide)
```

### Style dramatique
```
Font: Times New Roman
Font Size: 64
Color: Rouge (#FF0000)
Alignment: Centré
Letter Spacing: 5
Letter Delay: 0.2 (lent)
Line Delay: 8 (longues pauses)
```

---

## 🎭 Exemples avancés

### Poème avec fond semi-transparent

1. Sélectionnez le **Canvas** dans la Hierarchy
2. **Add Component** → **Image**
3. **Color** : Noir avec Alpha = 150

### Ajouter une ombre portée

Sur le **Poem Text** :
1. **Add Component** → **Shadow**
2. **Effect Distance** : `(3, -3)`
3. **Color** : Noir semi-transparent

### Ajouter un contour

Sur le **Poem Text** :
1. **Add Component** → **Outline**
2. **Effect Distance** : `(2, 2)`
3. **Color** : Noir

---

## 🐛 Troubleshooting

### "Rien ne se passe quand j'appuie sur 1"
✅ Vérifiez que **Text Display** est assigné dans l'Inspector  
✅ Vérifiez la Console pour les logs (si **Show Debug Logs** est coché)  
✅ Vérifiez que **Start Key** = `Digit1`

### "Le texte n'apparaît pas à l'écran"
✅ Vérifiez que le **Canvas** est visible dans la scène  
✅ Vérifiez la couleur du texte (pas noir sur noir !)  
✅ Vérifiez la taille de la police (pas trop petite)

### "Le texte est coupé"
✅ Augmentez la taille du **Rect Transform** du texte  
✅ Changez **Overflow** en `Overflow` au lieu de `Truncate`

### "Les lettres apparaissent trop vite/lent"
✅ Ajustez **Letter Delay** dans l'Inspector (0.1 = vitesse normale)

---

## 💡 Astuces

### Plusieurs poèmes

Créez plusieurs **Poem Reader UI** avec des touches différentes :
- Poème 1 : Touche `Digit1`
- Poème 2 : Touche `Digit2`
- Poème 3 : Touche `Digit3`

### Fade in/out

Ajoutez un **CanvasGroup** sur le texte et contrôlez l'alpha :

```csharp
CanvasGroup canvasGroup = textDisplay.GetComponent<CanvasGroup>();
// Fade in
StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, 2f));
```

### Poème aléatoire

Mélangez les lignes avant de commencer :

```csharp
// Dans Start() ou avant StartPoem()
System.Random rng = new System.Random();
_poemLines = _poemLines.OrderBy(x => rng.Next()).ToArray();
```

---

## 📊 API Publique

```csharp
PoemReaderUI reader = GetComponent<PoemReaderUI>();

// Contrôle
reader.StartPoem();
reader.StopPoem();
reader.ClearText();
reader.ResetReader();

// État
bool isReading = reader.IsReading;
int currentLine = reader.CurrentLine;
float progress = reader.Progress; // 0.0 à 1.0
string currentText = reader.CurrentText;
```

---

## 🎯 Checklist finale

✅ 1. TextMeshPro UI créé dans la scène  
✅ 2. TextMeshPro stylisé (taille, couleur, alignement)  
✅ 3. Poem Reader UI créé  
✅ 4. Text Display assigné  
✅ 5. Poem Lines remplies  
✅ 6. Play + touche `1`  

**Résultat** : Poème qui s'écrit lettre par lettre ! 🎉

---

## 🔥 Intégration avec Spout

Pour exporter le rendu vers Resolume :

1. Créez une **Camera** qui rend le Canvas
2. Assignez un **Render Texture** à la caméra
3. Utilisez **SpoutRenderTextureUpdater** sur ce Render Texture

Le poème sera streamé en temps réel ! 🚀

---

Beaucoup plus simple que la version VFX, non ? 😊
