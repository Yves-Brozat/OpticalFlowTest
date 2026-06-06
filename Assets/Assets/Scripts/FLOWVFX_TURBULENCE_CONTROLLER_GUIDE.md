# 🌀 Guide : Contrôleur d'Intensité des Turbulences FlowVFX

## 🎯 Fonctionnalité

Un **contrôleur clavier** pour ajuster l'intensité des turbulences du FlowVFX en temps réel :
- ✅ **Z** : Augmente l'intensité de 1
- ✅ **S** : Diminue l'intensité de 1
- ✅ **R** : Réinitialise l'intensité à 1 (synchronisé avec le reset des poèmes)

---

## 🚀 Configuration (30 secondes)

### 1. Ajouter le composant à votre GameObject FlowVFX

1. Sélectionnez le GameObject qui contient le **FlowVFX** dans votre scène
2. Cliquez sur **Add Component**
3. Cherchez **FlowVFXTurbulenceController**
4. Ajoutez-le

### 2. Configurer les références

Dans l'Inspector du **FlowVFXTurbulenceController** :

#### Section **"VFX Reference"** :
- **Visual Effect** : Glissez-déposez le composant Visual Effect du FlowVFX

#### Section **"Turbulence Settings"** :
- **Turbulence Property Name** : `TurbulenceIntensity` (ou le nom de votre propriété dans le VFX Graph)
- **Initial Intensity** : `1` (valeur de départ)
- **Min Intensity** : `0` (minimum autorisé)
- **Max Intensity** : `100` (maximum autorisé)
- **Intensity Step** : `1` (incrément par appui)

#### Section **"Input Keys"** :
- **Increase Key** : `Z` (augmenter)
- **Decrease Key** : `S` (diminuer)
- **Reset Key** : `R` (réinitialiser)

---

## 🎮 Utilisation

### En jeu :

- **Touche Z** → Augmente l'intensité de 1 🔼
- **Touche S** → Diminue l'intensité de 1 🔽
- **Touche R** → Réinitialise à 1 🔄

### Exemple d'utilisation :

```
[Intensité = 1]
	↓
[Appui sur Z x5]
	↓
[Intensité = 6]
	↓
[Appui sur S x2]
	↓
[Intensité = 4]
	↓
[Appui sur R]
	↓
[Intensité = 1] ✨
```

---

## 🎨 Configuration du VFX Graph

### Dans votre FlowVFX.vfx :

1. Ouvrez le **VFX Graph** du FlowVFX
2. Ajoutez une propriété exposée de type **Float** nommée `TurbulenceIntensity`
3. Connectez cette propriété au paramètre d'intensité de votre effet de turbulence
4. Définissez la valeur par défaut à `1`

### Exemple de configuration :

```
[Exposed Property: TurbulenceIntensity (Float) = 1.0]
	↓
[Multiply]
	↓
[Turbulence Force]
```

---

## ⚙️ Personnalisation

### Modifier les touches :

Dans l'Inspector, section **"Input Keys"** :
- Changez **Increase Key** / **Decrease Key** / **Reset Key**
- Touches disponibles : `Z`, `S`, `R`, `Q`, `A`, `W`, `E`, etc.

### Modifier l'incrément :

- **Intensity Step** : Changez à `0.5` pour des ajustements plus fins, ou `5` pour des sauts plus importants

### Modifier les limites :

- **Min Intensity** : Minimum (ex: `0`)
- **Max Intensity** : Maximum (ex: `100`)

### Modifier la valeur initiale :

- **Initial Intensity** : Valeur au démarrage et après reset (ex: `1`)

---

## 🔗 Intégration avec PoemReaderPlaylist

Le contrôleur est conçu pour fonctionner en parallèle avec le **PoemReaderPlaylist** :

- **Touche R** fonctionne pour :
  - ✅ Réinitialiser toutes les zones de poèmes
  - ✅ Réinitialiser l'intensité des turbulences à 1

Les deux composants peuvent coexister sur des GameObjects différents et répondront tous les deux à la touche R.

---

## 🐛 Debug

### Activer les logs :

Dans l'Inspector, section **"Debug"** :
- ☑ **Show Debug Logs** : Cochez pour voir les messages dans la Console

### Messages de debug :

```
[FlowVFXTurbulenceController] Initialisé avec intensité = 1
[FlowVFXTurbulenceController] ⬆ Intensité augmentée : 2
[FlowVFXTurbulenceController] ⬇ Intensité diminuée : 1
[FlowVFXTurbulenceController] 🔄 Intensité réinitialisée : 1
```

### Problèmes courants :

❌ **La propriété VFX n'existe pas** :
- Vérifiez que `TurbulenceIntensity` est bien exposé dans votre VFX Graph
- Vérifiez l'orthographe du nom de la propriété

❌ **Rien ne se passe** :
- Vérifiez que le **Visual Effect** est bien assigné
- Vérifiez que le VFX est actif dans la scène
- Activez les logs de debug

---

## 📝 API Publique

### Méthodes disponibles :

```csharp
// Augmenter l'intensité
controller.IncreaseIntensity();

// Diminuer l'intensité
controller.DecreaseIntensity();

// Réinitialiser l'intensité
controller.ResetIntensity();

// Définir une valeur spécifique
controller.SetIntensity(5.0f);

// Obtenir la valeur actuelle
float current = controller.GetCurrentIntensity();
```

### Exemple d'utilisation par script :

```csharp
using OpticalFlowTest;

public class MyController : MonoBehaviour
{
	public FlowVFXTurbulenceController turbulenceController;

	void Start()
	{
		// Définir l'intensité à 10
		turbulenceController.SetIntensity(10f);
	}
}
```

---

## ✨ Résumé rapide

1. **Ajoutez** le composant **FlowVFXTurbulenceController** au GameObject FlowVFX
2. **Assignez** la référence au **Visual Effect**
3. **Configurez** le nom de la propriété : `TurbulenceIntensity`
4. **Exposez** la propriété dans votre VFX Graph
5. **Jouez** et utilisez **Z/S/R** pour contrôler l'intensité ! 🎮

---

**Créé pour le projet Optical Flow Test - Syllabe** ✨
