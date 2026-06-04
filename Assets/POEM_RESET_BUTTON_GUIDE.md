# 🔄 Guide : Touche Reset pour PoemReaderPlaylist

## 🎯 Fonctionnalité

Une **touche clavier** qui réinitialise toutes les zones :
- ✅ Arrête tous les poèmes en cours
- ✅ Retour au premier poème de chaque playlist
- ✅ Efface tous les textes affichés
- ✅ Réinitialise les flags de démarrage

---

## 🚀 Configuration (10 secondes)

### Dans l'Inspector du PoemReaderPlaylist :

Section **"Global Controls"** :
- **Reset Key** : `R` (ou la touche de votre choix)

Touches disponibles :
- `R` (recommandé pour Reset)
- `Backspace` (effacement)
- `Delete` (suppression)
- `Digit0` (touche 0)
- `Space` (barre espace)
- etc.

---

## 🎮 Utilisation

### En jeu :

**Touche R** → Toutes les zones se réinitialisent instantanément ! 🔄

### Contrôles complets :
- **Touches 1-8** : Cycle dans les poèmes de chaque zone
- **Escape** : Arrête toutes les zones (mais ne réinitialise pas)
- **R** : Réinitialise toutes les zones (arrête + retour au début)

---

## 🎬 Comportement

```
[Zone 1 est au Poème 3, en lecture...]
[Zone 2 est au Poème 2, finie]
[Zone 5 est au Poème 1, en lecture...]
	 ↓
[Appui sur R]
	 ↓
[Toutes les zones arrêtées immédiatement]
[Tous les textes effacés]
[Toutes les zones retournent au Poème 0]
[Flags hasStarted réinitialisés]
	 ↓
[Prêt à recommencer !]
[Prochain appui sur 1-8 démarre le Poème 0]
```

---

## 🎨 Styles de bouton

### Style minimaliste :
```
Texte: "↻"
Font Size: 36
Width: 60
Height: 60
Position: Coin bas-droit
```

### Style texte :
```
Texte: "Recommencer"
Font Size: 20
Width: 180
Height: 50
Position: Centre bas
```

### Style icône + texte :
```
Image: Icône de flèche circulaire
Texte: "RESET"
Font Size: 18
Width: 120
Height: 50
```

---

## 💡 Variantes avancées

### Ajouter une confirmation

Modifiez le script pour demander confirmation :

```csharp
// Dans PoemReaderPlaylist.cs
[SerializeField] bool _confirmBeforeReset = true;

public void ResetAllZones()
{
	if (_confirmBeforeReset)
	{
		// Afficher un popup de confirmation
		// (nécessite un système de dialogue)
	}

	// ... reste du code
}
```

### Ajouter un raccourci clavier

Le bouton existe déjà, mais ajoutons aussi une touche :

```csharp
// Dans Update()
if (Keyboard.current[Key.R].wasPressedThisFrame)
{
	ResetAllZones();
}
```

### Désactiver le bouton pendant la lecture

```csharp
void Update()
{
	// Désactiver le bouton si au moins une zone est en lecture
	if (_resetButton != null)
	{
		bool anyReading = false;
		foreach (var zone in _zones)
		{
			if (zone != null && zone.isReading)
			{
				anyReading = true;
				break;
			}
		}

		_resetButton.interactable = !anyReading;
	}
}
```

---

## 🎯 Checklist

✅ 1. Créé le Button UI dans le Canvas  
✅ 2. Stylisé le bouton (taille, couleur, texte)  
✅ 3. Glissé le bouton dans **Reset Button** du PoemReaderPlaylist  
✅ 4. Play + test en cliquant  

**Résultat** : Bouton qui réinitialise toutes les zones ! 🔄

---

## 🐛 Troubleshooting

### "Le bouton ne fait rien"
✅ Vérifiez que le bouton est bien assigné dans **Reset Button**  
✅ Vérifiez la Console pour les logs (si Debug Logs activé)  
✅ Vérifiez que le bouton a un **Graphic Raycaster** sur le Canvas

### "Le bouton n'est pas cliquable"
✅ Assurez-vous que le Canvas a un **Event System** (créé automatiquement normalement)  
✅ Vérifiez que le bouton n'est pas derrière un autre élément UI  
✅ Vérifiez que **Interactable** est coché sur le Button

### "Console affiche 'Bouton de reset connecté' mais rien ne se passe"
✅ Vérifiez que vous cliquez bien sur le bouton en mode Play  
✅ Vérifiez que `ResetAllZones()` est bien appelé (ajoutez un Debug.Log)

---

## 📊 API

Le bouton appelle simplement :

```csharp
PoemReaderPlaylist reader = GetComponent<PoemReaderPlaylist>();
reader.ResetAllZones();
```

Vous pouvez appeler cette méthode depuis n'importe où dans votre code !

---

## 🎨 Exemple de layout complet

```
┌─────────────────────────────────────┐
│            Canvas                   │
│                                     │
│  ┌──────────┬──────────┐           │
│  │ Zone 1   │ Zone 2   │           │
│  │          │          │           │
│  ├──────────┼──────────┤           │
│  │ Zone 3   │ Zone 4   │           │
│  │          │          │           │
│  └──────────┴──────────┘           │
│                                     │
│                     ┌─────────┐    │
│                     │  RESET  │    │
│                     └─────────┘    │
└─────────────────────────────────────┘
```

Simple et efficace ! ✨
