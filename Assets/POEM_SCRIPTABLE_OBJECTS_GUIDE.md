# 📚 Guide : Banque de Poèmes avec ScriptableObjects

## 🎯 Problème résolu

**Avant** : Réécrire les mêmes poèmes dans chaque zone = fastidieux et erreurs

**Maintenant** : Créer une **banque de poèmes** réutilisables partout !

---

## 🚀 Workflow en 3 étapes

### Étape 1 : Créer la banque de poèmes (une seule fois)

1. **Project** → Clic droit dans Assets → **Create** → **OpticalFlowTest** → **Poem**
2. Renommez : **"Verlaine_Strophe1"**
3. Dans l'Inspector :
   ```
   Title: "Chanson d'automne - Strophe 1"
   Author: "Paul Verlaine"
   Lines → Size: 3
	 [0] "Les sanglots longs"
	 [1] "Des violons"
	 [2] "De l'automne"
   Text Alignment: Left (ou Right, Center, Justified...)
   ```

4. Répétez pour créer tous vos poèmes :
   - `Verlaine_Strophe1.asset`
   - `Verlaine_Strophe2.asset`
   - `Verlaine_Strophe3.asset`
   - `Prevert_Page1.asset`
   - `Haiku_Basho.asset`
   - etc.

**Organisation recommandée** :
```
Assets/
└─ Poems/
   ├─ Verlaine/
   │  ├─ Strophe1.asset
   │  ├─ Strophe2.asset
   │  └─ Strophe3.asset
   ├─ Prevert/
   │  ├─ Page1.asset
   │  └─ Page2.asset
   └─ Haikus/
	  ├─ Basho1.asset
	  └─ Basho2.asset
```

---

### Étape 2 : Configurer les zones avec les poèmes

Dans le **Poem Reader Playlist** :

#### Zone 1 (Touche 1) - Verlaine complet :
```
Trigger Key: Digit1
Text Display: Zone1_Text
Poems → Size: 3
  [0] Verlaine_Strophe1 (glisser depuis Project)
  [1] Verlaine_Strophe2
  [2] Verlaine_Strophe3
```

#### Zone 2 (Touche 2) - Verlaine complet aussi :
```
Trigger Key: Digit2
Text Display: Zone2_Text
Poems → Size: 3
  [0] Verlaine_Strophe1 ← MÊME asset !
  [1] Verlaine_Strophe2 ← MÊME asset !
  [2] Verlaine_Strophe3 ← MÊME asset !
```

✅ **Pas de réécriture** : vous réutilisez les mêmes assets !

---

### Étape 3 : Utiliser

**Play** et appuyez sur `1` ou `2` : les deux zones affichent les mêmes poèmes ! 🎉

---

## 🎨 Avantages

### ✅ **Pas de duplication**
- Un poème = un fichier .asset
- Réutilisable dans N zones

### ✅ **Modification centralisée**
- Changez le texte dans le .asset
- **Toutes** les zones qui l'utilisent sont mises à jour automatiquement !

### ✅ **Organisation propre**
- Dossiers par auteur/thème
- Facile à retrouver
- Facile à versionner (Git)

### ✅ **Métadonnées**
- Title, Author, Category
- Utile pour l'UI ou les filtres

---

## 💡 Cas d'usage

### Cas 1 : Même playlist partout

**8 zones affichent le même contenu** (ex: installation avec 8 écrans) :

```
Zone 1-8 : TOUTES utilisent la même playlist
  [0] Verlaine_Strophe1
  [1] Verlaine_Strophe2
  [2] Verlaine_Strophe3
```

→ Un visiteur appuie sur n'importe quelle touche (1-8) = avance dans le poème

---

### Cas 2 : Playlists variées mais avec des poèmes communs

**Zone 1** : Verlaine complet
```
[0] Verlaine_S1
[1] Verlaine_S2
[2] Verlaine_S3
```

**Zone 2** : Mix Verlaine + Prévert
```
[0] Verlaine_S1  ← partagé
[1] Prevert_P1
[2] Verlaine_S2  ← partagé
```

**Zone 3** : Haïkus seulement
```
[0] Haiku_Basho1
[1] Haiku_Basho2
```

→ Composition flexible sans duplication !

---

### Cas 3 : Thèmes différents par zone

**Zone 1** : Poésie classique  
**Zone 2** : Poésie moderne  
**Zone 3** : Haïkus  
**Zone 4** : Slam  

Chaque zone a sa propre playlist, mais des poèmes peuvent être partagés entre thèmes.

---

## 🔧 Workflow de création rapide

### Créer 10 poèmes rapidement :

1. **Create** → **Poem** → Renommez `Poem_01`
2. Remplissez le contenu
3. **Ctrl+D** (dupliquer)
4. Renommez `Poem_02`
5. Modifiez le contenu
6. Répétez !

### Template de poème :

Créez un **"Template.asset"** vide et dupliquez-le pour chaque nouveau poème.

---

## 🐛 Troubleshooting

### "PoemAsset manquant" dans la Console
✅ Glissez un fichier .asset dans le slot vide de la playlist

### "Les changements ne s'appliquent pas"
✅ Assurez-vous de modifier le fichier **.asset** (pas une copie locale)  
✅ Sauvegardez le .asset (Ctrl+S)

### "Je ne trouve pas le menu Create → Poem"
✅ Vérifiez que `PoemAsset.cs` compile sans erreur  
✅ Rechargez Unity (Assets → Refresh ou Ctrl+R)

---

## 📊 Structure de données

### PoemAsset (ScriptableObject)
```
Title: "Chanson d'automne"
Author: "Paul Verlaine"
Lines: ["Les sanglots longs", "Des violons", ...]
Text Alignment: Left (ou Right, Center, Justified...)
Category: "Classique"
```

### PoemZone (dans PoemReaderPlaylist)
```
Trigger Key: Digit1
Text Display: (TextMeshPro UI)
Poems: [PoemAsset, PoemAsset, PoemAsset, ...]
```

---

## 🎯 Workflow complet : Exemple réel

### 1. Créer la banque (10 minutes)

```
Assets/Poems/
├─ Verlaine_Chanson_S1.asset
├─ Verlaine_Chanson_S2.asset
├─ Verlaine_Chanson_S3.asset
├─ Prevert_Page1.asset
├─ Prevert_Page2.asset
├─ Prevert_Page3.asset
├─ Haiku_Basho_Etang.asset
├─ Haiku_Issa_Neige.asset
└─ Rimbaud_Bateau_S1.asset
```

### 2. Configurer les 8 zones (5 minutes)

**Zones 1-4** : Verlaine complet (même playlist)  
**Zones 5-6** : Prévert complet (même playlist)  
**Zones 7-8** : Haïkus alternés  

### 3. Play et tester (immédiat)

- Touche 1-4 → Verlaine
- Touche 5-6 → Prévert
- Touche 7-8 → Haïkus

### 4. Modifier un poème (30 secondes)

Ouvrez `Verlaine_Chanson_S1.asset` → Changez une ligne → **Toutes les zones 1-4 sont mises à jour !**

---

## 🚀 Checklist

✅ 1. Créé les fichiers PoemAsset (.asset) dans Project  
✅ 2. Rempli Title, Author, Lines  
✅ 3. Glissé les .asset dans les playlists des zones  
✅ 4. Configuré Trigger Key et Text Display  
✅ 5. Play + test  

**Résultat** : Banque de poèmes réutilisable partout ! 🎉

---

## 💡 Astuces avancées

### Créer une bibliothèque de 100+ poèmes

Organisez par dossiers :
```
Poems/
├─ Classiques/
├─ Modernes/
├─ Haikus/
├─ Slam/
└─ Citations/
```

### Personnaliser l'alignement par poème

Chaque poème peut avoir son propre alignement :
- **Left** : Poésie classique, texte standard
- **Center** : Haïkus, citations courtes
- **Right** : Effets visuels, contraste
- **Justified** : Prose poétique, textes longs

L'alignement change automatiquement à chaque nouveau poème !

### Utiliser la Category pour filtrer

Dans un script avancé, vous pourriez filtrer :
```csharp
var haikus = allPoems.Where(p => p.category == "Haiku").ToArray();
```

### Exporter/Importer depuis un fichier texte

Créez un script Editor qui lit un .txt et génère automatiquement les .asset !

---

C'est exactement ce qu'il vous fallait ? 😊
