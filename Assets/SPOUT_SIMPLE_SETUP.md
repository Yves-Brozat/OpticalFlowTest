# Configuration Spout Simple

## Problème : Le flux vidéo freeze dans Resolume/Receiver

### Cause
1. Le SpoutSender natif ne met pas à jour automatiquement la texture qui change chaque frame
2. Unity réduit son framerate quand la fenêtre n'est pas au premier plan

### Solution : SpoutTextureUpdater + SpoutOptimization

## Configuration complète (3 minutes)

### Étape 0 : Activer l'exécution en arrière-plan ⚠️ IMPORTANT
1. Sur **n'importe quel GameObject** (Main Camera par exemple)
2. `Add Component → Spout Optimization`
3. Configuration automatique :
   - ✅ Run in background
   - ✅ Framerate stable (60 FPS)
   - ✅ VSync désactivé

**Sans cette étape, Spout freezera quand vous passerez à Resolume !**

### Étape 1 : Setup GameObject Spout
1. Créez un GameObject vide : `GameObject → Create Empty`
2. Nommez-le : **"Webcam Spout"**

### Étape 2 : Ajouter SpoutSender (natif)
1. Sur "Webcam Spout" : `Add Component → Spout Sender`
2. Configuration :
   - **Spout Name** : `OpticalFlow_Webcam` (ou votre nom)
   - **Capture Method** : `Texture`
   - **Source Texture** : Laissez vide (sera mis à jour dynamiquement)

### Étape 3 : Ajouter SpoutTextureUpdater
1. Sur le même GameObject : `Add Component → Spout Texture Updater`
2. Configuration :
   - **Image Source** : Drag votre GameObject ImageSource

### Étape 4 : Play !
1. Lancez Unity (Play)
2. Ouvrez Resolume
3. Sources → Spout → `OpticalFlow_Webcam`
4. Le flux devrait maintenant être **fluide** et **mis à jour** ! ✅

---

## Configuration avancée (Multiple Spouts)

### Pour envoyer plusieurs textures :

**1. Webcam (ImageSource)**
```
GameObject : "Webcam Spout"
├─ SpoutSender (Spout Name: "Webcam")
└─ SpoutTextureUpdater (Image Source: ImageSource)
```

**2. Optical Flow**
```
GameObject : "Flow Spout"  
├─ SpoutSender (Spout Name: "OpticalFlow")
└─ [Script custom pour mettre à jour avec OpticalFlowEstimator.AsRenderTexture]
```

**3. Datamosh**
```
GameObject : "Datamosh Spout"
├─ SpoutSender (Spout Name: "Datamosh", Source Texture: votre RenderTexture)
```
(Pas besoin d'updater si la RenderTexture est fixe dans l'Inspector)

---

## Pourquoi ça marche ?

- **SpoutSender** : Envoie la texture via Spout (natif KlakSpout)
- **SpoutTextureUpdater** : Force la mise à jour de `sourceTexture` à chaque frame dans `LateUpdate()`
- **LateUpdate** : S'exécute après que la webcam/flow soit mis à jour

---

## Troubleshooting

### "Toujours figé"
1. Vérifiez que les deux composants sont sur le **même GameObject**
2. Vérifiez que **Image Source** est bien assigné dans SpoutTextureUpdater
3. Vérifiez dans l'Inspector (mode Play) : **Source Texture** devrait changer

### "Texture noire"
1. Vérifiez que ImageSource fonctionne (texture visible dans Scene view)
2. Vérifiez que la webcam est autorisée dans Windows

### "Pas de sender visible dans Resolume"
1. Unity doit être en mode **Play**
2. Dans Resolume : Cliquez sur **Refresh**
3. Vérifiez le nom Spout (sensible à la casse)

### "Performance"
- Spout est très optimisé (GPU direct)
- Si lent : baissez la résolution de la webcam dans ImageSource

---

## Alternative : Sans SpoutTextureUpdater

Si vous voulez vraiment utiliser **uniquement SpoutSender** :

1. Dans SpoutSender :
   - **Capture Method** : `Game View` ou `Camera`
   - **Source Camera** : Assignez votre caméra

Cette méthode capture la vue de la caméra au lieu de la texture directement.

**Mais** : Moins flexible et consomme plus de ressources.

---

## Résumé

✅ **Solution recommandée** :
```
GameObject
├─ SpoutSender (natif KlakSpout)
└─ SpoutTextureUpdater (force update)
```

✅ **Avantages** :
- Simple (2 composants)
- Utilise le SpoutSender natif
- Pas de code complexe
- Fonctionne pour ImageSource, OpticalFlow, etc.

✅ **Configuration** : < 2 minutes
