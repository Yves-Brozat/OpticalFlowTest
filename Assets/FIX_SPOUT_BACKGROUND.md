# Fix : Unity freeze quand la fenêtre n'est pas au premier plan

## Problème
Quand vous passez à Resolume/autre app, Unity réduit son framerate → Spout freeze.

## Solution rapide (30 secondes)

### Option 1 : Avec SpoutOptimization (Recommandé)
1. Sur **n'importe quel GameObject** (Main Camera par exemple)
2. `Add Component → Spout Optimization`
3. C'est tout ! ✅

**Configuration automatique :**
- ✅ Run in Background activé
- ✅ VSync désactivé
- ✅ Framerate forcé à 60 FPS
- ✅ Continue même si fenêtre minimisée
- ✅ Logs de debug

### Option 2 : Avec RunInBackground (Simple)
1. Sur n'importe quel GameObject
2. `Add Component → Run In Background`
3. Ajustez le framerate si besoin (défaut: 60 FPS)

---

## Vérification

### Dans Unity Console, vous devriez voir :
```
=== Spout Optimization ===
Run In Background: True
Target Framerate: 60 FPS
VSync: Disabled
Platform: WindowsEditor
```

### Test :
1. Lancez Unity (Play)
2. Passez à Resolume (Alt+Tab)
3. Le flux Spout devrait **rester fluide** ✅
4. Dans Unity Console : "Unity a perdu le focus - Spout continue à 60 FPS"

---

## Configuration manuelle (Alternative)

Si vous ne voulez pas utiliser les scripts :

### Dans Unity Editor :
```
Edit → Project Settings → Player → Resolution and Presentation
└─ Run In Background : ✅ Coché
```

### Dans un script existant :
```csharp
void Awake() {
	Application.runInBackground = true;
	Application.targetFrameRate = 60;
	QualitySettings.vSyncCount = 0;
}
```

---

## Options avancées

### Framerate personnalisé
Dans l'Inspector (SpoutOptimization) :
- **30 FPS** : Économie de ressources
- **60 FPS** : Standard (recommandé)
- **90 FPS** : Haute fluidité
- **120 FPS** : Maximum

### Build (Application finale)
Les scripts gèrent automatiquement :
- ✅ Continue quand minimisé
- ✅ Continue en arrière-plan
- ✅ Framerate stable

---

## Troubleshooting

### "Freeze toujours quand je change de fenêtre"
1. Vérifiez dans Console : `Run In Background: True`
2. Si `False` : Le script n'est pas actif
3. Solution : Ajoutez le script sur un GameObject actif

### "Performance dégradée"
1. Baissez le framerate : 30 au lieu de 60
2. Vérifiez Stats (Game view) : CPU/GPU usage
3. Optimisez la résolution de la webcam

### "En build ça ne marche pas"
1. Vérifiez : Edit → Project Settings → Player
2. Run In Background doit être coché
3. Ou utilisez SpoutOptimization (configure automatiquement)

---

## Résumé

**Avant** :
- ❌ Unity pas au premier plan → Framerate réduit → Spout freeze

**Après** (avec SpoutOptimization) :
- ✅ Unity en arrière-plan → 60 FPS stable → Spout fluide
- ✅ Fonctionne en Editor ET en Build
- ✅ Configuration automatique
- ✅ Aucune intervention manuelle

**Setup** : Ajoutez SpoutOptimization sur n'importe quel GameObject. C'est tout ! 🚀
