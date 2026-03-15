# Unity VR Experiment: Sustained Attention Offline version (Fixed Difficulty)

A Unity-based experimental framework designed for cognitive load and reaction time research. This system manages object spawning, trial timing, and synchronized data streaming via **Lab Streaming Layer (LSL)**.

## 🚀 Overview

This experiment presents participants with two types of targets (**Yellow** and **Blue** cubes) that spawn at varying frequencies. The primary goal is to measure performance across different "Spawn Rates" while maintaining high-fidelity data synchronization with external sensors (EEG, Heart Rate, etc.) through LSL.



## 🛠 Features

* **LSL Integration:** 3 independent streams for markers, difficulty telemetry, and scoring.
* **Randomized Trials:** Automatic shuffling of 8 distinct difficulty levels (spawn rates).
* **Session Management:** Automated 60s trials followed by 10s rest periods.
* **Precise Cleanup:** Force-clears all active stimuli between levels to ensure trial integrity.

---

## 📊 Data Streaming (LSL)

The script initializes three `StreamOutlets`. Capture these using **LabRecorder** or a custom LSL inlet:

| Stream Name | Type | Channel Count | Description |
| :--- | :--- | :--- | :--- |
| `CubeSpawnData` | Markers | 4 | $[X, Y, Z, \text{ID}]$ (ID 0: Yellow, ID 1: Blue) |
| `SpawnRate` | Metadata | 1 | Current spawn interval in seconds |
| `CubeDestroyScore` | Performance | 1 | Real-time cumulative hit count |

---

## ⚙️ Configuration & Setup

### 1. Requirements
* **Unity:** Recommended 2020.3+
* **Dependencies:** * [LSL4Unity](https://github.com/labstreaminglayer/LSL4Unity)
    * TextMeshPro
    * SteamVR (or compatible OpenVR wrapper)

### 2. Inspector Assignment
Attach the `core3.cs` script to an empty Manager object and assign the following:

* **Prefabs:** * `Cube Yellow Prefab`: Target object for Type 0.
    * `Cube Blue Prefab`: Target object for Type 1.
* **Spawn Points:** An array of `Transforms` where objects can appear.
* **UI:** A `TextMeshPro` (World Space or Overlay) for the `Game Time Text`.

---

## 🕹 Experimental Protocol

1.  **Preparation (10s):** Initial buffer to allow the participant to stabilize.
2.  **Trial Phase (60s):**
    * Yellow cubes spawn every `spawnRate` seconds.
    * Blue cubes spawn every `spawnRate` seconds (with a 1s offset).
    * Coordinates and object types are pushed to LSL on instantiation.
3.  **Break Phase (10s):**
    * `gameTime` counts down the rest period.
    * Spawn rates are set to 0.
    * Remaining objects are destroyed.
4.  **Loop:** The system selects the next shuffled spawn rate and repeats until 16 levels (default) are completed.

---

## 📝 Key Variables

You can modify these in the script to suit your specific study design:
* `spawnRates`: A list of floats representing the time (in seconds) between spawns.
* `breakTime`: Duration of rest between trials (default `10f`).
* `gameTime`: Duration of each active trial (default `60f`).

---

## 💾 License
This project is intended for research purposes. Please cite this repository if used in published work.
Use the following references to published papers: 
Predictive modelling of cognitive workload in VR: An eye-tracking approach. In Proceedings of the 2024 Symposium on Eye Tracking Research and Applications, D. Szczepaniak, M. Harvey and F. Deligianni,  Association for Computing Machinery, Art 46, pp. 1-3, 2024 (2024) https://dl.acm.org/doi/10.1145/3649902.3655642 
ML-driven cognitive workload estimation in a VR-based sustained attention task, D. Szczepaniak, M. Harvey and F. Deligianni, IEEE International Symposium on Mixed and Augmented Reality Adjunct (ISMAR-Adjunct), pp. 557-560, 2024 (2024) https://ieeexplore.ieee.org/document/10765330 
