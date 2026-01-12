# Blackjack (Unity)

Bu proje, **Unity** kullanılarak geliştirilmiş, temel ve ileri **Blackjack (21)** kurallarını destekleyen bir kart oyunu uygulamasıdır.  
Oyun; split, double down, soft 17, dealer AI ve gelişmiş UI kontrolü gibi mekanikleri içermektedir.

---

## Özellikler

- ✅ Standart Blackjack kuralları
- ✅ Çoklu deste (deck) desteği
- ✅ **Split** (iki el ile oynama)
- ✅ **Double Down**
- ✅ **Soft 17** kuralı (dealer ayarlanabilir)
- ✅ Dealer ilk kartı **kapalı** başlar
- ✅ Dealer blackjack durumunda:
  - Oyun hemen bitmez
  - Player turu oynanır
  - Dealer kartı açıldıktan sonra sonuçlanır
- ✅ Dinamik UI (butonlar ve el geçişleri)
- ✅ Kart sprite cache sistemi
- ✅ Modüler ve genişletilebilir mimari

---

## Oyun Akışı

1. Player ve dealer başlangıçta 2’şer kart alır  
2. Dealer’ın **ilk kartı kapalı** dağıtılır  
3. Player:
   - Hit
   - Stand
   - Split (uygunsa)
   - Double Down (uygunsa)
4. Player tüm ellerini tamamladığında:
   - Dealer kapalı kartını açar
   - Dealer AI kurallara göre oynar
5. Sonuç hesaplanır ve UI üzerinden gösterilir

---

## Kurallar & Mantık

### Blackjack
- Player: 2 kartla 21 → **Blackjack**
- Dealer blackjack ise:
  - Player blackjack değilse → Dealer kazanır
  - İkisi de blackjack ise → Push

### Split
- Aynı değerde iki kart varsa yapılabilir
- Split sonrası her el bağımsız oynanır
- Her el için sonuç ayrı hesaplanır

### Double Down
- Sadece ilk 2 kartta mümkündür
- Bahis ikiye katlanır
- Tek kart çekilir ve el otomatik stand olur

---

## Proje Yapısı

```Assets/
 └── Scripts/
     ├── GameManager.cs        # Oyun akışı ve state yönetimi
     ├── BlackjackRound.cs     # Oyun kuralları ve el çözümleme
     ├── DealerAI.cs           # Dealer karar mekanizması
     ├── Deste.cs              # Kart destesi ve karıştırma
     ├── El.cs                 # El (hand) ve skor hesaplama
     ├── Kart.cs               # Kart modeli
     ├── CardPresenter.cs      # Kartların sahnede gösterimi
     ├── UIController.cs       # UI buton ve sonuç yönetimi
     └── PlayerCardImage.cs    # Kart sprite yönetimi
```

## Kontroller

- **Hit** → Kart çek
- **Stand** → Eli bitir
- **Split** → Eli ikiye böl (uygunsa)
- **Double** → Bahsi ikiye katla (uygunsa)

Butonlar oyun durumuna göre otomatik aktif/pasif olur.

---

## Teknik Detaylar

- Unity Coroutine kullanımı
- State-based game flow (`RoundState`)
- Dealer AI soft-17 kontrolü
- UI ile oyun mantığının ayrılması (MVC benzeri yapı)
- Sprite cache ile performans optimizasyonu

---

## Geliştirilebilir Özellikler

- 🔹 Insurance
- 🔹 Surrender
- 🔹 Chip / Bakiye sistemi
- 🔹 İstatistik ekranı
- 🔹 Animasyonlu kart flip
- 🔹 Çoklu split desteği

---

## Notlar

- `.vs/`, `Library/`, `Temp/` gibi klasörler **.gitignore** ile dışlanmıştır
- Proje eğitim ve geliştirme amaçlıdır

---

## Geliştirici

**Hasan Kürşat Mertoğlu**

---

## Lisans

Bu proje eğitim amaçlıdır. Ticari kullanım için düzenlenebilir.
