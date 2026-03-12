# OrdoMill 💊

**OrdoMill** هو تطبيق مكتبي لإدارة الصيدليات مبني بـ **.NET 10** و **WPF**.

يساعد التطبيق الصيادلة على تنظيم العمل اليومي بشكل أسرع وأوضح، من خلال إدارة:

- الوصفات الطبية (Ordonnances) 🧾
- المرضى 👨‍⚕️👩‍⚕️
- الأطباء 🩺
- الأدوية (Medicaments) 💉
- الفوترة 🧮
- ملفات التأمين و CNAS 📁

## المزايا الرئيسية ✨

- واجهة مكتبية حديثة وسهلة الاستخدام
- اعتماد نمط **MVVM** لفصل الواجهة عن منطق العمل
- تخزين البيانات عبر **Entity Framework 6**
- دعم العمليات غير المتزامنة لتحسين الأداء ⚡

## التقنيات المستخدمة 🛠️

- .NET 10
- WPF
- Entity Framework 6
- CommunityToolkit.Mvvm
- PropertyChanged.Fody
- MahApps.Metro

## تشغيل المشروع 🚀

```bash
dotnet restore OrdoMill.sln
dotnet build OrdoMill.sln
dotnet run --project OrdoMill/OrdoMill.csproj
```

## هيكل الحل 📂

```text
OrdoMill/
├── OrdoMill/            # التطبيق الرئيسي (WPF)
├── OrdoMill.Data/       # طبقة الوصول للبيانات
└── OrdoMill.Resources/  # الموارد العامة والترجمة
```

---

إذا كنت مطوراً أو صيدلياً وتبحث عن نظام واضح لإدارة الصيدلية، فـ **OrdoMill** نقطة بداية ممتازة ✅
