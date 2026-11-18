using Microsoft.Data.SqlClient;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string baglantiAdresi = @"Server=localhost\SQLEXPRESS;Database=PersonalFinanceDB;Trusted_Connection=True;TrustServerCertificate=True;";

while (true)
{
    Console.Clear();
    Console.WriteLine("=========================================");
    Console.WriteLine("      KISISEL FINANS TAKIP SISTEMI       ");
    Console.WriteLine("=========================================");
    Console.WriteLine("1 - Yeni Gelir / Gider Ekle");
    Console.WriteLine("2 - Tum Islemleri Listele");
    Console.WriteLine("3 - Bakiye ve Harcama Ozetini Gor");
    Console.WriteLine("4 - Kategori Bazli Harcama Analizi");
    Console.WriteLine("5 - Kayit Sil");
    Console.WriteLine("6 - Kayit Guncelle");
    Console.WriteLine("7 - Aylik Islem Filtreleme");
    Console.WriteLine("0 - Cikis");
    Console.WriteLine("-----------------------------------------");
    Console.Write("Seciminiz: ");
    string secim = Console.ReadLine();

    if (secim == "1")
    {
        IslemEkle(baglantiAdresi);
    }
    else if (secim == "2")
    {
        IslemleriListele(baglantiAdresi);
    }
    else if (secim == "3")
    {
        OzetGoster(baglantiAdresi);
    }
    else if (secim == "4")
    {
        KategoriAnalizi(baglantiAdresi);
    }
    else if (secim == "5")
    {
        KayitSil(baglantiAdresi);
    }
    else if (secim == "6")
    {
        KayitGuncelle(baglantiAdresi);
    }
    else if (secim == "7")
    {
        AylikFiltrele(baglantiAdresi);
    }
    else if (secim == "0")
    {
        Console.WriteLine("Programdan cikiliyor...");
        break;
    }
    else
    {
        Console.WriteLine("Hatali secim yaptiniz!");
        Console.ReadKey();
    }
}

static void IslemEkle(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- YENI GELIR / GIDER EKLE ---\n");

    try
    {
        SqlConnection baglanti = new SqlConnection(baglantiAdresi);
        baglanti.Open();

        SqlCommand katKomut = new SqlCommand("SELECT CategoryId, CategoryName, CategoryType FROM Categories", baglanti);
        SqlDataReader okuyucu = katKomut.ExecuteReader();

        while (okuyucu.Read())
        {
            Console.WriteLine(okuyucu["CategoryId"] + " - " + okuyucu["CategoryName"] + " (" + okuyucu["CategoryType"] + ")");
        }
        okuyucu.Close();

        Console.Write("\nKategori Numarasi Secin: ");
        int kategoriId = int.Parse(Console.ReadLine());

        Console.Write("Tutar Girin (Ornek: 150,50): ");
        decimal tutar = decimal.Parse(Console.ReadLine());

        Console.Write("Aciklama Girin: ");
        string aciklama = Console.ReadLine();

        string ekleSorgu = "INSERT INTO Transactions (UserId, CategoryId, Amount, Description) VALUES (1, @katId, @tutar, @aciklama)";
        SqlCommand ekleKomut = new SqlCommand(ekleSorgu, baglanti);
        ekleKomut.Parameters.AddWithValue("@katId", kategoriId);
        ekleKomut.Parameters.AddWithValue("@tutar", tutar);
        ekleKomut.Parameters.AddWithValue("@aciklama", aciklama);

        ekleKomut.ExecuteNonQuery();
        baglanti.Close();

        Console.WriteLine("\nIslem basariyla kaydedildi!");
    }
    catch (Exception)
    {
        Console.WriteLine("\nHata: Girdiginiz bilgileri kontrol edin (Sayi yerine harf girmis olabilirsiniz).");
    }

    Console.WriteLine("Menuye donmek icin bir tusa basin...");
    Console.ReadKey();
}

static void IslemleriListele(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- TUM FINANSAL ISLEMLER ---\n");

    SqlConnection baglanti = new SqlConnection(baglantiAdresi);
    baglanti.Open();

    string sorgu = @"SELECT t.TransactionId, c.CategoryName, c.CategoryType, t.Amount, t.Description, t.TransactionDate 
                     FROM Transactions t
                     INNER JOIN Categories c ON t.CategoryId = c.CategoryId
                     ORDER BY t.TransactionDate DESC";

    SqlCommand komut = new SqlCommand(sorgu, baglanti);
    SqlDataReader okuyucu = komut.ExecuteReader();

    while (okuyucu.Read())
    {
        string tarih = Convert.ToDateTime(okuyucu["TransactionDate"]).ToString("dd.MM.yyyy HH:mm");
        Console.WriteLine($"ID: {okuyucu["TransactionId"]} | [{tarih}] {okuyucu["CategoryType"]} - {okuyucu["CategoryName"]} : {okuyucu["Amount"]} TL ({okuyucu["Description"]})");
    }

    okuyucu.Close();
    baglanti.Close();

    Console.WriteLine("\nMenuye donmek icin bir tusa basin...");
    Console.ReadKey();
}

static void OzetGoster(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- BAKIYE VE HARCAMA OZETI ---\n");

    SqlConnection baglanti = new SqlConnection(baglantiAdresi);
    baglanti.Open();

    string sorgu = @"SELECT c.CategoryType, ISNULL(SUM(t.Amount), 0) AS Toplam
                     FROM Categories c
                     LEFT JOIN Transactions t ON c.CategoryId = t.CategoryId
                     GROUP BY c.CategoryType";

    SqlCommand komut = new SqlCommand(sorgu, baglanti);
    SqlDataReader okuyucu = komut.ExecuteReader();

    decimal toplamGelir = 0;
    decimal toplamGider = 0;

    while (okuyucu.Read())
    {
        string tur = okuyucu["CategoryType"].ToString();
        decimal toplam = Convert.ToDecimal(okuyucu["Toplam"]);

        if (tur == "Gelir")
            toplamGelir = toplam;
        else if (tur == "Gider")
            toplamGider = toplam;
    }

    okuyucu.Close();
    baglanti.Close();

    decimal netBakiye = toplamGelir - toplamGider;

    Console.WriteLine("Toplam Gelir : " + toplamGelir + " TL");
    Console.WriteLine("Toplam Gider : " + toplamGider + " TL");
    Console.WriteLine("-----------------------------");
    Console.WriteLine("Guncel Bakiye: " + netBakiye + " TL");

    Console.WriteLine("\nMenuye donmek icin bir tusa basin...");
    Console.ReadKey();
}

static void KategoriAnalizi(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- KATEGORI BAZLI HARCAMA ANALIZI ---\n");

    SqlConnection baglanti = new SqlConnection(baglantiAdresi);
    baglanti.Open();

    string sorgu = @"SELECT c.CategoryName, SUM(t.Amount) AS ToplamHarcama
                     FROM Transactions t
                     INNER JOIN Categories c ON t.CategoryId = c.CategoryId
                     WHERE c.CategoryType = 'Gider'
                     GROUP BY c.CategoryName
                     ORDER BY ToplamHarcama DESC";

    SqlCommand komut = new SqlCommand(sorgu, baglanti);
    SqlDataReader okuyucu = komut.ExecuteReader();

    while (okuyucu.Read())
    {
        Console.WriteLine(okuyucu["CategoryName"] + " : " + okuyucu["ToplamHarcama"] + " TL");
    }

    okuyucu.Close();
    baglanti.Close();

    Console.WriteLine("\nMenuye donmek icin bir tusa basin...");
    Console.ReadKey();
}

static void KayitSil(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- KAYIT SILME ---\n");

    try
    {
        SqlConnection baglanti = new SqlConnection(baglantiAdresi);
        baglanti.Open();

        string listeSorgu = @"SELECT t.TransactionId, c.CategoryName, t.Amount, t.Description 
                              FROM Transactions t
                              INNER JOIN Categories c ON t.CategoryId = c.CategoryId";
        SqlCommand listeKomut = new SqlCommand(listeSorgu, baglanti);
        SqlDataReader okuyucu = listeKomut.ExecuteReader();

        while (okuyucu.Read())
        {
            Console.WriteLine($"ID: {okuyucu["TransactionId"]} | {okuyucu["CategoryName"]} - {okuyucu["Amount"]} TL ({okuyucu["Description"]})");
        }
        okuyucu.Close();

        Console.Write("\nSilmek istediginiz islemin ID numarasini girin: ");
        int silinecekId = int.Parse(Console.ReadLine());

        string silSorgu = "DELETE FROM Transactions WHERE TransactionId = @id";
        SqlCommand silKomut = new SqlCommand(silSorgu, baglanti);
        silKomut.Parameters.AddWithValue("@id", silinecekId);

        int etkilenenSatir = silKomut.ExecuteNonQuery();
        baglanti.Close();

        if (etkilenenSatir > 0)
            Console.WriteLine("\nKayit basariyla silindi!");
        else
            Console.WriteLine("\nBu ID numarasina ait kayit bulunamadi.");
    }
    catch (Exception)
    {
        Console.WriteLine("\nHata: Gecerli bir ID numarasi girmelisiniz.");
    }

    Console.WriteLine("Menuye donmek icin bir tusa basin...");
    Console.ReadKey();
}
static void KayitGuncelle(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- KAYIT GUNCELLEME ---\n");

    try
    {
        SqlConnection baglanti = new SqlConnection(baglantiAdresi);
        baglanti.Open();

        string listeSorgu = @"SELECT t.TransactionId, c.CategoryName, t.Amount, t.Description 
                              FROM Transactions t
                              INNER JOIN Categories c ON t.CategoryId = c.CategoryId";
        SqlCommand listeKomut = new SqlCommand(listeSorgu, baglanti);
        SqlDataReader okuyucu = listeKomut.ExecuteReader();

        while (okuyucu.Read())
        {
            Console.WriteLine($"ID: {okuyucu["TransactionId"]} | {okuyucu["CategoryName"]} - {okuyucu["Amount"]} TL ({okuyucu["Description"]})");
        }
        okuyucu.Close();

        Console.Write("\nGuncellemek istediginiz islemin ID numarasini girin: ");
        int guncellenecekId = int.Parse(Console.ReadLine());

        Console.Write("Yeni Tutar Girin (Ornek: 200,00): ");
        decimal yeniTutar = decimal.Parse(Console.ReadLine());

        Console.Write("Yeni Aciklama Girin: ");
        string yeniAciklama = Console.ReadLine();

        string guncelleSorgu = "UPDATE Transactions SET Amount = @tutar, Description = @aciklama WHERE TransactionId = @id";
        SqlCommand guncelleKomut = new SqlCommand(guncelleSorgu, baglanti);
        guncelleKomut.Parameters.AddWithValue("@tutar", yeniTutar);
        guncelleKomut.Parameters.AddWithValue("@aciklama", yeniAciklama);
        guncelleKomut.Parameters.AddWithValue("@id", guncellenecekId);

        int etkilenenSatir = guncelleKomut.ExecuteNonQuery();
        baglanti.Close();

        if (etkilenenSatir > 0)
            Console.WriteLine("\nKayit basariyla guncellendi!");
        else
            Console.WriteLine("\nBu ID numarasina ait kayit bulunamadi.");
    }
    catch (Exception)
    {
        Console.WriteLine("\nHata: Girdiginiz bilgileri kontrol edin.");
    }

    Console.WriteLine("Menuye donmek icin bir tusa basin...");
    Console.ReadKey();
}
static void AylikFiltrele(string baglantiAdresi)
{
    Console.Clear();
    Console.WriteLine("--- AYLIK ISLEM FILTRELEME ---\n");

    try
    {
        Console.Write("Yil Girin (Ornek: 2026): ");
        int yil = int.Parse(Console.ReadLine());

        Console.Write("Ay Girin (1-12 arasi): ");
        int ay = int.Parse(Console.ReadLine());

        SqlConnection baglanti = new SqlConnection(baglantiAdresi);
        baglanti.Open();

        string sorgu = @"SELECT t.TransactionId, c.CategoryName, c.CategoryType, t.Amount, t.Description, t.TransactionDate 
                         FROM Transactions t
                         INNER JOIN Categories c ON t.CategoryId = c.CategoryId
                         WHERE YEAR(t.TransactionDate) = @yil AND MONTH(t.TransactionDate) = @ay
                         ORDER BY t.TransactionDate DESC";

        SqlCommand komut = new SqlCommand(sorgu, baglanti);
        komut.Parameters.AddWithValue("@yil", yil);
        komut.Parameters.AddWithValue("@ay", ay);

        SqlDataReader okuyucu = komut.ExecuteReader();

        decimal aylikGelir = 0;
        decimal aylikGider = 0;
        bool kayitVarMi = false;

        Console.WriteLine($"\n--- {ay}.{yil} DONEMI ISLEMLERI ---");

        while (okuyucu.Read())
        {
            kayitVarMi = true;
            string tarih = Convert.ToDateTime(okuyucu["TransactionDate"]).ToString("dd.MM.yyyy HH:mm");
            string tur = okuyucu["CategoryType"].ToString();
            decimal tutar = Convert.ToDecimal(okuyucu["Amount"]);

            if (tur == "Gelir")
                aylikGelir += tutar;
            else if (tur == "Gider")
                aylikGider += tutar;

            Console.WriteLine($"ID: {okuyucu["TransactionId"]} | [{tarih}] {tur} - {okuyucu["CategoryName"]} : {tutar} TL ({okuyucu["Description"]})");
        }

        okuyucu.Close();
        baglanti.Close();

        if (!kayitVarMi)
        {
            Console.WriteLine("Secilen ay ve yila ait kayit bulunamadi.");
        }
        else
        {
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine($"Secilen Ay Toplam Gelir : {aylikGelir} TL");
            Console.WriteLine($"Secilen Ay Toplam Gider : {aylikGider} TL");
            Console.WriteLine($"Secilen Ay Net Durum    : {aylikGelir - aylikGider} TL");
        }
    }
    catch (Exception)
    {
        Console.WriteLine("\nHata: Gecerli bir yil veya ay numarasi girmelisiniz.");
    }

    Console.WriteLine("\nMenuye donmek icin bir tusa basin...");
    Console.ReadKey();
}