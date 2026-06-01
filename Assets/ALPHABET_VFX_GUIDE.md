# Guide : Alphabet Texture Generator pour VFX Graph

## 🎯 Objectif
Remplacer les sprites de particules par des lettres aléatoires de l'alphabet.

---

## 📦 Partie 1 : Générer les textures d'alphabet

### Étape 1 : Créer le générateur
1. Créez un GameObject : `GameObject → Create Empty`
2. Nommez-le : **"Alphabet Generator"**
3. Ajoutez le composant : **`Alphabet Texture Generator Pro`**

### Étape 2 : Configuration
Dans l'Inspector :

**Alphabet Settings :**
- **Alphabet** : `ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`
- **Include Lowercase** : ☐ (décoché pour commencer)

**Texture Settings :**
- **Texture Size** : `256` (ou 512 pour meilleure qualité)
- **Font** : Drag une font (Arial, Roboto, etc.) ou laissez vide pour la font par défaut
- **Font Size** : `200`
- **Font Style** : `Bold`

**Colors :**
- **Text Color** : Blanc (pour particules visibles)
- **Background Color** : Transparent (Alpha = 0)
- **Anti Aliasing** : `1.0`

### Étape 3 : Générer
1. En mode Play ou Edit : Clic droit sur le script → **"Generate Alphabet Textures"**
2. Vérifiez la Console : `✅ Généré 36 textures`

### Étape 4 : Sauvegarder (Important !)
1. Clic droit sur le script → **"Save Texture Array to Asset"**
2. Une Texture2DArray sera créée dans `Assets/AlphabetTextures.asset`
3. Vous pouvez maintenant supprimer le GameObject générateur

---

## 🎨 Partie 2 : Utiliser dans VFX Graph

### Méthode A : Texture aléatoire par particule (Simple)

#### Dans VFX Graph :

**1. Ajouter la Texture2DArray**
   - Dans le **Blackboard** (panneau gauche)
   - Cliquez **"+"** → **Property** → **Texture 2D Array**
   - Nommez : `AlphabetTextures`
   - Assignez votre `AlphabetTextures.asset`
   - Cochez **"Exposed"**

**2. Dans Initialize Particle :**
   - Ajoutez : **Set Texture Index**
   - Mode : `Random`
   - Min : `0`
   - Max : `36` (nombre de caractères dans votre alphabet)

**3. Dans Output Particle (Quad) :**
   - **Main Texture** : Connectez `AlphabetTextures` (Texture2DArray)
   - Le VFX Graph utilisera automatiquement le Texture Index

**4. Shader :**
   - Material : Assurez-vous que le shader supporte Texture2DArray
   - Si problème : Utilisez un shader Unlit ou VFX Lit

---

### Méthode B : Changer de lettre pendant la vie de la particule (Avancé)

#### Dans VFX Graph :

**1. Dans Update Particle :**
   - Ajoutez : **Set Texture Index**
   - Mode : `Custom`
   - Formule : `floor(Random(Seed + totalTime * 10)) * 36`
   (Change de lettre environ 10 fois par seconde)

**2. Pour un changement moins fréquent :**
   - Formule : `floor(Age / 0.5) % 36`
   (Change toutes les 0.5 secondes)

---

### Méthode C : Index basé sur position/vélocité (Stylé)

#### Dans VFX Graph :

**1. Dans Initialize ou Update :**
   - Ajoutez : **Set Texture Index**
   - Formule : `(int)(length(velocity) * 10) % 36`
   (La lettre change selon la vitesse de la particule)

Ou basé sur la position :
   - Formule : `(int)(position.x * 5) % 36`

---

## 🎮 Partie 3 : Contrôle runtime

Pour changer l'alphabet dynamiquement depuis C#, créez un binder :

```csharp
using UnityEngine.VFX;

public class VFXAlphabetBinder : VFXBinderBase
{
	public AlphabetTextureGeneratorPro generator;

	public override bool IsValid(VisualEffect vfx)
		=> vfx.HasTexture("AlphabetTextures");

	public override void UpdateBinding(VisualEffect vfx)
	{
		if (generator.TextureArray != null)
			vfx.SetTexture("AlphabetTextures", generator.TextureArray);
	}
}
```

---

## 🎨 Customisation avancée

### Différentes fonts pour différents effets :

**Font futuriste** :
- Orbitron, Audiowide, Exo

**Font glitch** :
- VT323, Share Tech Mono

**Font manuscrite** :
- Permanent Marker, Indie Flower

### Alphabet personnalisé :

Dans le générateur, changez `Alphabet` par :
```
Symboles : !@#$%^&*()[]{}
Japonais : あいうえお
Emojis : ★☆♥♦♣♠
Code : {}()<>/\;:
```

### Taille de texture selon usage :

- **128x128** : Performance max, distance
- **256x256** : Standard (recommandé)
- **512x512** : Haute qualité, gros plans
- **1024x1024** : Très haute qualité (overkill)

---

## 🔧 Troubleshooting

### "Toutes les particules ont la même lettre"
- Vérifiez que **Set Texture Index** est dans **Initialize Particle**
- Mode doit être **Random** (pas Constant)

### "Lettres noires/invisibles"
- Vérifiez **Text Color** = Blanc
- Vérifiez **Background Color** = Transparent (Alpha = 0)
- Dans VFX Graph : Shader doit supporter la transparence

### "Texture2DArray n'apparaît pas dans VFX"
- Le type doit être **Texture 2D Array** (pas Texture 2D)
- Vérifiez que l'asset est bien sauvegardé

### "Qualité médiocre"
- Augmentez **Texture Size** (512 ou 1024)
- Augmentez **Font Size**
- Activez **Anti Aliasing** à 1.0

---

## 📋 Exemple configuration complète

```
GameObject : "Alphabet Generator"
└─ AlphabetTextureGeneratorPro
   ├─ Alphabet: "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
   ├─ Texture Size: 256
   ├─ Font Size: 200
   ├─ Font Style: Bold
   ├─ Text Color: White
   └─ Background: Transparent

VFX Graph:
├─ Blackboard: AlphabetTextures (Texture2DArray)
├─ Initialize Particle
│  └─ Set Texture Index: Random (0-26)
└─ Output Particle Quad
   └─ Main Texture: AlphabetTextures
```

---

## 🎯 Résultat

✅ Chaque particule affiche une lettre aléatoire  
✅ Les lettres peuvent changer pendant la vie de la particule  
✅ Utilisable avec Optical Flow pour un effet glitch/matrix stylé  
✅ Performance optimale (Texture2DArray)  

Profitez de votre effet de texte Matrix-style ! 🚀
