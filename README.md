# HomeOfMemories.Api
Серверный модуль с автоматическим переводом и динамическим кешированием данных для готовой веб-оболочки


# Основные возможности
Аутентификация и профили: регистрация, вход и выход пользователей с использованием Cookie-аутентификации и безопасного хеширования паролей (`IPasswordHasher`).
Управление воспоминаниями: создание публикаций с историей, годом события, категорией и загрузкой до 12 фотографий (JPG, PNG, WebP).
Мультиязычность и перевод: встроенный словарь интерфейса (RU / EN) и автоматический перевод текстов через MyMemory API с кешированием результатов в базе данных.


# Стек технологий
Платформа: .NET 10 / ASP.NET Core Web API
База данных: Microsoft SQL Server
ORM: Entity Framework Core (Code First + Migrations)
Документация API: Swagger / OpenAPI


# Запуск проекта локально

## 1. Требования
* [.NET 10 SDK](https://microsoft.com)
* Microsoft SQL Server (Express, Developer или LocalDB)

## 2. Клонирование репозитория
```bash
git clone https://github.com
cd HomeOfMemories.Api/HomeOfMemories.Api
```

## 3. Настройка подключения к БД
Убедитесь, что в файле `appsettings.json` указана актуальная строка подключения к вашему экземпляру MS SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HomeOfMemories;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 4. Применение миграций
Выполните команду в терминале из папки с файлом `HomeOfMemories.Api.csproj`, чтобы автоматически создать базу данных, таблицы и заполнить начальные категории и словарь:
```bash
dotnet ef database update
```

## 5. Запуск приложения
```bash
dotnet run
```
<img width="1022" height="489" alt="Главная страница веб-приложения" src="https://github.com/user-attachments/assets/6c017ba6-d9ca-444d-9a9d-6229cd325b30" />

*Главная страница веб-приложения с переводом*

После запуска интерактивная документация Swagger будет доступна в браузере по адресу: `https://localhost:<порт>/swagger` (точные порты указаны в `Properties/launchSettings.json`).
