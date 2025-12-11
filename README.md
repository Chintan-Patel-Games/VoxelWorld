# VoxelWorld – Infinite Procedural Voxel Terrain in Unity

VoxelWorld is a fully procedural, Minecraft-style voxel terrain system built in Unity.  
It features **multithreaded mesh generation**, **biome-based terrain**, **tree generation**, **chunk streaming**, and a clean **Service-based architecture**.

This project is designed for performance, scalability, and extensibility — making it a solid foundation for voxel sandbox games.

---

## 🚀 Features

### ✔ Infinite Procedural Terrain
Terrain generation uses biome-driven heightmaps through `TerrainService` and `BiomeProvider`.  
<sup>Source: TerrainService.cs, BiomeProvider.cs</sup>

### ✔ Biome System
Smooth biome blending with Perlin noise and weight distributions:
- Plains  
- Oak Forest  
- Mountains  

<sup>Source: PlainsBiome.cs, OakForestBiome.cs, MountainBiome.cs, Biome.cs</sup>

### ✔ Chunk-Based World Streaming
Chunks load, unload, mesh, and destroy automatically depending on player distance using `WorldController`.

Ranges:
- **VIEW_RADIUS** → Mesh built  
- **LOAD_RADIUS** → Chunk data generated  
- **UNLOAD_DATA_RADIUS** → Chunk destroyed  
- **Simulation Distance** → Colliders enabled

<sup>Source: WorldController.cs</sup>

### ✔ Multithreaded Mesh Generation
Meshing happens asynchronously using `MeshService` and is applied safely on the main thread using `ChunkRunner`.

<sup>Source: MeshService.cs, ChunkRunner.cs</sup>

### ✔ Optimized Mesh Creation + UV Atlas Support
Generates faces only when needed (no hidden faces), with UVs fetched from a texture atlas via `BlockUVData`.

<sup>Source: MeshController.cs, BlockUVData.cs</sup>

### ✔ Tree Generation System
Each biome controls its own:
- Tree spawn chance  
- Height  
- Leaf variation  

Trees avoid overlapping and respect chunk boundaries.

<sup>Source: TreeService.cs</sup>

### ✔ Block System
Blocks include:
- Air  
- Grass  
- Dirt  
- Stone  
- Wood  
- Leaves  
- Snow  

<sup>Source: Block.cs, BlockType.cs</sup>

### ✔ Clean, Modular Architecture
Key services:
- `GameService` → Bootstraps world, trees, events, and player spawn  
- `WorldService` → Manages terrain + chunk streaming  
- `ChunkService` → Handles chunk creation, linking, and meshing  
- `EventService` → Handles async notifications (e.g., chunk mesh ready)

<sup>Source: GameService.cs, WorldService.cs, ChunkService.cs, EventService.cs</sup>

### ✔ Event-Driven Player Spawning
Player appears only after the spawn chunk mesh is ready.

---

## 🧠 How It Works (Technical Breakdown)

### 1. World Bootstrapping
`GameService`:
- Initializes terrain, biomes, chunk streaming  
- Spawns player only after mesh-ready event  
- Assigns Cinemachine camera follow target  

<sup>Source: GameService.cs</sup>

---

### 2. Chunk Lifecycle

#### **Chunk Generation**
Handled by `ChunkService.GenerateChunk()`:
1. Instantiate `ChunkView`  
2. Build chunk model  
3. Fill blocks using terrain generator  
4. Place trees  
5. Link chunk neighbors  
6. Mark ready  

<sup>Source: ChunkService.cs</sup>

---

### 3. Chunk Meshing (Multithreaded)

- Worker thread → `MeshModel` (vertices, triangles, UVs)  
- Main thread → Mesh applied by `ChunkRunner`  

<sup>Source: MeshService.cs, ChunkRunner.cs</sup>

---

### 4. World Streaming System

`WorldController` recalculates required chunks each frame:

- Load chunks in **LOAD_RADIUS**  
- Build meshes in **VIEW_RADIUS**  
- Unload meshes outside visible range  
- Destroy very far chunk data  
- Enable colliders near player  

<sup>Source: WorldController.cs</sup>

---

### 5. Trees & Vegetation
Biome-specific rules determine:
- Placement probability  
- Tree height  
- Leaf distribution  

<sup>Source: TreeService.cs</sup>

---

## 🕹 Player Setup

Player spawn is determined by:
- Random location  
- Nearest terrain surface height  
- `PlayerCameraRoot` assigned to Cinemachine  

<sup>Source: GameService.cs</sup>

---

## 🛠 Requirements

- Unity **2022.3+**
- A chunk prefab containing:
  - MeshFilter  
  - MeshRenderer  
  - MeshCollider  
  (Assigned via `ChunkView.cs`)

---

## 📸 Screenshots

<img width="1920" height="1080" alt="Screenshot 2025-12-11 180623" src="https://github.com/user-attachments/assets/d8b2f4b5-e277-4816-a8cb-bf1bdcbbd530" />
<img width="1920" height="1080" alt="Screenshot 2025-12-11 180629" src="https://github.com/user-attachments/assets/a931c3b8-7314-451c-9fd0-a4f7088d2d1b" />
<img width="1920" height="1080" alt="Screenshot 2025-12-11 180739" src="https://github.com/user-attachments/assets/7438cd51-484f-4d1f-a4af-13d9c68d6c08" />
<img width="1920" height="1080" alt="Screenshot 2025-12-11 180845" src="https://github.com/user-attachments/assets/5fb46ab5-9352-42f3-80aa-ddbcb413987e" />

---

## 🎥 Game Demo Video

[Watch The Demo] https://drive.google.com/file/d/1iXSV0DHwYy2G-RZVpjXXmU-A7sHsgd_Z/view?usp=drive_link

---

## 🔧 Future Extensions

Potential future improvements:
- Block breaking/placing system  
- Caves, ores, biomes like desert, snow, swamp  
- Save/load of generated chunks  
- Compute shader GPU meshing  
- Multiplayer world sync  
- Village/structure generation  

---

## 🙌 Credits

Developed by **Chintan Patel** (Black Ember Studios)

---
