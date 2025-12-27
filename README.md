# Mail Mayhem 📬

[Français](#français) | [English](#english)

---

## Français

### Description
**Mail Mayhem** est un jeu de tri de courrier développé sur Unity où le joueur doit rapidement trier des lettres en les envoyant dans les bonnes boîtes selon leur symbole. Le jeu supporte à la fois les contrôles PC (clavier) et mobile (tactile).

### 🎮 Gameplay
- Les lettres apparaissent automatiquement avec différents symboles (Carré, Triangle, Cercle, Losange)
- Le joueur doit envoyer chaque lettre dans la boîte correspondante
- Deux modes de contrôle :
  - **PC** : Touches du clavier pour lancer les lettres vers les boîtes
  - **Mobile** : Glisser-déposer (drag & drop) pour jeter les lettres
- Retour visuel instantané (succès/échec) après chaque livraison

### 🛠️ Construit avec

#### Moteur & Version
- **Unity** 6000.2.15f1 (Unity 6)
- **Universal Render Pipeline (URP)** 17.2.0

#### Packages Unity principaux
- **Input System** 1.16.0 - Gestion des entrées multi-plateforme
- **2D Animation** 12.0.3 - Système d'animation 2D
- **2D Sprite** 1.0.0 - Gestion des sprites
- **UGUI** 2.0.0 - Interface utilisateur
- **Visual Scripting** 1.9.8

##### Plugin tiers
- **DOTween** - Animations et tweening (dans Assets/Plugins/Demigiant)

### 📁 Architecture du projet

```
Assets/
├── Scenes/
│   ├── MainMenu.unity        # Menu principal
│   └── Gameplay.unity         # Scène de jeu
├── Scripts/
│   ├── Letters/               # Système de lettres
│   │   ├── Letter.cs          # Logique principale des lettres
│   │   ├── LetterSpawner.cs   # Génération automatique
│   │   ├── Animations/        # Animations des lettres
│   │   └── Throw/             # Système de lancer
│   │       ├── LetterHitDetector.cs  # Détection de collision
│   │       └── UiThrowSampler.cs     # Échantillonnage du lancer
│   ├── Boxes/                 # Système de boîtes
│   │   ├── ServiceBox.cs      # Boîte de réception
│   │   └── BoxesRegistry.cs   # Registre des boîtes
│   ├── Inputs/                # Gestion des entrées
│   │   ├── LetterInputRouter.cs   # Routeur d'entrées
│   │   ├── PcLetterInput.cs       # Contrôles PC
│   │   └── MobileLetterInput.cs   # Contrôles mobile
│   └── UI/
│       └── UiPlatformSwitcher.cs  # Basculement PC/Mobile
├── Prefabs/
│   ├── Letters/               # Préfabriqués de lettres
│   └── Boxes/                 # Préfabriqués de boîtes
├── Sprites/                   # Assets visuels
│   ├── Letters/
│   └── Boxes/
└── Settings/                  # Configuration URP et rendu
```

### 🎯 Fonctionnalités clés

#### Système de lettres
- **4 types de symboles** : Carré, Triangle, Cercle, Losange
- **États** : Idle, Dragging, Throwing, Feedback
- **Spawn automatique** avec intervalle configurable
- **Animations fluides** utilisant DOTween
- **Physique de lancer** avec friction et vélocité réaliste

#### Système d'entrées
- **Architecture modulaire** avec routeur d'entrées
- **Détection automatique** de la plateforme (PC/Mobile)
- **Input System** pour une gestion moderne des contrôles
- **Double support** : 
  - Clavier (PC)
  - Touch/Drag (Mobile)

#### Système de boîtes
- **Registre centralisé** pour gérer toutes les boîtes
- **Détection de collision** par overlap de RectTransform
- **Validation** : vérification symbole attendu vs reçu
- **Feedback visuel** instantané

#### Interface utilisateur
- **Switcheur de plateforme** pour basculer entre UI PC et Mobile
- **UI adaptative** selon le mode de contrôle
- **Retour visuel** (shake, couleurs) pour succès/échec

### 🚀 Comment démarrer

1. **Ouvrir le projet** dans Unity 6000.2.15f1 ou version compatible
2. **Charger la scène** :
   - `Assets/Scenes/MainMenu.unity` pour le menu
   - `Assets/Scenes/Gameplay.unity` pour jouer directement
3. **Appuyer sur Play** dans l'éditeur Unity
4. **Contrôles** :
   - PC : Utiliser les touches du clavier
   - Mobile : Glisser les lettres vers les boîtes

### 🔧 Configuration

Les paramètres principaux sont exposés dans l'inspecteur :
- **LetterSpawner** : Intervalle de spawn, préfab de lettre
- **Letter** : Vitesse de lancer, friction, durée d'animation
- **ServiceBox** : Type de symbole accepté
- **UiPlatformSwitcher** : Sélection manuelle ou auto du mode PC/Mobile

### 📝 Notes techniques

- Le projet utilise **namespaces** pour organiser le code (Letters, Boxes, Inputs, UI)
- La détection de hit utilise des **RectTransform en coordonnées monde**
- Le système d'entrées est **découplé** via un pattern de routeur
- Les animations utilisent **DOTween** pour des performances optimales
- Compatible avec le **Device Simulator** Unity pour tester le mode mobile

---

## English

### Description
**Mail Mayhem** is a mail sorting game developed in Unity where the player must quickly sort letters by sending them to the correct boxes based on their symbol. The game supports both PC (keyboard) and mobile (touch) controls.

### 🎮 Gameplay
- Letters automatically appear with different symbols (Square, Triangle, Circle, Diamond)
- The player must send each letter to the matching box
- Two control modes:
  - **PC**: Keyboard keys to throw letters to boxes
  - **Mobile**: Drag & drop to throw letters
- Instant visual feedback (success/failure) after each delivery

### 🛠️ Built With

#### Engine & Version
- **Unity** 6000.2.15f1 (Unity 6)
- **Universal Render Pipeline (URP)** 17.2.0

#### Main Unity Packages
- **Input System** 1.16.0 - Cross-platform input management
- **2D Animation** 12.0.3 - 2D animation system
- **2D Sprite** 1.0.0 - Sprite handling
- **UGUI** 2.0.0 - User interface
- **Visual Scripting** 1.9.8

#### Third-party Plugin
- **DOTween** - Animations and tweening (in Assets/Plugins/Demigiant)

### 📁 Project Architecture

```
Assets/
├── Scenes/
│   ├── MainMenu.unity        # Main menu
│   └── Gameplay.unity         # Gameplay scene
├── Scripts/
│   ├── Letters/               # Letter system
│   │   ├── Letter.cs          # Main letter logic
│   │   ├── LetterSpawner.cs   # Automatic spawning
│   │   ├── Animations/        # Letter animations
│   │   └── Throw/             # Throwing system
│   │       ├── LetterHitDetector.cs  # Collision detection
│   │       └── UiThrowSampler.cs     # Throw sampling
│   ├── Boxes/                 # Box system
│   │   ├── ServiceBox.cs      # Service box
│   │   └── BoxesRegistry.cs   # Box registry
│   ├── Inputs/                # Input management
│   │   ├── LetterInputRouter.cs   # Input router
│   │   ├── PcLetterInput.cs       # PC controls
│   │   └── MobileLetterInput.cs   # Mobile controls
│   └── UI/
│       └── UiPlatformSwitcher.cs  # PC/Mobile switcher
├── Prefabs/
│   ├── Letters/               # Letter prefabs
│   └── Boxes/                 # Box prefabs
├── Sprites/                   # Visual assets
│   ├── Letters/
│   └── Boxes/
└── Settings/                  # URP and render settings
```

### 🎯 Key Features

#### Letter System
- **4 symbol types**: Square, Triangle, Circle, Diamond
- **States**: Idle, Dragging, Throwing, Feedback
- **Automatic spawn** with configurable interval
- **Smooth animations** using DOTween
- **Throw physics** with friction and realistic velocity

#### Input System
- **Modular architecture** with input router
- **Automatic platform detection** (PC/Mobile)
- **Input System** for modern control management
- **Dual support**: 
  - Keyboard (PC)
  - Touch/Drag (Mobile)

#### Box System
- **Centralized registry** to manage all boxes
- **Collision detection** using RectTransform overlap
- **Validation**: expected vs received symbol verification
- **Instant visual feedback**

#### User Interface
- **Platform switcher** to toggle between PC and Mobile UI
- **Adaptive UI** based on control mode
- **Visual feedback** (shake, colors) for success/failure

### 🚀 Getting Started

1. **Open the project** in Unity 6000.2.15f1 or compatible version
2. **Load the scene**:
   - `Assets/Scenes/MainMenu.unity` for the menu
   - `Assets/Scenes/Gameplay.unity` to play directly
3. **Press Play** in the Unity editor
4. **Controls**:
   - PC: Use keyboard keys
   - Mobile: Drag letters to boxes

### 🔧 Configuration

Main parameters are exposed in the inspector:
- **LetterSpawner**: Spawn interval, letter prefab
- **Letter**: Throw speed, friction, animation duration
- **ServiceBox**: Accepted symbol type
- **UiPlatformSwitcher**: Manual or auto PC/Mobile mode selection

### 📝 Technical Notes

- The project uses **namespaces** to organize code (Letters, Boxes, Inputs, UI)
- Hit detection uses **RectTransform in world coordinates**
- The input system is **decoupled** via a router pattern
- Animations use **DOTween** for optimal performance
- Compatible with Unity's **Device Simulator** to test mobile mode

---

**Project developed with Unity 6 - 2025**

