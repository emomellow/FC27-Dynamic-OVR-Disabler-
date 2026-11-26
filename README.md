# FC 27 Dynamic OVR Disabler

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform: Windows](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011%20x64-0078D6.svg)](https://microsoft.com/windows)
[![Game: EA SPORTS FC 27](https://img.shields.io/badge/Game-EA%20SPORTS%20FC%2027-00E5CC.svg)](https://store.steampowered.com/app/4080220)
[![Version: 0.1.0-alpha](https://img.shields.io/badge/Version-0.1.0--alpha-39F5A0.svg)](https://github.com/emomellow/FC27-Dynamic-OVR-Disabler-)

> [!NOTE]
> 🇬🇧 English documentation is on top.  
> 🇹🇷 Türkçe dökümantasyon sayfanın alt kısmında yer almaktadır.

---

## 🇬🇧 English

A standalone, open-source Windows desktop application designed to neutralize temporary **Dynamic Overall (OVR)** attribute modifiers in **EA SPORTS FC 27's offline Career Mode**.

### 🌟 Key Features

- **Form Modifier Neutralization**: Eliminates temporary attribute boosts and penalties caused by match form (`Very Good`, `Good`, `Bad`, `Very Bad`).
- **Morale Modifier Neutralization**: Eliminates artificial rating boosts and penalties caused by player morale (`Very Happy`, `Happy`, `Sad`, `Very Sad`).
- **Match Fitness Modifier Neutralization**: Disables artificial fitness scaling across player positions (`Abysmal`, `Unfit`, `Off-Par`, `Decent`, `Sharp`).
- **Multi-Language Support**: Seamlessly switch between **English** and **Turkish** inside the Settings tab without restarting.

### 🛡️ Architectural Preservation

The tool only neutralizes temporary Dynamic OVR cosmetics and guarantees:
- ✅ **Normal Permanent Player Growth**: Training curves and natural potential progression (e.g. 88 ➔ 89) remain fully intact.
- ✅ **Natural Player Regression**: Age-related attribute regression functions completely normally.
- ✅ **Injury Penalties**: Original injury penalties (`Yellow: -30`, `Orange/Red: -80`) are preserved.
- ✅ **Thresholds & Weights**: Original category thresholds and skill/physical/mental weights remain intact.

### 🔒 Safety & Independence

- **No Third-Party Tool Requirement**: Does not require FIFA Editor Tool, FIFA Mod Manager, Cheat Engine, or Live Editor during runtime.
- **Anti-Cheat Safe**: Does not use DLL injection, memory patching, driver manipulation, or executable tampering.
- **Data Integrity**: Automatic timestamped backups with SHA-256 hash verification before any write operation.
- **Deterministic & Idempotent**: Transformations are schema-aware and repeatable without side effects.

### 🚀 Quick Start Guide (For Players)

You don't need any modding experience to use this tool:

1. **Download**: Go to the [Releases](https://github.com/emomellow/FC27-Dynamic-OVR-Disabler-/releases) section and download `FC27-Dynamic-OVR-Disabler-v0.1.0-alpha-win-x64.zip`.
2. **Extract & Run**: Extract the zip anywhere on your computer and run **`OVRDisabler.App.exe`**.
3. **Automatic Detection**: The app will automatically find your EA SPORTS FC 27 installation and evaluate compatibility.
4. **Choose Options**: Under the **Dynamic OVR** tab, simply toggle which modifiers you want disabled (Form, Morale, Match Fitness).
5. **Apply / Export**:
   - Click **"Export Modified CSV..."** to generate zeroed-out data files.
   - Or launch the game via Steam with `-dataPath FIFAModData` in Steam Launch Options to load modded files directly.
6. **Play**: Launch Career Mode! Your player ratings will now show `+0 OVR` for form/morale without affecting permanent potential progression.

### ⚖️ Legal Disclaimer

This project is an independent and unofficial open-source community tool. It is not affiliated with, endorsed by, or associated with Electronic Arts Inc.

---

## 🇹🇷 Türkçe

**EA SPORTS FC 27 Çevrimdışı Kariyer Modu** için geliştirilmiş; yapay **Dynamic Overall (OVR)** nitelik modifikatörlerini nötralize eden, bağımsız ve açık kaynaklı Windows masaüstü uygulaması.

### 🌟 Temel Özellikler

- **Form Etkilerini Nötralize Etme**: Oyuncu formuna bağlı geçici nitelik bonuslarını ve cezalarını sıfırlar (`Very Good`, `Good`, `Bad`, `Very Bad`).
- **Moral Etkilerini Nötralize Etme**: Moral durumundan kaynaklanan suni reyting artış ve düşüşlerini sıfırlar (`Very Happy`, `Happy`, `Sad`, `Very Sad`).
- **Maç Kondisyonu Etkilerini Nötralize Etme**: Maç kondisyonuna bağlı pozisyonel nitelik çarpanlarını sıfırlar (`Abysmal`, `Unfit`, `Off-Par`, `Decent`, `Sharp`).
- **Çift Dil Desteği**: Ayarlar sekmesinden tek tıkla **Türkçe** ve **İngilizce** arasında anında geçiş yapabilirsiniz.

### 🛡️ Mimari Koruma Güvencesi

Uygulama yalnızca geçici Dynamic OVR makyajını sıfırlar, gerçekçi kariyer gelişimini korur:
- ✅ **Kalıcı Oyuncu Gelişimi Korunur**: Antrenman ve maç tecrübesine dayalı doğal potansiyel artışı (örneğin 88 ➔ 89) eksiksiz çalışır.
- ✅ **Yaşa Bağlı Gerileme Korunur**: Yaşlanan oyuncuların doğal fiziksel gerilemesi aynen devam eder.
- ✅ **Sakatlık Cezaları Korunur**: Orijinal sakatlık cezaları (`Sarı: -30`, `Kırmızı/Turuncu: -80`) korunur.
- ✅ **Eşikler ve Ağırlık Oranları Korunur**: Kondisyon eşikleri ve beceri/fiziksel/zihinsel ağırlık yüzdeleri bozulmaz.

### 🔒 Güvenlik ve Bağımsızlık

- **Üçüncü Parti Araç Zorunluluğu Yoktur**: FIFA Editor Tool, Mod Manager veya Cheat Engine gerektirmez.
- **Anticheat Dostu**: DLL injection, bellek yamalama, sürücü modifikasyonu veya dosya manipülasyonu yapmaz.
- **Veri Bütünlüğü**: Her işlem öncesinde SHA-256 hash doğrulamalı, zaman damgalı otomatik yedek alınır.
- **Deterministik ve Güvenli**: Dönüşümler şema duyarlıdır ve özdeş sonuçlar üretir.

### 🚀 Hızlı Başlangıç Rehberi (Oyuncular İçin)

Bu aracı kullanmak için hiçbir modlama veya teknik bilgiye ihtiyacınız yoktur:

1. **İndirin**: [Releases](https://github.com/emomellow/FC27-Dynamic-OVR-Disabler-/releases) sayfasından `FC27-Dynamic-OVR-Disabler-v0.1.0-alpha-win-x64.zip` dosyasını indirin.
2. **Çıkartın ve Çalıştırın**: ZIP dosyasını klasöre çıkartın ve **`OVRDisabler.App.exe`** uygulamasını çift tıklayarak açın.
3. **Otomatik Algılama**: Uygulama Steam ve EA SPORTS FC 27 kurulumunuzu otomatik olarak bulacaktır.
4. **Seçenekleri Belirleyin**: **Dynamic OVR** sekmesine gelip kapatmak istediğiniz etkileri seçin (Form, Moral, Maç Kondisyonu).
5. **Dışa Aktarın / Başlatın**:
   - Dilerseniz **"Düzenlenmiş Dosyaları Dışa Aktar..."** butonu ile sıfırlanmış CSV dosyalarını alabilirsiniz.
   - Ya da Steam'de oyunun Başlatma Seçenekleri'ne `-dataPath FIFAModData` ekleyerek doğrudan oyunu açabilirsiniz.
6. **Keyfini Çıkarın**: Kariyer Modunuza girin! Oyuncularınızın reytinginde form/moral bonusları artık `+0 GEN` olarak görünecek; kalıcı oyuncu gelişimi ise doğal olarak devam edecektir.

### ⚖️ Yasal Bildirim

Bu uygulama bağımsız ve gayriresmî bir açık kaynak topluluk projesidir. Electronic Arts Inc. ile hiçbir ticari veya resmi bağlantısı bulunmamaktadır.

---

### 📄 License / Lisans

MIT License — Copyright (c) 2026 emresfy.
