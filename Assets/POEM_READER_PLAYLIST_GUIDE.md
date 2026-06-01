# 📖 Guide : Lecteur de Poèmes avec Playlist

## 🎯 Concept

**PoemReaderPlaylist** : Système de **playlist de poèmes**
- ✅ **8 zones** de texte indépendantes
- ✅ Chaque zone a **une touche** dédiée
- ✅ Chaque zone a une **playlist de poèmes**
- ✅ Réappuyer sur la touche → **poème suivant**

---

## 📐 Structure

```
Zone 1 (Touche 1)              Zone 2 (Touche 2)
├─ Poème A                     ├─ Poème X
├─ Poème B                     ├─ Poème Y
└─ Poème C                     └─ Poème Z

Appui sur 1:                   Appui sur 2:
  1er appui → Poème A            1er appui → Poème X
  2e appui  → Poème B            2e appui  → Poème Y
  3e appui  → Poème C            3e appui  → Poème Z
  4e appui  → Poème A (boucle)   4e appui  → Poème X (boucle)
```

---

## 🚀 Setup rapide

### Étape 1 : Créer les zones de texte UI

Pour chaque zone (8 maximum) :

1. **Hierarchy** → **UI** → **Text - TextMeshPro**
2. Renommez : **"Zone 1 Text"**, **"Zone 2 Text"**, etc.
3. Positionnez-les dans votre Canvas

**Exemple de layout** :
```
┌──────────────┬──────────────┐
│   Zone 1     │   Zone 2     │
│  (Touche 1)  │  (Touche 2)  │
├──────────────┼──────────────┤
│   Zone 3     │   Zone 4     │
│  (Touche 3)  │  (Touche 4)  │
└──────────────┴──────────────┘
```

### Étape 2 : Créer le lecteur

1. **GameObject** → **Create Empty** → **"Poem Reader"**
2. **Add Component** → **Poem Reader Playlist**

### Étape 3 : Configuration

Dans l'Inspector du **Poem Reader Playlist** :

#### **Zones Configuration** → **Zones** → Size : `8`

#### Pour chaque zone (ex: Element 0 = Zone 1) :

**1. Input :**
- **Trigger Key** : `Digit1` (touche 1)

**2. UI Reference :**
- **Text Display** : Glissez **"Zone 1 Text"**

**3. Poems Playlist :**
- **Poems** → Size : `3` (nombre de poèmes dans cette zone)

**Element 0 (Poème A) :**
```
Lines → Size: 3
[0] "Les sanglots longs"
[1] "Des violons"
[2] "De l'automne"
```

**Element 1 (Poème B) :**
```
Lines → Size: 3
[0] "Blessent mon coeur"
[1] "D'une langueur"
[2] "Monotone"
```

**Element 2 (Poème C) :**
```
Lines → Size: 2
[0] "Et je m'en vais"
[1] "Au vent mauvais"
```

---

## 🎮 Utilisation

### En jeu :

1. **Appuyez sur 1** → Poème A s'affiche dans Zone 1
2. **Réappuyez sur 1** → Poème A s'arrête, Poème B démarre
3. **Réappuyez sur 1** → Poème B s'arrête, Poème C démarre
4. **Réappuyez sur 1** → Poème C s'arrête, retour au Poème A (boucle)

### Contrôles :
- **Touche 1-8** : Cycle entre les poèmes de chaque zone
- **Escape** : Arrête toutes les zones

---

## 🎬 Comportement détaillé

### Appuyer sur la touche pendant la lecture :
```
[Poème A en cours d'écriture...]
	 ↓
[Appui sur la touche]
	 ↓
[Poème A arrêté immédiatement]
	 ↓
[Poème B démarre]
```

### Fin automatique d'un poème :
```
[Dernière ligne s'affiche]
	 ↓
[Pause de 5 secondes]
	 ↓
[Texte s'efface]
	 ↓
[Zone en attente - prête pour le prochain appui]
```

### Navigation :
- Les poèmes **bouclent** : après le dernier, retour au premier
- Chaque zone est **indépendante** : Zone 1 peut être au Poème B pendant que Zone 2 est au Poème A

---

## ⚙️ Configuration avancée

### Timing (partagé par toutes les zones)
- **Letter Delay** : `0.1s` (vitesse d'écriture)
- **Line Delay** : `5s` (pause entre phrases)

### Display Options
- **Clear Between Lines** : ✅ (efface chaque ligne)
- **Add Line Breaks** : ☐ (pas besoin si on efface)

---

## 💡 Exemples de configuration

### Configuration 1 : Verlaine complet (3 strophes)

**Zone 1 - Touche 1 - 3 poèmes (= 3 strophes) :**

Poème 0 :
```
"Les sanglots longs"
"Des violons"
"De l'automne"
```

Poème 1 :
```
"Blessent mon coeur"
"D'une langueur"
"Monotone"
```

Poème 2 :
```
"Tout suffocant"
"Et blême, quand"
"Sonne l'heure"
```

→ Chaque appui sur `1` affiche la strophe suivante !

---

### Configuration 2 : 4 zones, 4 poètes différents

**Zone 1 (Touche 1) - Verlaine** : 3 poèmes (strophes)  
**Zone 2 (Touche 2) - Prévert** : 2 poèmes  
**Zone 3 (Touche 3) - Haïkus** : 5 poèmes courts  
**Zone 4 (Touche 4) - Citations** : 10 phrases

→ Interface de performance poétique interactive !

---

## 🐛 Troubleshooting

### "Rien ne se passe quand j'appuie sur la touche"
✅ Vérifiez que **Text Display** est assigné  
✅ Vérifiez que **Poems** contient au moins 1 poème avec des lignes  
✅ Vérifiez la Console pour les erreurs

### "Le poème ne passe pas au suivant"
✅ Assurez-vous d'avoir plusieurs poèmes dans **Poems Playlist**  
✅ Réappuyez sur la touche (ne pas attendre la fin automatique)

### "Les mots sautent de ligne bizarrement"
✅ Augmentez la largeur du TextMeshPro  
✅ Réduisez la taille de la police

### "Le poème continue après avoir appuyé sur la touche"
✅ C'est normal : il arrête le poème actuel ET démarre le suivant  
✅ Pour juste arrêter : appuyez sur **Escape**

---

## 📊 API Publique

```csharp
PoemReaderPlaylist reader = GetComponent<PoemReaderPlaylist>();

// Contrôle
reader.StartZonePoem(0);    // Démarre le poème actuel de la zone 0
reader.StopZone(0);          // Arrête la zone 0
reader.NextPoem(0);          // Passe au poème suivant dans la zone 0
reader.ResetZone(0);         // Retour au premier poème de la zone 0

// Global
reader.StopAllZones();
reader.ResetAllZones();

// État
int currentPoem = reader.GetZoneCurrentPoem(0); // Index du poème actuel
bool isReading = reader.IsZoneReading(0);       // Zone en lecture ?
```

---

## 🎯 Checklist

✅ 1. Créé les TextMeshPro UI (1 à 8 zones)  
✅ 2. Créé le PoemReaderPlaylist  
✅ 3. Configuré chaque zone (touche, texte UI, playlist de poèmes)  
✅ 4. Rempli au moins 2 poèmes par zone  
✅ 5. Play + appuis successifs sur la touche  

**Résultat** : Playlist de poèmes qui se succèdent ! 🎉

---

## 🎨 Cas d'usage créatifs

### Performance interactive
- 8 zones = 8 interprètes
- Chacun contrôle sa zone avec sa touche
- Création de compositions textuelles collectives

### Installation d'art numérique
- Visiteurs appuient sur des boutons physiques (mappés aux touches)
- Chaque bouton révèle une nouvelle partie d'une histoire
- Navigation non-linéaire dans un récit

### Karaoké poétique
- Une zone centrale (grand texte)
- Plusieurs poèmes en playlist
- Le public contrôle la progression

Bon voyage dans vos playlists poétiques ! ✨
