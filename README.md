# Personal Finance Tracker

C# ve Microsoft SQL Server kullanarak geliştirdiğim konsol tabanlı kişisel gelir-gider takip uygulaması. Nesne yönelimli programlama ve temel veri tabanı işlemlerini (CRUD) pratiğe dökmek amacıyla hazırladım.

## Özellikler
* Yeni gelir ve gider kaydı ekleme
* Tüm işlemleri tarih sırasına göre listeleme
* Toplam gelir, gider ve güncel bakiye özetini görüntüleme
* Kategori bazlı harcama analizi (GROUP BY)
* İşlem numarasına (ID) göre kayıt silme

## Kullanılan Teknolojiler
* C# (.NET Console Application)
* Microsoft SQL Server (MSSQL)
* Microsoft.Data.SqlClient (ADO.NET)

## Kurulum ve Çalıştırma
1. SQL Server üzerinde `PersonalFinanceDB` adında bir veri tabanı oluşturun ve `Users`, `Categories`, `Transactions` tablolarını kurun.
2. `Program.cs` içerisindeki `baglantiAdresi` değişkenini kendi SQL Server sunucu adınıza göre güncelleyin.
3. Projeyi Visual Studio üzerinden derleyip çalıştırın.
