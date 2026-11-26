namespace OVRDisabler.Core.Localization;

public enum AppLanguage
{
    Turkish,
    English
}

public static class LocalizationManager
{
    public static AppLanguage CurrentLanguage { get; set; } = AppLanguage.Turkish;

    private static readonly Dictionary<string, (string TR, string EN)> Strings = new()
    {
        // Navigation
        ["Nav_Overview"] = ("Genel Bakış", "Overview"),
        ["Nav_DynamicOVR"] = ("Dynamic OVR", "Dynamic OVR"),
        ["Nav_Analysis"] = ("Dosya Analizi", "File Analysis"),
        ["Nav_Backups"] = ("Yedeklemeler", "Backups"),
        ["Nav_Logs"] = ("İşlem Geçmişi", "Activity Logs"),
        ["Nav_Settings"] = ("Ayarlar", "Settings"),
        ["Nav_About"] = ("Hakkında", "About"),
        ["Badge_CompatStatus"] = ("Uyumluluk Durumu:", "Compatibility Status:"),

        // Overview
        ["Overview_Title"] = ("Genel Bakış", "Overview"),
        ["Overview_Subtitle"] = ("Sistem durumu, EA SPORTS FC 27 kurulum ve uyumluluk verileri.", "System status, EA SPORTS FC 27 installation and compatibility info."),
        ["Card_GameStatus"] = ("OYUN DURUMU", "GAME STATUS"),
        ["Card_CompatStatus"] = ("UYUMLULUK DURUMU", "COMPATIBILITY STATUS"),
        ["Card_GameVersion"] = ("OYUN SÜRÜMÜ BİLGİSİ", "GAME VERSION INFO"),
        ["Card_AppInfo"] = ("UYGULAMA VE ANALİZ BİLGİSİ", "APPLICATION & ANALYSIS INFO"),
        ["Lbl_FileVersion"] = ("Dosya Sürümü: ", "File Version: "),
        ["Lbl_ProductVersion"] = ("Ürün Sürümü: ", "Product Version: "),
        ["Lbl_AppVersion"] = ("Uygulama Sürümü: ", "App Version: "),
        ["Lbl_LastAnalysis"] = ("Son Analiz: ", "Last Analysis: "),
        ["Btn_DetectGame"] = ("Oyunu Algıla", "Detect Game"),
        ["Btn_SelectFolder"] = ("Oyun Klasörünü Seç...", "Select Game Folder..."),
        ["Btn_AnalyzeFiles"] = ("Dosyaları Analiz Et", "Analyze Files"),

        // Dynamic OVR
        ["OVR_Title"] = ("Dynamic OVR Modifikasyon Ayarları", "Dynamic OVR Modification Settings"),
        ["OVR_Subtitle"] = ("Kariyer Modundaki geçici reyting modifikatörlerini nötralize edin.", "Neutralize temporary attribute modifiers in Career Mode."),
        ["Toggle_Form"] = ("Form Etkilerini Devre Dışı Bırak (Very Good / Good / Bad / Very Bad)", "Disable Form Modifiers (Very Good / Good / Bad / Very Bad)"),
        ["Toggle_Morale"] = ("Moral Etkilerini Devre Dışı Bırak (Very Happy / Happy / Sad / Very Sad)", "Disable Morale Modifiers (Very Happy / Happy / Sad / Very Sad)"),
        ["Toggle_MatchFitness"] = ("Maç Kondisyonu Etkilerini Devre Dışı Bırak (Abysmal / Unfit / Off-Par / Decent / Sharp)", "Disable Match Fitness Modifiers (Abysmal / Unfit / Off-Par / Decent / Sharp)"),
        ["Notice_Title"] = ("MİMARİ KORUMA GÜVENCESİ", "ARCHITECTURAL PRESERVATION GUARANTEE"),
        ["Notice_1"] = ("• Normal oyuncu gelişimine doğrudan müdahale edilmez (Potansiyel artışı korunur).", "• Normal permanent player growth is preserved (Potential increase unaffected)."),
        ["Notice_2"] = ("• Yaşa bağlı oyuncu gerilemesine doğrudan müdahale edilmez.", "• Age-related player attribute regression is preserved."),
        ["Notice_3"] = ("• Sakatlık cezaları orijinal değerleriyle korunur (Yellow: -30, Red/Orange: -80).", "• Original injury penalties are preserved (Yellow: -30, Red/Orange: -80)."),
        ["Notice_4"] = ("• Kondisyon eşikleri ve form/moral ağırlık yüzdeleri bozulmadan korunur.", "• Fitness thresholds and category weights are preserved untouched."),
        ["Btn_PreviewChanges"] = ("Değişiklikleri Önizle", "Preview Changes"),
        ["Btn_ExportCsv"] = ("Düzenlenmiş Dosyaları Dışa Aktar...", "Export Modified CSV..."),
        ["Btn_ApplyChanges"] = ("Değişiklikleri Uygula", "Apply Changes"),
        ["Btn_RestoreOriginal"] = ("Orijinal Dosyaları Geri Yükle", "Restore Original Files"),

        // File Analysis
        ["Analysis_Title"] = ("Dosya Analizi ve Şema Doğrulama", "File Analysis & Schema Validation"),
        ["Analysis_Subtitle"] = ("Orijinal veya düzenlenmiş CSV varlıklarını inceleyin ve doğrulayın.", "Inspect and validate original or modified CSV assets."),
        ["Btn_SelectCsv"] = ("CSV Dosyası Seç...", "Select CSV File..."),
        ["Lbl_FileName"] = ("Dosya Adı:", "File Name:"),
        ["Lbl_FileSize"] = ("Dosya Boyutu:", "File Size:"),
        ["Lbl_DetectedSchema"] = ("Tespit Edilen Şema / Tür:", "Detected Schema / Type:"),
        ["Lbl_SchemaStatus"] = ("Şema Doğrulaması:", "Schema Validation:"),
        ["Lbl_ValuesToModify"] = ("Değiştirilecek / Nötralize Edilecek Değer Sayısı:", "Values to Modify / Neutralize:"),
        ["Lbl_Sha256"] = ("SHA-256 Özeti:", "SHA-256 Checksum:"),
        ["Title_PreviewTable"] = ("Değişiklik Önizlemesi (İlk 100 Değişim)", "Change Preview (First 100 Changes)"),
        ["Col_Section"] = ("Bölüm", "Section"),
        ["Col_Row"] = ("Satır Kimliği", "Row ID"),
        ["Col_Column"] = ("Sütun", "Column"),
        ["Col_Original"] = ("Orijinal Değer", "Original Value"),
        ["Col_New"] = ("Yeni Değer", "New Value"),

        // Backups
        ["Backups_Title"] = ("Yedekleme ve Kurtarma Yönetimi", "Backup & Recovery Management"),
        ["Backups_Subtitle"] = ("İşlem öncesi oluşturulan zaman damgalı güvenli yedekler.", "Timestamped secure backups created prior to operations."),
        ["Col_Date"] = ("Tarih", "Date"),
        ["Col_Desc"] = ("Açıklama", "Description"),
        ["Col_OrigPath"] = ("Orijinal Yol", "Original Path"),
        ["Col_Size"] = ("Boyut", "Size"),

        // Activity Logs
        ["Logs_Title"] = ("İşlem Geçmişi ve Sistem Günlükleri", "Activity Logs & Diagnostics"),
        ["Logs_Subtitle"] = ("Uygulama logları ve teşhis bilgileri.", "Application runtime logs and diagnostic events."),

        // Settings
        ["Settings_Title"] = ("Ayarlar", "Settings"),
        ["Settings_Subtitle"] = ("Uygulama ve dağıtım tercihleri.", "Application preferences and configuration."),
        ["Settings_Lang"] = ("Uygulama Dili / Language:", "Language / Uygulama Dili:"),
        ["Settings_SteamDetect"] = ("Steam Otomatik Algılama: Etkin", "Steam Automatic Detection: Enabled"),
        ["Settings_BackupVerify"] = ("Yedekleme Güvenlik Doğrulaması: SHA-256 Zorunlu", "Backup Integrity Verification: SHA-256 Enforced"),
        ["Btn_SupportCoffee"] = ("Geliştiriciye Destek Ol ☕", "Buy Me a Coffee ☕"),

        // About
        ["About_Title"] = ("Hakkında", "About"),
        ["About_Subtitle"] = ("Proje bilgileri, geliştirici ve lisans hakları.", "Project info, developer and license terms."),
        ["About_AppName"] = ("FC 27 Dynamic OVR Disabler", "FC 27 Dynamic OVR Disabler"),
        ["About_Version"] = ("Sürüm: 0.1.0-alpha", "Version: 0.1.0-alpha"),
        ["About_Dev"] = ("Geliştirici: emresfy", "Developer: emresfy"),
        ["About_License"] = ("Lisans: MIT Lisansı", "License: MIT License"),
        ["About_Price"] = ("Fiyat: Tamamen Ücretsiz ve Açık Kaynak", "Pricing: Completely Free & Open Source"),
        ["About_Disclaimer"] = ("YASAL BİLDİRİM: Bu uygulama bağımsız ve gayriresmî bir topluluk projesidir. Electronic Arts ile bağlantılı değildir.",
                                "LEGAL DISCLAIMER: This application is an independent and unofficial community project. It is not affiliated with Electronic Arts.")
    };

    public static string Get(string key)
    {
        if (Strings.TryGetValue(key, out var pair))
        {
            return CurrentLanguage == AppLanguage.Turkish ? pair.TR : pair.EN;
        }
        return key;
    }
}
