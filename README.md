# Tournois

Application de gestion de tournois, d'équipes, de joueurs et de matchs.
Le projet comprend une API ASP.NET Core et une interface web React/Vite.

## Fonctionnalités

- Authentification par JWT.
- Gestion des tournois, jeux, équipes, joueurs et matchs.
- Gestion des rôles `Admin`, `Organizer` et `Player`.
- CRUD des utilisateurs réservé aux administrateurs.
- Documentation de l'API avec Swagger.
- Interface web avec mode clair/sombre et bouton de déconnexion.

## Prérequis

- .NET SDK 11.
- Node.js et npm.
- SQL Server Express installé et démarré.
- Une base SQL Server accessible avec la chaîne configurée dans
  `backend/appsettings.json`.

## Configuration du backend

Avant le premier démarrage, vérifiez la chaîne de connexion et les paramètres
JWT dans `backend/appsettings.json`. Ne partagez pas les clés secrètes et
utilisez des variables d'environnement pour les environnements sensibles.

Pour créer automatiquement un compte administrateur, définissez les variables
suivantes dans PowerShell :

```powershell
$env:AdminSeed__Email = "admin@example.com"
$env:AdminSeed__Password = "MotDePasseAdminFort123!"
```

Le compte est créé au démarrage si les variables sont présentes.

## Installation et base de données

Depuis la racine du projet :

```powershell
cd backend
dotnet restore
dotnet ef database update
```

Si les outils Entity Framework ne sont pas installés :

```powershell
dotnet tool install --global dotnet-ef
```

## Lancer l'API

Dans un terminal :

```powershell
cd backend
dotnet run --launch-profile http
```

L'API est disponible à l'adresse `http://localhost:5192`.
Swagger est accessible à `http://localhost:5192`.

Le profil HTTPS utilise également `https://localhost:7007`.

## Lancer le frontend

Dans un autre terminal :

```powershell
cd frontend
npm install
npm run dev
```

L'interface est disponible à l'adresse `http://localhost:5173`.
Par défaut, elle utilise l'API `http://localhost:5192/api`. Pour modifier cette
adresse, créez un fichier `frontend/.env.local` :

```env
VITE_API_URL=http://localhost:5192/api
```

## Commandes utiles

### Backend

```powershell
cd backend
dotnet build
dotnet ef migrations add NomDeLaMigration
dotnet ef database update
```

### Frontend

```powershell
cd frontend
npm run build
npm run lint
npm run preview
```

## Endpoints principaux

| Méthode | URL | Accès |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Public |
| `POST` | `/api/auth/login` | Public |
| `GET` | `/api/users` | Admin |
| `POST` | `/api/users` | Admin |
| `PUT` | `/api/users/{id}` | Admin |
| `DELETE` | `/api/users/{id}` | Admin |
| `GET` | `/api/roles` | Admin |

Un fichier de requêtes est disponible dans
[`backend/API.http`](./backend/API.http).

## Structure du projet

```text
backend/
├── Controllers/   Contrôleurs HTTP
├── DTO/           Objets de transfert
├── Models/        Entités et contexte Entity Framework
├── Repositories/  Accès aux données
├── Services/      Logique métier
└── Migrations/    Migrations de base de données

frontend/
└── src/           Application React et styles
```
