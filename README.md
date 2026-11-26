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

### 🚀 Installation & Usage

1. Download the latest release from the [Releases](https://github.com/emomellow/FC27-Dynamic-OVR-Disabler-/releases) section or run the portable executable:
   ```text
   publish/FC27-Dynamic-OVR-Disabler-v0.1.0-alpha-win-x64/OVRDisabler.App.exe
   ```
2. The application will automatically detect your Steam installation and game version.
3. Select your options under **Dynamic OVR** tab:
   - Disable Form Modifiers
   - Disable Morale Modifiers
   - Disable Match Fitness Modifiers
4. Export your transformed CSV or apply changes directly.

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

### 🚀 Kurulum ve Kullanım

1. [Releases](https://github.com/emomellow/FC27-Dynamic-OVR-Disabler-/releases) sayfasından son sürümü indirin veya hazır taşınabilir dosyayı çalıştırın:
   ```text
   publish/FC27-Dynamic-OVR-Disabler-v0.1.0-alpha-win-x64/OVRDisabler.App.exe
   ```
2. Uygulama Steam kurulumunuzu ve oyun sürümünüzü otomatik olarak algılayacaktır.
3. **Dynamic OVR** sekmesinden tercihlerinizi belirleyin:
   - Form Etkilerini Devre Dışı Bırak
   - Moral Etkilerini Devre Dışı Bırak
   - Maç Kondisyonu Etkilerini Devre Dışı Bırak
4. Düzenlenmiş CSV dosyanızı dışa aktarın veya doğrudan oyuna uygulayın.

### ⚖️ Yasal Bildirim

Bu uygulama bağımsız ve gayriresmî bir açık kaynak topluluk projesidir. Electronic Arts Inc. ile hiçbir ticari veya resmi bağlantısı bulunmamaktadır.

---

### 📄 License / Lisans

MIT License — Copyright (c) 2026 emresfy.
