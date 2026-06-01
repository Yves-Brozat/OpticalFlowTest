# 📖 Guide : Lecteur Multi-Poèmes

## 🎯 Vue d'ensemble

**PoemReaderMulti** permet de gérer **8 poèmes indépendants** avec :
- ✅ Une touche dédiée par poème
- ✅ Un TextMeshPro UI dédié par poème
- ✅ Lecture simultanée possible

---

## 🚀 Setup rapide

### Étape 1 : Créer les UI textes

Pour chaque poème que vous voulez afficher :

1. **Clic droit** dans Hierarchy → **UI** → **Text - TextMeshPro**
2. Renommez : **"Poem Text 1"**, **"Poem Text 2"**, etc.
3. Positionnez-les dans votre scène (côte à côte, superposés, etc.)
4. Stylisez-les (taille, couleur, alignement)

**Exemple de layout** :
```
Canvas
├─ Poem Text 1 (coin haut-gauche)
├─ Poem Text 2 (coin haut-droit)
├─ Poem Text 3 (centre)
└─ Poem Text 4 (bas)
```

### Étape 2 : Créer le lecteur

1. **GameObject** → **Create Empty** → **"Poem Reader Multi"**
2. **Add Component** → **Poem Reader Multi**

### Étape 3 : Configuration

Dans l'Inspector du **Poem Reader Multi** :

#### **Poems Configuration** → **Poems** :
- **Size** : `8` (ou moins si vous avez moins de poèmes)

#### Pour chaque élément (ex: Element 0) :
1. **Start Key** : `Digit1` (touche 1)
2. **Lines** → Size : `3` (ou plus)
   ```
   [0] "Première ligne"
   [1] "Deuxième ligne"
   [2] "Troisième ligne"
   ```
3. **Text Display** : Glissez **"Poem Text 1"** depuis la Hierarchy

Répétez pour chaque poème avec des touches différentes :
- Element 0 : Touche `Digit1`
- Element 1 : Touche `Digit2`
- Element 2 : Touche `Digit3`
- etc.

---

## 🎮 Utilisation

### En jeu :
- **Touche 1** : Démarre le poème 1
- **Touche 2** : Démarre le poème 2
- **Touche 3** : Démarre le poème 3
- etc.
- **Escape** : Arrête tous les poèmes

### Plusieurs poèmes en même temps :
Oui ! Vous pouvez lancer plusieurs poèmes simultanément. Chacun s'affiche dans son propre TextMeshPro.

---

## ⚙️ Configuration avancée

### Timing (partagé par tous les poèmes)
- **Letter Delay** : `0.1s` (vitesse d'écriture)
- **Line Delay** : `5s` (pause entre phrases)

### Display Options
- **Clear Between Lines** : ✅ (efface chaque ligne avant la suivante)
- **Add Line Breaks** : ☐ (pas besoin si on efface)

---

## 💡 Exemples de setup

### Setup 1 : Poèmes côte à côte
```
┌─────────────┬─────────────┐
│  Poème 1    │  Poème 2    │
│  (Touche 1) │  (Touche 2) │
│             │             │
└─────────────┴─────────────┘
```

### Setup 2 : Un seul texte, plusieurs poèmes
```
┌─────────────────────────────┐
│      Texte central          │
│                             │
│   Touche 1-8 : différents   │
│   poèmes affichés ici       │
│                             │
└─────────────────────────────┘
```
→ Tous les poèmes utilisent le **même TextMeshPro**

### Setup 3 : 4 coins
```
Poème 1          Poème 2
(Touche 1)       (Touche 2)




Poème 3          Poème 4
(Touche 3)       (Touche 4)
```

---

## 🎨 Exemple de configuration complète

### Poème 1 (Verlaine)
```
Start Key: Digit1
Lines:
  [0] "Les sanglots longs"
  [1] "Des violons"
  [2] "De l'automne"
Text Display: Poem Text 1
```

### Poème 2 (Prévert)
```
Start Key: Digit2
Lines:
  [0] "Deux et deux quatre"
  [1] "Quatre et quatre huit"
  [2] "Huit et huit font seize"
Text Display: Poem Text 2
```

### Poème 3 (Haiku)
```
Start Key: Digit3
Lines:
  [0] "Un vieil étang"
  [1] "Une grenouille qui plonge"
  [2] "Le bruit de l'eau"
Text Display: Poem Text 3
```

---

## 🐛 Troubleshooting

### "Rien ne se passe quand j'appuie sur une touche"
✅ Vérifiez que **Text Display** est assigné pour ce poème  
✅ Vérifiez que **Lines** contient au moins une ligne  
✅ Vérifiez la Console pour les erreurs

### "Plusieurs poèmes s'affichent au même endroit"
✅ C'est normal si vous avez assigné le **même TextMeshPro** à plusieurs poèmes  
✅ Créez des TextMeshPro séparés si vous voulez les afficher en parallèle

### "Un poème ne s'arrête pas avec Escape"
✅ Vérifiez que le script est actif  
✅ Vérifiez la Console pour les erreurs

---

## 📊 API Publique

```csharp
PoemReaderMulti reader = GetComponent<PoemReaderMulti>();

// Contrôle individuel
reader.StartPoem(0); // Démarre poème 1
reader.StopPoem(0);  // Arrête poème 1
reader.ClearText(0); // Efface texte du poème 1

// Contrôle global
reader.StopAllPoems();
reader.ClearAllTexts();
reader.ResetAllReaders();

// État
bool anyReading = reader.IsAnyReading();
int readingCount = reader.GetReadingCount();
```

---

## 🎯 Checklist

✅ 1. Créé les TextMeshPro UI (1 à 8)  
✅ 2. Créé le PoemReaderMulti  
✅ 3. Configuré chaque poème (touche, lignes, texte)  
✅ 4. Play + touche numérique  

**Résultat** : Plusieurs poèmes contrôlables indépendamment ! 🎉

---

## 💡 Astuce pro

Pour un effet **chorégraphie de textes** :
1. Créez 4-8 textes dans différentes zones
2. Assignez des poèmes courts (2-3 lignes)
3. Lancez-les en cascade avec un délai
4. Effet visuel impressionnant !

Bon voyage poétique multiple ! ✨
