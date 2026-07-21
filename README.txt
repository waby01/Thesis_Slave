# 🎓 THESIS SLAVE: ACC IS A MYTH
> **Turn-Based Card Battle Game tentang Perjuangan Mahasiswa Melawan Coretan Dosen Pembimbing**

---

## Deskripsi Proyek

**Thesis Slave** adalah game *deck-building / tactical card-battle* 2D bertema satir akademis. Pemain berperan sebagai seorang mahasiswa tingkat akhir yang berjuang menghabiskan "Bar Revisi" Dosen Pembimbing menggunakan strategi kombinasi kartu, sambil terus menjaga indikator **Mental Health** agar tidak *drop* menjadi nol.

Game ini dikembangkan menggunakan engine **Unity 6** dengan menerapkan arsitektur **Finite State Machine (FSM)** dan **Action Queue Execution** untuk mengelola alur permainan *turn-based* yang dinamis.

---

## Gameplay Mechanics & Flow

### Alur Permainan (Turn-Based Loop)
1. **Player Turn Phase:** 
   * Banner `"PLAYER TURN"` muncul selama 1.5 detik.
   * Player dibekali **Stamina** (default: 3) dan menarik 4 kartu dari Deck.
   * Kartu yang diklik tidak langsung mengeksekusi serangan, melainkan masuk ke dalam **Antrean Aksi (Action Queue)**.
2. **Execution Phase:** 
   * Saat tombol **`END TURN`** ditekan, seluruh kartu dalam antrean dieksekusi satu per satu dengan jeda animasi/efek.
3. **Enemy Turn Phase:** 
   * Banner `"ENEMY TURN"` muncul selama 1.5 detik.
   * Dosen Pembimbing / Draf Skripsi memberikan serangan coretan (*damage*) yang mengurangi **Mental Health** player.
4. **Draw & Reset Phase:**
   * Terdapat jeda 2 detik sebelum giliran baru dimulai. Stamina di-reset, dan player menarik kartu baru.

---

## Sistem Kartu (12 Cards System)

| Nama Kartu | Tipe | Cost Stamina | Efek Utama |
| :--- | :---: | :---: | :--- |
| **KATING** | `Draw` | 1 | Mengambil tambahan kartu dari Deck ke tangan |
| **JURNAL ILEGAL** | `Draw` | 1 | Mengambil kartu ekstra untuk opsi serangan |
| **KOPAG** | `Heal` | 1 | Memulihkan sebagian Mental Health player |
| **LAPOR ORTU** | `Heal` | 2 | Pemulihan Mental Health dalam jumlah besar |
| **GANTI JUDUL** | `Discard` | Variable | Membuang seluruh kartu di tangan & mengocok ulang (*Redraw*) |
| **LAPORAN KAPRODI** | `Damage`| 3 | Mengurangi Bar Revisi Dosen hingga mencapai 0 (ACC) |
| **LOGIN** | `Heal` | 1 || Memulihkan sebagian Mental Health player |
| **MIE AYAM** | `Stamina Buff' | 1 | Memulihkan sebagian stamina player |
| **PINJEM LAPTOP** | `Damage`| 2 | Mengurangi Bar Revisi Dosen (ACC) |
| **REVISI** | `Damage`| 1 | Mengurangi Bar Revisi Dosen (ACC) |
| **SKS** | `Damage`| 2 | Mengurangi Bar Revisi Dosen (ACC) |
| **TURU** | `Stamina Buff' | 0 | Memulihkan sebagian stamina player |

---

## Struktur & Arsitektur Script (C#)
Assets/
└── Scripts/
├── BattleManager.cs      # Core Game Loop, FSM State Machine, Queue Execution
├── AudioManagers.cs      # Singleton Audio Controller (BGM, SFX, Panel Mute Override)
├── DeckManager.cs       # Pengelola Pengocokan Deck, Draw, Discard, & Hand Visual
├── SlotCard.cs          # Dynamic UI Card Spawner & Rendering
└── MuteButtonVisual.cs  # Observer Visual Listener untuk Status Mute Audio

### Fitur Kunci Teknis:
* **State Machine Pattern:** Mengelola status `Start`, `PlayerTurn`, `ExecutingPlayerActions`, `EnemyTurn`, `Win`, dan `Lose`.
* **Action Queuing System:** Mencegah kalkulasi instan, menciptakan efek serangan berurutan (*turn-based feeling*).
* **UI Protection & Interaction Lock:** Tombol `End Turn` dan kartu terkunci otomatis (`interactable = false`) saat panel indikator atau menu pause sedang aktif.
* **Persistent Audio Manager:** Event-driven architecture untuk perubahan status audio yang ter-sinkronisasi lintas scene.

---

## Kontrol Permainan

| Tombol / Aksi | Fungsi |
| :--- | :--- |
| **Klik Kiri Mouse** | Memilih / Memainkan Kartu & Mengklik Tombol UI |
| **`ESC` / `P`** | Membuka / Menutup Panel Pause |
| **`E`** | Mute / Unmute Audio Global |

---

## Petunjuk Instalasi & Menjalankan Proyek

1. **Persyaratan Software:**
   * Engine: **Unity 6 (6000.4.3f1)** atau versi yang kompatibel.
2. **Langkah Menjalankan:**
   * Clone/download repository ini.
   * Buka project lewat Unity Hub.
   * Buka Scene: `Assets/Scenes/MainMenu.unity`.
   * Tekan tombol **Play** di Unity Editor.

---