# 🏭 Factory Frenzy — Module PC Multijoueur (POC)

Prototype multijoueur PC de **Factory Frenzy**, un party-game inspiré de *Fall Guys*.  
Objectif : tester en 10 jours la viabilité d’une architecture en **deux applications** :

1. **VR Editor** — création de niveaux, export JSON  
2. **PC Game (KMS)** — jeu multijoueurs utilisant les niveaux VR

Ce dépôt contient **la partie PC multijoueurs** du POC.

---

## 🎮 Fonctionnalités implémentées (POC)

| Code        | Feature                                              | Statut |
|-------------|------------------------------------------------------|--------|
| PC-PLA-P1   | TPS Character Controller en multijoueurs             | ✅ |
| PC-PLA-P2   | Plateforme de début de course                        | ✅ |
| PC-PLA-P3   | Plateforme de fin de course                          | ✅ |
| PC-PLA-P4   | Plateforme classique + gestion de la chute           | ✅ |
| PC-PLA-P5   | Plateforme mouvante                                  | ✅ |
| PC-PLA-P6   | Plateforme trampoline                                | ✅ |
| PC-PLA-P7   | Checkpoint                                           | ✅ |
| PC-TRA-T1   | Piège Bumper                                         | ✅ |
| PC-TRA-T2   | Piège Lanceur                                        | ✅ |
| PC-TRA-T3   | Piège Ventilateur                                    | ✅ |
| PC-LOB-L1   | Lobby multijoueurs                                   | ✅ |
| PC-LOB-L2   | Import de niveaux exportés depuis l’éditeur VR (JSON)| ✅ |

---

## ⌨️ Commandes

**Joueur**
- **Mouvements** : `ZQSD`
- **Caméra** : mouvement souris

**Host**
- **Changer de niveau** : `A` / `E`
- **Lancer la partie** : `K`

---

## 🧱 Architecture côté PC

- **Moteur** : Unity 2022.3.8
- **Réseau** : Netcode for GameObjects

---

## 🚀 Démarrage rapide

```bash
git clone <repo>
# Ouvrir dans Unity
# Lancer la scène StartUp pour test multi local/lan
```

---

## Lien vers Editeur
https://github.com/AlexCTZ/RV
